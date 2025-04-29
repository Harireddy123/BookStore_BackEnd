using System;
using System.Collections.Generic;
using System.Text;
using BussinessLayer.Interface;
using RepositoryLayer.Entity;
using RepositoryLayer.Interface;

namespace BussinessLayer.Service
{
    public class OrderBL : IOrderBL
    {
        private readonly IOrderRL _orderRL;

        public OrderBL(IOrderRL orderRL)
        {
            _orderRL = orderRL;
        }

        public Order PlaceOrder(int userId, int cartId)
        {
            return _orderRL.PlaceOrder(userId, cartId);
        }

        public List<Order> GetOrdersByUserId(int userId)
        {
            return _orderRL.GetOrdersByUserId(userId);
        }
    }
}
