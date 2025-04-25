using System;
using System.Collections.Generic;
using System.Text;
using BussinessLayer.Interface;
using ModelLayer.Models;
using RepositoryLayer.Entity;
using RepositoryLayer.Interface;

namespace BussinessLayer.Service
{
    public class CartBL : ICartBL
    {
        private readonly ICartRL _cartRL;

        public CartBL(ICartRL cartRL)
        {
            _cartRL = cartRL;
        }

        public Cart AddToCart(int userId, CartModel model) => _cartRL.AddToCart(userId, model);

        public List<Cart> GetCartItems(int userId) => _cartRL.GetCartItems(userId);

        public bool RemoveFromCart(int cartId, int userId) => _cartRL.RemoveFromCart(cartId, userId);

        public Cart UpdateCartItem(int cartId, CartModel model, int userId) =>
            _cartRL.UpdateCartItem(cartId, model, userId);
    }

}
