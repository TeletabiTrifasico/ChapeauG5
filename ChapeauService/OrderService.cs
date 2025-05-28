using System;
using System.Collections.Generic;
using ChapeauDAL;
using ChapeauModel;

namespace ChapeauService
{
    public class OrderService
    {
        private OrderDao orderDao;

        public OrderService()
        {
            orderDao = new OrderDao();
        }

        public int CreateOrder(int tableId, int employeeId)
        {
            // Create a new order
            Order order = new Order
            {
                TableId = tableId,
                EmployeeId = employeeId,
                CreatedAt = DateTime.Now,
                IsDone = false
            };

            return orderDao.CreateOrder(order);
        }

        public void AddOrderItem(int orderId, int menuItemId, int quantity, string comment)
        {
            OrderItem item = new OrderItem
            {
                OrderId = orderId,
                MenuItemId = menuItemId,
                Quantity = quantity,
                Comment = comment,
                CreatedAt = DateTime.Now,
                Status = "Ordered"
            };

            orderDao.AddOrderItem(item);
        }

        public Order GetOrderByTableId(int tableId)
        {
            return orderDao.GetActiveOrderByTableId(tableId);
        }

        public List<OrderItem> GetOrderItemsByOrderId(int orderId)
        {
            return orderDao.GetOrderItemsByOrderId(orderId);
        }
        
        public void MarkAllItemsAsServed(int orderId)
        {
            orderDao.MarkAllItemsAsServed(orderId);
        }
    }
}