using System;
using System.Collections.Generic;
using System.Text;
using BussinessLayer.Interface;
using ModelLayer.Models;
using RepositoryLayer.Interface;

namespace BussinessLayer.Service
{
    public class BookBL : IBookBL
    {
        private readonly IBookRL _repository;

        public BookBL(IBookRL repository)
        {
            _repository = repository;
        }

        public void LoadBooks(string filePath) => _repository.LoadBooksFromCsv(filePath);

        public List<BookModel> GetAll(int pageNumber, int pageSize) => _repository.GetAllBooks(pageNumber, pageSize);

        public BookModel GetById(int id) => _repository.GetBookById(id);

        public bool AddBook(BookModel book) => _repository.AddBook(book);

        public bool Update(int id, BookModel book) => _repository.UpdateBook(id, book);

        public bool Delete(int id) => _repository.DeleteBook(id);

        public List<BookModel> SearchBooks(string query) => _repository.SearchBooks(query);

        public List<BookModel> SortByPrice(string order) => _repository.SortByPrice(order);

        public List<BookModel> GetNewArrivals() => _repository.GetNewArrivals();
    }
}
