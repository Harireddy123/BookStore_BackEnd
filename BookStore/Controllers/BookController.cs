using System.IO;
using System.Linq;
using BussinessLayer.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ModelLayer.Models;

namespace BookStore.Controllers
{
    [ApiController]
    [Route("api/book")]
    public class BookController : ControllerBase
    {
        private readonly IBookBL _bookBusiness;

        public BookController(IBookBL bookBusiness)
        {
            _bookBusiness = bookBusiness;
        }

        [Authorize]
        [HttpPost]
        public IActionResult LoadCsvData()
        {
            var role = User.FindFirst("custom_role")?.Value;

            if (role != "Admin")
                return Unauthorized(new ResponseModel<string> { Success = false, Message = "Only admins can load data from CSV." });

            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "CSV", "books.csv");

            if (!System.IO.File.Exists(filePath))
            {
                return NotFound(new ResponseModel<string> { Success = false, Message = $"CSV file not found at: {filePath}" });
            }

            _bookBusiness.LoadBooks(filePath);
            return Ok(new ResponseModel<string> { Success = true, Message = "CSV Data Loaded to DB" });
        }

        [Authorize]
        [HttpPost("add")]
        public IActionResult AddBook(BookModel book)
        {
            var role = User.FindFirst("custom_role")?.Value;
            if (role != "Admin")
                return Unauthorized(new ResponseModel<string> { Success = false, Message = "Only Admins can update books" });

            var result = _bookBusiness.AddBook(book);
            if (result)
                return Ok(new ResponseModel<string> { Success = true, Message = "Book added successfully"});
            else
                return BadRequest(new ResponseModel<string> { Success = false, Message = "failed to add book" });
        }



        [Authorize]
        [HttpPut("update")]
        public IActionResult Update(int id, BookModel book)
        {
            var role = User.FindFirst("custom_role")?.Value;
            if (role != "Admin")
                return Unauthorized(new ResponseModel<string> { Success = false, Message = "Only Admins can update books" });

            if (_bookBusiness.Update(id, book))
                return Ok(new ResponseModel<string> { Success = true, Message = "Book updated successfully" });
            else
                return NotFound(new ResponseModel<string> { Success = false, Message = "Book not found" });
        }

        [Authorize]
        [HttpDelete("delete")]
        public IActionResult Delete(int id)
        {
            var role = User.FindFirst("custom_role")?.Value;
            if (role != "Admin")
                return Unauthorized(new ResponseModel<string> { Success = false, Message = "Only Admins can delete books" });

            if (_bookBusiness.Delete(id))
                return Ok(new ResponseModel<string> { Success = true, Message = "Book deleted successfully" });
            else
                return NotFound(new ResponseModel<string> { Success = false, Message = "Book not found" });
        }

        [Authorize]
        [HttpGet("getAll")]
        public IActionResult GetAllBooks(int pageNumber = 1, int pageSize = 10)
        {
            var books = _bookBusiness.GetAll(pageNumber, pageSize);

            if (books != null && books.Any())
                return Ok(new ResponseModel<object> { Success = true, Message = "Books retrieved successfully", Data = books });
            else
                return NotFound(new ResponseModel<string> { Success = false, Message = "No books found" });
        }

        [Authorize]
        [HttpGet("getById")]
        public IActionResult GetById(int id)
        {
            var book = _bookBusiness.GetById(id);

            if (book == null)
                return NotFound(new ResponseModel<string> { Success = false, Message = "Book not found" });

            return Ok(new ResponseModel<object> { Success = true, Message = "Book retrieved successfully", Data = book });
        }

        [Authorize]
        [HttpGet("search")]
        public IActionResult Search([FromQuery] string query)
        {
            var result = _bookBusiness.SearchBooks(query);
            return Ok(new ResponseModel<object> { Success = true, Message = "Search completed", Data = result });
        }

        [Authorize]
        [HttpGet("sort-by-price")]
        public IActionResult SortByPrice([FromQuery] string order = "asc")
        {
            var result = _bookBusiness.SortByPrice(order);
            return Ok(new ResponseModel<object> { Success = true, Message = $"Books sorted by price ({order})", Data = result });
        }

        [Authorize]
        [HttpGet("new-arrivals")]
        public IActionResult GetNewArrivals()
        {
            var result = _bookBusiness.GetNewArrivals();
            return Ok(new ResponseModel<object> { Success = true, Message = "New arrivals retrieved", Data = result });
        }
    }
}
