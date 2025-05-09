﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using ModelLayer.Models;
using RepositoryLayer.Context;
using RepositoryLayer.Entity;
using RepositoryLayer.Interface;

namespace RepositoryLayer.Service
{
    public class CartRL : ICartRL
    {
        private readonly BookContext _context;

        public CartRL(BookContext context)
        {
            _context = context;
        }

        public Cart AddToCart(int userId, CartModel model)
        {
            var book = _context.Books.FirstOrDefault(b => b.Id == model.BookId);
            if (book == null || book.Quantity < model.Quantity)
                return null;

            var existingCartItem = _context.Carts
                .FirstOrDefault(c => c.UserId == userId && c.BookId == model.BookId && !c.IsPurchased);

            if (existingCartItem != null)
            {
                // Update existing cart item
                int newQuantity = existingCartItem.Quantity + model.Quantity;
                if (newQuantity > book.Quantity)
                    return null;

                existingCartItem.Quantity = newQuantity;
                existingCartItem.Price = newQuantity * book.DiscountPrice;
                _context.SaveChanges();
                return existingCartItem;
            }

            // Create new cart item
            var cartItem = new Cart
            {
                UserId = userId,
                BookId = model.BookId,
                Quantity = model.Quantity,
                Price = model.Quantity * book.DiscountPrice
            };

            _context.Carts.Add(cartItem);
            _context.SaveChanges();

            return cartItem;
        }



        public List<Cart> GetCartItems(int userId)
        {
            return _context.Carts
                .Include(c => c.Book)
                .Where(c => c.UserId == userId
                            && !c.IsPurchased
                            && c.Book != null
                            && c.Book.Quantity > 0)
                .ToList();
        }



        public bool RemoveFromCart(int cartId, int userId)
        {
            var cart = _context.Carts.FirstOrDefault(c => c.CartId == cartId && c.UserId == userId);
            if (cart == null) return false;

            _context.Carts.Remove(cart);
            _context.SaveChanges();
            return true;
        }

        public Cart UpdateCartItem(int cartId, CartModel model, int userId)
        {
            var cart = _context.Carts.FirstOrDefault(c => c.CartId == cartId && c.UserId == userId);
            var book = _context.Books.FirstOrDefault(b => b.Id == model.BookId);

            if (cart == null || book == null)
                return null;

            if (model.Quantity > book.Quantity)
                return null;

            if (model.Quantity == 0)
            {
                // If updated quantity is 0, remove the cart item
                _context.Carts.Remove(cart);
                _context.SaveChanges();
                return null; // Or you can return a different response indicating deletion
            }

            // Update cart item
            cart.Quantity = model.Quantity;
            cart.Price = model.Quantity * book.DiscountPrice;
            _context.SaveChanges();

            return cart;
        }


        public decimal GetCartTotal(int userId)
        {
            return _context.Carts
                .Where(c => c.UserId == userId && !c.IsPurchased)
                .Sum(c => c.Price);
        }

    }
}
