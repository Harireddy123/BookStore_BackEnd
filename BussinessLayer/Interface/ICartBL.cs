using System;
using System.Collections.Generic;
using System.Text;
using ModelLayer.Models;
using RepositoryLayer.Entity;

namespace BussinessLayer.Interface
{
    public interface ICartBL
    {
        Cart AddToCart(int userId, CartModel model);
        List<Cart> GetCartItems(int userId);
        bool RemoveFromCart(int cartId, int userId);
        Cart UpdateCartItem(int cartId, CartModel model, int userId);
    }

}
