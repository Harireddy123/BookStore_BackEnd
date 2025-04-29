using BussinessLayer.Interface;
using Microsoft.AspNetCore.Mvc;
using ModelLayer.Models;
using RepositoryLayer.Entity;
using System.Collections.Generic;

namespace BookStore.Controllers
{
    [ApiController]
    [Route("api/order")]
    public class OrderController : ControllerBase
    {
        private readonly IOrderBL _orderBL;

        public OrderController(IOrderBL orderBL)
        {
            _orderBL = orderBL;
        }

        [HttpPost("placeorder")]
        public IActionResult PlaceOrder(int cartId)
        {
            var role = User.FindFirst("custom_role")?.Value;
            if (role != "User")
                return Unauthorized(new ResponseModel<string> { Success = false, Message = "Only Users can place orders." });

            var userIdClaim = User.FindFirst("id")?.Value;
            if (!int.TryParse(userIdClaim, out var userId))
                return Unauthorized(new ResponseModel<string> { Success = false, Message = "Invalid UserId" });

            var result = _orderBL.PlaceOrder(userId, cartId);
            if (result == null)
                return BadRequest(new ResponseModel<string> { Success = false, Message = "Failed to place order." });

            return Ok(new ResponseModel<Order> { Success = true, Message = "Order placed successfully.", Data = result });
        }

        [HttpGet("getall")]
        public IActionResult GetAllOrders()
        {
            var role = User.FindFirst("custom_role")?.Value;
            if (role != "User")
                return Unauthorized(new ResponseModel<string> { Success = false, Message = "Only Users can view orders." });

            var userIdClaim = User.FindFirst("id")?.Value;
            if (!int.TryParse(userIdClaim, out var userId))
                return Unauthorized(new ResponseModel<string> { Success = false, Message = "Invalid UserId" });

            var orders = _orderBL.GetOrdersByUserId(userId);
            if (orders == null || orders.Count == 0)
                return NotFound(new ResponseModel<string> { Success = false, Message = "No orders found." });

            return Ok(new ResponseModel<List<Order>> { Success = true, Message = "Orders fetched successfully.", Data = orders });
        }
    }
}
