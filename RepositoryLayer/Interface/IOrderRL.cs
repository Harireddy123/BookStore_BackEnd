using System;
using System.Collections.Generic;
using System.Text;
using RepositoryLayer.Entity;

namespace RepositoryLayer.Interface
{
    public interface IOrderRL
    {
        Order PlaceOrder(int userId, int cartId);
        List<Order> GetOrdersByUserId(int userId);
    }
}
