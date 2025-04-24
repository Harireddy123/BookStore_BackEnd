using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using ModelLayer.Models;
using RepositoryLayer.Context;
using RepositoryLayer.Entity;
using RepositoryLayer.Interface;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace RepositoryLayer.Service
{
    public class BookRL : IBookRL
    {
        private readonly BookContext _context;

        public BookRL(BookContext context)
        {
            _context = context;
        }

        public void LoadBooksFromCsv(string filePath)
        {
            var lines = File.ReadAllLines(filePath).Skip(1); // Skip header row

            foreach (var line in lines)
            {
                var values = line.Split(',').Select(v => v.Trim()).ToArray();

                if (values.Length < 11)
                {
                    Console.WriteLine($"Skipped line (less than 11 fields): {line}");
                    continue;
                }

                if (!decimal.TryParse(values[4], out decimal price))
                {
                    Console.WriteLine($"Invalid price format: {values[4]}");
                    continue;
                }

                if (!decimal.TryParse(values[5], out decimal discountPrice))
                {
                    Console.WriteLine($"Invalid discount price format: {values[5]}");
                    continue;
                }

                if (!int.TryParse(values[6], out int quantity))
                {
                    Console.WriteLine($"Invalid quantity format: {values[6]}");
                    continue;
                }

                string adminUserId = values[8]; // Assign AdminUserId directly from CSV

                if (!DateTime.TryParse(values[9], out DateTime createdAt))
                {
                    Console.WriteLine($"Invalid CreatedAt format: {values[9]}");
                    continue;
                }

                if (!DateTime.TryParse(values[10], out DateTime updatedAt))
                {
                    Console.WriteLine($"Invalid UpdatedAt format: {values[10]}");
                    continue;
                }

                var book = new Book
                {
                    BookName = values[1],
                    Author = values[2],
                    Description = values[3],
                    Price = price,
                    DiscountPrice = discountPrice,
                    Quantity = quantity,
                    BookImage = values[7],
                    AdminUserId = adminUserId,
                    CreatedAt = createdAt,
                    UpdatedAt = updatedAt
                };

                _context.Books.Add(book);
            }

            _context.SaveChanges();
            Console.WriteLine("CSV data loaded successfully.");
        }



        public List<BookModel> GetAllBooks(int pageNumber, int pageSize)
        {
            return _context.Books
                .OrderBy(b => b.BookName)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .AsEnumerable() // Switches from IQueryable to IEnumerable (in-memory)
                .Select(b => BookRL.MapToModel(b)) // Call static method
                .ToList();
        }

        public BookModel GetBookById(int id)
        {
            var book = _context.Books.FirstOrDefault(b => b.Id == id);
            return book != null ? MapToModel(book) : null;
        }

        public bool UpdateBook(int id, BookModel model)
        {
            var book = _context.Books.FirstOrDefault(b => b.Id == id);
            if (book == null) return false;

            book.BookName = model.BookName;
            book.Author = model.Author;
            book.Description = model.Description;
            book.Price = model.Price;
            book.DiscountPrice = model.DiscountPrice;
            book.Quantity = model.Quantity;
            book.BookImage = model.BookImage;
            book.UpdatedAt = DateTime.Now;

            _context.SaveChanges();
            return true;
        }

        public bool DeleteBook(int id)
        {
            var book = _context.Books.FirstOrDefault(b => b.Id == id);
            if (book == null) return false;

            _context.Books.Remove(book);
            _context.SaveChanges();
            return true;
        }

        public List<BookModel> SearchBooks(string query)
        {
            try
            {
                Console.WriteLine($"Search Query: '{query}'");

                if (string.IsNullOrWhiteSpace(query))
                    return new List<BookModel>();

                var books = _context.Books
                    .Where(b => b.Author != null && b.Author.ToLower().Contains(query.ToLower()))
                    .ToList();

                Console.WriteLine($"Matched Books Count: {books.Count}");

                return books.Select(b => MapToModel(b)).ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in SearchBooks: {ex.ToString()}");
                return new List<BookModel>();
            }
        }

        public List<BookModel> SortByPrice(string order)
        {
            try
            {
                var sortedBooks = order.ToLower() == "desc"
                    ? _context.Books.OrderByDescending(b => b.Price)
                    : _context.Books.OrderBy(b => b.Price);

                return sortedBooks.Select(b => MapToModel(b)).ToList();
            }
            catch (Exception ex)
            {
                // Log the error
                Console.WriteLine($"Error in SortByPrice: {ex.Message}");
                return new List<BookModel>();
            }
        }

        public List<BookModel> GetNewArrivals()
        {
            try
            {
                var dateThreshold = DateTime.Now.AddDays(-30);
                return _context.Books
                    .Where(b => b.CreatedAt >= dateThreshold)
                    .Select(b => MapToModel(b))
                    .ToList();
            }
            catch (Exception ex)
            {
                // Log the error
                Console.WriteLine($"Error in GetNewArrivals: {ex.Message}");
                return new List<BookModel>();
            }
        }


        public bool AddBook(BookModel model)
        {
            var book = new Book
            {
                BookName = model.BookName,
                Author = model.Author,
                Description = model.Description,
                Price = model.Price,
                DiscountPrice = model.DiscountPrice,
                Quantity = model.Quantity,
                BookImage = model.BookImage,
                AdminUserId = model.AdminUserId,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };

            _context.Books.Add(book);
            return _context.SaveChanges() > 0;
        }

        public static BookModel MapToModel(Book book)
        {
            return new BookModel
            {
                BookName = book.BookName,
                Author = book.Author,
                Description = book.Description,
                Price = book.Price,
                DiscountPrice = book.DiscountPrice,
                Quantity = book.Quantity,
                BookImage = book.BookImage,
                AdminUserId = book.AdminUserId,
                CreatedAt = book.CreatedAt,
                UpdatedAt = book.UpdatedAt
            };
        }

    }
}
