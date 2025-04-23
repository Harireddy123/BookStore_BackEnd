using System;
using System.Collections.Generic;
using System.Text;
using ModelLayer.Models;

namespace RepositoryLayer.Interface
{
    public interface IBookRL
    {
        void LoadBooksFromCsv(string filePath);
        List<BookModel> GetAllBooks(int pageNumber, int pageSize);
        BookModel GetBookById(int id);
        bool UpdateBook(BookModel book);
        bool DeleteBook(int id);
        List<BookModel> SearchBooks(string query);
        List<BookModel> SortByPrice(string order);
        List<BookModel> GetNewArrivals();

    }
}
