using System;
using System.Collections.Generic;
using System.Text;
using RepositoryLayer.Entity;

namespace BussinessLayer.Interface
{
    public interface IOrderBL
    {
        Order PlaceOrder(int userId, int cartId);
        List<Order> GetOrdersByUserId(int userId);
    }
}
