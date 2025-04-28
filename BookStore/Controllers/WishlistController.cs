using BussinessLayer.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ModelLayer.Models;
using RepositoryLayer.Entity;
using RepositoryLayer.Migrations;
using System.Collections.Generic;
using System.Linq;

namespace BookStore.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class WishlistController : ControllerBase
    {
        private readonly IWishlistBL _wishlistBL;

        public WishlistController(IWishlistBL wishlistBL)
        {
            _wishlistBL = wishlistBL;
        }

        [HttpPost("add")]
        public IActionResult AddToWishlist([FromBody] WishlistModel model)
        {
            var role = User.FindFirst("custom_role")?.Value;
            if (role != "User")
                return Unauthorized(new ResponseModel<string> { Success = false, Message = "Only Users can add to wishlist." });

            var userIdClaim = User.FindFirst("id")?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
                return Unauthorized(new ResponseModel<string> { Success = false, Message = "Invalid UserId" });

            var existingItem = _wishlistBL.GetWishlistByUserId(userId)
                .FirstOrDefault(w => w.BookId == model.BookId);

            if (existingItem != null)
            {
                return Conflict(new ResponseModel<string> { Success = false, Message = "Book is already in wishlist." });
            }

            var result = _wishlistBL.AddToWishlist(userId, model.BookId);
            if (result == null)
                return BadRequest(new ResponseModel<string> { Success = false, Message = "Failed to add book to wishlist." });

            // Load book details (you need to include Book in Wishlist entity)
            var bookData = result.Book;

            var response = new
            {
                UserId = userId,
                WishlistId = result.WishlistId,
                Book = bookData
            };

            return Ok(new ResponseModel<object> { Success = true, Message = "Book added to wishlist.", Data = response });
        }



        [HttpDelete("remove")]
        public IActionResult RemoveFromWishlist(int wishlistId)
        {
            var role = User.FindFirst("custom_role")?.Value;
            if (role != "User")
                return Unauthorized(new ResponseModel<string> { Success = false, Message = "Only Users can remove from wishlist." });

            var userIdClaim = User.FindFirst("id")?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
                return Unauthorized(new ResponseModel<string> { Success = false, Message = "Invalid UserId" });

            var removed = _wishlistBL.RemoveFromWishlist(wishlistId, userId);
            if (!removed)
                return NotFound(new ResponseModel<string> { Success = false, Message = "Wishlist item not found." });


            return Ok(new ResponseModel<string> { Success = true, Message = "Book removed from wishlist.",  });
        }

        [Authorize]
        [HttpGet("getall")]
        public IActionResult GetWishlist()
        {
            var role = User.FindFirst("custom_role")?.Value;
            if (role != "User")
                return Unauthorized(new ResponseModel<string> { Success = false, Message = "Only Users can view wishlist." });

            var userIdClaim = User.FindFirst("id")?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
                return Unauthorized(new ResponseModel<string> { Success = false, Message = "Invalid UserId" });

            var list = _wishlistBL.GetWishlistByUserId(userId)?.ToList();  // Ensure it's a List<Wishlist>

            if (list == null || list.Count == 0)
                return NotFound(new ResponseModel<string> { Success = false, Message = "Wishlist is empty." });

            return Ok(new ResponseModel<List<RepositoryLayer.Entity.WishList>>
            {
                Success = true,
                Message = "Wishlist fetched successfully.",
                Data = list
            });

        }


    }

}
