using System;
using System.Collections.Generic;
using System.Text;
using ModelLayer.Models;

namespace BussinessLayer.Interface
{
    public interface IBookBL
    {
        void LoadBooks(string filePath);
        List<BookModel> GetAll(int pageNumber, int pageSize);
        BookModel GetById(int id);
        bool AddBook(BookModel book);
        bool Update(int id, BookModel book);
        bool Delete(int id);
        List<BookModel> SearchBooks(string query);
        List<BookModel> SortByPrice(string order);
        List<BookModel> GetNewArrivals();

    }
}
