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
                // Create Table object with the ID
                TableId = new Table { TableId = tableId },
                // Create Employee object with the ID and required fields
                EmployeeId = new Employee { 
                    EmployeeId = employeeId,
                    Username = string.Empty,  // Required field
                    PasswordHash = string.Empty,  // Required field
                    FirstName = string.Empty,  // Required field
                    LastName = string.Empty,  // Required field
                    Email = string.Empty  // Required field
                },
                CreatedAt = DateTime.Now,
                IsDone = false
            };

            return orderDao.CreateOrder(order);
        }

        public void AddOrderItem(int orderId, int menuItemId, int quantity, string comment)
        {
            OrderItem item = new OrderItem
            {
                // Create Order object with the ID
                OrderId = new Order { OrderId = orderId },
                // Create MenuItem object with the ID
                MenuItemId = new MenuItem { MenuItemId = menuItemId },
                Quantity = quantity,
                Comment = comment,
                CreatedAt = DateTime.Now,
                // Use the enum value instead of string
                Status = OrderItem.OrderStatus.Ordered
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

        public void UpdateOrderItem(int orderItemId, int quantity, string comment)
        {
            orderDao.UpdateOrderItem(orderItemId, quantity, comment);
        }

        public void DeleteOrderItem(int orderItemId)
        {
            orderDao.DeleteOrderItem(orderItemId);
        }
    }
}