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
                return Forbid("Only admins can load data from CSV.");

            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "CSV", "books.csv");

            if (!System.IO.File.Exists(filePath))
            {
                return NotFound("CSV file not found at: " + filePath);
            }

            _bookBusiness.LoadBooks(filePath);
            return Ok("CSV Data Loaded to DB");
        }


        [Authorize]
        [HttpPut("update")]
        public IActionResult Update(BookModel book)
        {
            var role = User.FindFirst("custom_role")?.Value;
            if (role != "Admin")
                return Unauthorized("Only Admins can update books");

            if (_bookBusiness.Update(book))
                return Ok("Updated");
            else
                return NotFound();
        }

        [Authorize]
        [HttpDelete("delete")]
        public IActionResult Delete(int id)
        {
            var role = User.FindFirst("custom_role")?.Value;
            if (role != "Admin")
                return Unauthorized("Only Admins can delete books");

            if (_bookBusiness.Delete(id))
                return Ok("Deleted");
            else
                return NotFound();
        }

        [Authorize]
        [HttpGet("getAll")]
        public IActionResult GetAllBooks(int pageNumber = 1, int pageSize = 10)
        {
            var books = _bookBusiness.GetAll(pageNumber, pageSize);

            if (books != null && books.Any())
                return Ok(books);
            else
                return NotFound("No books found.");
        }

        [Authorize]
        [HttpGet("getById")]
        public IActionResult GetById(int id)
        {
            var book = _bookBusiness.GetById(id);

            if (book == null)
            {
                return NotFound();
            }

            return Ok(book);
        }



        [Authorize]
        [HttpGet("search")]
        public IActionResult Search([FromQuery] string query)
            => Ok(_bookBusiness.SearchBooks(query));

        [Authorize]
        [HttpGet("sort-by-price")]
        public IActionResult SortByPrice([FromQuery] string order = "asc")
            => Ok(_bookBusiness.SortByPrice(order));

        [Authorize]
        [HttpGet("new-arrivals")]
        public IActionResult GetNewArrivals()
            => Ok(_bookBusiness.GetNewArrivals());

    }
}
