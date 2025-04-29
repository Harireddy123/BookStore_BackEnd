using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.EntityFrameworkCore;
using RepositoryLayer.Context;
using RepositoryLayer.Entity;
using RepositoryLayer.Interface;

namespace RepositoryLayer.Service
{
    public class OrderRL : IOrderRL
    {
        private readonly BookContext _context;

        public OrderRL(BookContext context)
        {
            _context = context;
        }

        public Order PlaceOrder(int userId, int cartId)
        {
            var cart = _context.Carts
                .Include(c => c.Book)
                .FirstOrDefault(c => c.CartId == cartId && c.UserId == userId && !c.IsPurchased);

            if (cart == null || cart.Book == null || cart.Book.Quantity < cart.Quantity)
                return null;

            var order = new Order
            {
                UserId = userId,
                BookId = cart.BookId,
                Quantity = cart.Quantity,
                TotalPrice = cart.Quantity * cart.Book.DiscountPrice,
                OrderDate = DateTime.Now
            };

            // Reduce Book Quantity
            cart.Book.Quantity -= cart.Quantity;

            // Mark Cart as Purchased
            cart.IsPurchased = true;

            // Remove Cart Item
            _context.Carts.Remove(cart);

            // Add Order
            _context.Orders.Add(order);

            _context.SaveChanges();
            return order;
        }

        public List<Order> GetOrdersByUserId(int userId)
        {
            return _context.Orders
                           .Include(o => o.Book)
                           .Where(o => o.UserId == userId)
                           .ToList();
        }
    }
}
