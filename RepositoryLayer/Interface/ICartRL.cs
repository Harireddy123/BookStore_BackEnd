using System;
using System.Collections.Generic;
using System.Text;
using ModelLayer.Models;
using RepositoryLayer.Entity;

namespace RepositoryLayer.Interface
{
    public interface ICartRL
    {
        Cart AddToCart(int userId, CartModel model);
        List<Cart> GetCartItems(int userId);
        bool RemoveFromCart(int cartId, int userId);
        Cart UpdateCartItem(int cartId, CartModel model, int userId);
    }

}
