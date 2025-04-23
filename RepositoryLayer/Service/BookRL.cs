using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using ModelLayer.Models;
using RepositoryLayer.Interface;

namespace RepositoryLayer.Service
{
    public class BookRL : IBookRL
    {
        private readonly string _connectionString;

        public BookRL(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public void LoadBooksFromCsv(string filePath)
        {
            var lines = File.ReadAllLines(filePath).Skip(1);

            using var connection = new SqlConnection(_connectionString);
            connection.Open();

            foreach (var line in lines)
            {
                var values = line.Split(',');

                if (values.Length < 11)
                {
                    Console.WriteLine($"Skipping line due to insufficient columns: {line}");
                    continue;
                }

                if (!decimal.TryParse(values[4], out decimal price))
                {
                    Console.WriteLine($"Invalid price in line: {line}");
                    continue;
                }
                if (!decimal.TryParse(values[5], out decimal discountPrice))
                {
                    Console.WriteLine($"Invalid discount price in line: {line}");
                    continue;
                }
                if (!int.TryParse(values[6], out int quantity))
                {
                    Console.WriteLine($"Invalid quantity in line: {line}");
                    continue;
                }
                if (!DateTime.TryParse(values[9], out DateTime createdAt))
                {
                    Console.WriteLine($"Invalid createdAt in line: {line}");
                    continue;
                }
                if (!DateTime.TryParse(values[10], out DateTime updatedAt))
                {
                    Console.WriteLine($"Invalid updatedAt in line: {line}");
                    continue;
                }

                var command = new SqlCommand(
                    "INSERT INTO Books (BookName, Author, Description, Price, DiscountPrice, Quantity, BookImage, CreatedAt, UpdatedAt) " +
                    "VALUES (@BookName, @Author, @Description, @Price, @DiscountPrice, @Quantity, @BookImage, @CreatedAt, @UpdatedAt)", connection);

                command.Parameters.AddWithValue("@BookName", values[1].Trim());
                command.Parameters.AddWithValue("@Author", values[2].Trim());
                command.Parameters.AddWithValue("@Description", values[3].Trim());
                command.Parameters.AddWithValue("@Price", price);
                command.Parameters.AddWithValue("@DiscountPrice", discountPrice);
                command.Parameters.AddWithValue("@Quantity", quantity);
                command.Parameters.AddWithValue("@BookImage", values[7].Trim());
                command.Parameters.AddWithValue("@CreatedAt", createdAt);
                command.Parameters.AddWithValue("@UpdatedAt", updatedAt);

                command.ExecuteNonQuery();
            }
        }


        public List<BookModel> GetAllBooks(int pageNumber, int pageSize)
        {
            var books = new List<BookModel>();

            using var connection = new SqlConnection(_connectionString);

            // Calculate how many rows to skip
            int offset = (pageNumber - 1) * pageSize;

            var command = new SqlCommand(@"
        SELECT * FROM Books
        ORDER BY Id
        OFFSET @Offset ROWS
        FETCH NEXT @PageSize ROWS ONLY", connection);

            command.Parameters.AddWithValue("@Offset", offset);
            command.Parameters.AddWithValue("@PageSize", pageSize);

            connection.Open();
            var reader = command.ExecuteReader();

            while (reader.Read())
            {
                books.Add(new BookModel
                {
                    Id = (int)reader["Id"],
                    BookName = reader["BookName"].ToString(),
                    Author = reader["Author"].ToString(),
                    Description = reader["Description"].ToString(),
                    Price = (decimal)reader["Price"],
                    DiscountPrice = (decimal)reader["DiscountPrice"],
                    Quantity = (int)reader["Quantity"],
                    BookImage = reader["BookImage"].ToString(),
                    CreatedAt = (DateTime)reader["CreatedAt"],
                    UpdatedAt = (DateTime)reader["UpdatedAt"]
                });
            }

            return books;
        }


        public BookModel GetBookById(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            var command = new SqlCommand("SELECT * FROM Books WHERE Id = @Id", connection);
            command.Parameters.AddWithValue("@Id", id);
            connection.Open();
            var reader = command.ExecuteReader();

            if (reader.Read())
            {
                return new BookModel
                {
                    Id = (int)reader["Id"],
                    BookName = reader["BookName"].ToString(),
                    Author = reader["Author"].ToString(),
                    Description = reader["Description"].ToString(),
                    Price = (decimal)reader["Price"],
                    DiscountPrice = (decimal)reader["DiscountPrice"],
                    Quantity = (int)reader["Quantity"],
                    BookImage = reader["BookImage"].ToString(),
                    CreatedAt = (DateTime)reader["CreatedAt"],
                    UpdatedAt = (DateTime)reader["UpdatedAt"]
                };
            }

            return null;
        }

        public bool UpdateBook(BookModel book)
        {
            using var connection = new SqlConnection(_connectionString);
            var command = new SqlCommand("UPDATE Books SET BookName=@BookName, Author=@Author, Description=@Description, Price=@Price, DiscountPrice=@DiscountPrice, Quantity=@Quantity, BookImage=@BookImage, CreatedAt=@CreatedAt, UpdatedAt=@UpdatedAt WHERE Id=@Id", connection);

            command.Parameters.AddWithValue("@BookName", book.BookName);
            command.Parameters.AddWithValue("@Author", book.Author);
            command.Parameters.AddWithValue("@Description", book.Description);
            command.Parameters.AddWithValue("@Price", book.Price);
            command.Parameters.AddWithValue("@DiscountPrice", book.DiscountPrice);
            command.Parameters.AddWithValue("@Quantity", book.Quantity);
            command.Parameters.AddWithValue("@BookImage", book.BookImage);
            command.Parameters.AddWithValue("@CreatedAt", book.CreatedAt);
            command.Parameters.AddWithValue("@UpdatedAt", book.UpdatedAt);
            command.Parameters.AddWithValue("@Id", book.Id);

            connection.Open();
            return command.ExecuteNonQuery() > 0;
        }

        public bool DeleteBook(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            var command = new SqlCommand("DELETE FROM Books WHERE Id=@Id", connection);
            command.Parameters.AddWithValue("@Id", id);
            connection.Open();
            return command.ExecuteNonQuery() > 0;
        }

        public List<BookModel> SearchBooks(string query)
        {
            var books = new List<BookModel>();
            using var connection = new SqlConnection(_connectionString);
            var command = new SqlCommand("SELECT * FROM Books WHERE BookName LIKE @Query OR Author LIKE @Query", connection);
            command.Parameters.AddWithValue("@Query", $"%{query}%");
            connection.Open();
            var reader = command.ExecuteReader();

            while (reader.Read())
            {
                books.Add(ReadBook(reader));
            }

            return books;
        }

        public List<BookModel> SortByPrice(string order)
        {
            var books = new List<BookModel>();
            using var connection = new SqlConnection(_connectionString);

            string sortOrder = order.ToLower() == "desc" ? "DESC" : "ASC";
            var command = new SqlCommand($"SELECT * FROM Books ORDER BY Price {sortOrder}", connection);
            connection.Open();
            var reader = command.ExecuteReader();

            while (reader.Read())
            {
                books.Add(ReadBook(reader));
            }

            return books;
        }

        public List<BookModel> GetNewArrivals()
        {
            var books = new List<BookModel>();
            using var connection = new SqlConnection(_connectionString);
            var command = new SqlCommand("SELECT * FROM Books WHERE CreatedAt >= @RecentDate", connection);
            command.Parameters.AddWithValue("@RecentDate", DateTime.Now.AddDays(-30)); // last 30 days
            connection.Open();
            var reader = command.ExecuteReader();

            while (reader.Read())
            {
                books.Add(ReadBook(reader));
            }

            return books;
        }

        private BookModel ReadBook(SqlDataReader reader)
        {
            return new BookModel
            {
                Id = (int)reader["Id"],
                BookName = reader["BookName"].ToString(),
                Author = reader["Author"].ToString(),
                Description = reader["Description"].ToString(),
                Price = (decimal)reader["Price"],
                DiscountPrice = (decimal)reader["DiscountPrice"],
                Quantity = (int)reader["Quantity"],
                BookImage = reader["BookImage"].ToString(),
                CreatedAt = (DateTime)reader["CreatedAt"],
                UpdatedAt = (DateTime)reader["UpdatedAt"]
            };
        }

    }
}
