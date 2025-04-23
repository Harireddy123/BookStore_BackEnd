using System;
using System.Collections.Generic;
using System.Text;
using ModelLayer.Models;

namespace BussinessLayer.Interface
{
    public interface IBookBL
    {
        public void LoadBooks(string filePath);
        public List<BookModel> GetAll(int pageNumber, int pageSize);
        public BookModel GetById(int id);
        public bool Update(BookModel book);
        public bool Delete(int id);

        List<BookModel> SearchBooks(string query);
        List<BookModel> SortByPrice(string order);
        List<BookModel> GetNewArrivals();

    }
}
