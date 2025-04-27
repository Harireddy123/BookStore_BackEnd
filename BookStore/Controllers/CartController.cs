using System.Linq;
using BussinessLayer.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ModelLayer.Models;
using RepositoryLayer.Entity;

namespace BookStore.Controllers
{
    [ApiController]
    [Route("api/cart")]
    public class CartController : ControllerBase
    {
        private readonly ICartBL _cartBL;

        public CartController(ICartBL cartBL)
        {
            _cartBL = cartBL;
        }

        [Authorize]
        [HttpPost("add")]
        public IActionResult AddToCart(CartModel model)
        {
            var role = User.FindFirst("custom_role")?.Value;
            if (role != "User")
                return Unauthorized(new ResponseModel<string> { Success = false, Message = "Only Users can add to cart." });

            var userIdClaim = User.FindFirst("id")?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
                return Unauthorized(new ResponseModel<string> { Success = false, Message = "Invalid userId" });

            var result = _cartBL.AddToCart(userId, model);
            if (result == null)
                return BadRequest(new ResponseModel<string> { Success = false, Message = "Book not available or quantity invalid." });

            return Ok(new ResponseModel<object> { Success = true, Message = "Book added to cart", Data = result });
        }

        [Authorize]
        [HttpGet]
        public IActionResult GetCartItems()
        {
            var role = User.FindFirst("custom_role")?.Value;
            if (role != "User")
                return Unauthorized(new ResponseModel<string> { Success = false, Message = "Only Users can view cart items." });

            var userIdClaim = User.FindFirst("id")?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
                return Unauthorized(new ResponseModel<string> { Success = false, Message = "Invalid userId" });

            var items = _cartBL.GetCartItems(userId);
            if (items == null || items.Count == 0)
                return NotFound(new ResponseModel<string> { Success = false, Message = "No items in cart." });

            // 👉 Calculate total price here
            var totalPrice = items.Sum(c => c.Price);

            var response = new
            {
                CartItems = items,
                TotalPrice = totalPrice
            };

            return Ok(new ResponseModel<object>
            {
                Success = true,
                Message = "Cart items retrieved",
                Data = response
            });
        }


        [Authorize]
        [HttpPut("update")]
        public IActionResult UpdateCartItem(int cartId, CartModel model)
        {
            var role = User.FindFirst("custom_role")?.Value;
            if (role != "User")
                return Unauthorized(new ResponseModel<string> { Success = false, Message = "Only Users can update cart." });

            var userIdClaim = User.FindFirst("id")?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
                return Unauthorized(new ResponseModel<string> { Success = false, Message = "Invalid userId" });

            var updated = _cartBL.UpdateCartItem(cartId, model, userId);
            if (updated == null)
                return BadRequest(new ResponseModel<string> { Success = false, Message = "Unable to update cart item." });

            return Ok(new ResponseModel<object> { Success = true, Message = "Cart item updated", Data = updated });
        }

        [Authorize]
        [HttpDelete("remove")]
        public IActionResult RemoveFromCart(int cartId)
        {
            var role = User.FindFirst("custom_role")?.Value;
            if (role != "User")
                return Unauthorized(new ResponseModel<string> { Success = false, Message = "Only Users can remove from cart." });

            var userIdClaim = User.FindFirst("id")?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
                return Unauthorized(new ResponseModel<string> { Success = false, Message = "Invalid userId" });

            var removed = _cartBL.RemoveFromCart(cartId, userId);
            if (!removed)
                return NotFound(new ResponseModel<string> { Success = false, Message = "Cart item not found." });

            return Ok(new ResponseModel<string> { Success = true, Message = "Cart item removed successfully" });
        }


    }
}
