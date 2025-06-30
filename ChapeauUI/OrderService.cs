using System;
using System.Collections.Generic;
using System.Linq;
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

        public List<TableOrderStatus> GetTableOrderStatuses()
        {
            return orderDao.GetTableOrderStatuses();
        }

        public void MarkOrderItemAsServed(int orderItemId)
        {
            orderDao.MarkOrderItemAsServed(orderItemId);
        }

        public Order GetOrderWithItemsById(int orderId)
        {
            return orderDao.GetOrderWithItemsById(orderId);
        }

        public OrderItem FindExistingOrderItem(List<OrderItem> orderItems, MenuItem menuItem, string comment)
        {
            return orderItems.Find(o => o.MenuItemId.MenuItemId == menuItem.MenuItemId && 
                                       (o.Comment ?? "") == (comment ?? ""));
        }

        public void UpdateOrderItemQuantity(OrderItem item, int quantity)
        {
            item.Quantity += quantity;
            item.CreatedAt = DateTime.Now;
        }

        public OrderItem CreateNewOrderItem(MenuItem menuItem, int quantity, string comment, int? orderId = null)
        {
            return new OrderItem
            {
                OrderId = new Order { OrderId = orderId ?? 0 },
                MenuItemId = menuItem,
                Quantity = quantity,
                Comment = comment,
                CreatedAt = DateTime.Now,
                Status = OrderItem.OrderStatus.Ordered
            };
        }

        public bool ValidateOrderHasItems(List<OrderItem> orderItems)
        {
            return orderItems.Count > 0;
        }

        public void SaveOrderItems(int orderId, List<OrderItem> orderItems)
        {
            foreach (var item in orderItems)
            {
                if (item.OrderItemId == 0) // New item
                {
                    AddOrderItem(orderId, item.MenuItemId.MenuItemId, item.Quantity, item.Comment);
                }
            }
        }

        public bool AreAllItemsServed(List<OrderItem> orderItems)
        {
            return orderItems.All(item => item.Status == OrderItem.OrderStatus.Served);
        }

        public bool IsOrderEmpty(List<OrderItem> orderItems)
        {
            return orderItems.Count == 0;
        }

        public bool CanBeMarkedAsServed(OrderItem item, bool isExistingOrder)
        {
            return isExistingOrder && item.OrderItemId != 0;
        }

        public bool IsAlreadyServed(OrderItem item)
        {
            return item.Status == OrderItem.OrderStatus.Served;
        }

        public void MarkItemAsServed(OrderItem item)
        {
            MarkOrderItemAsServed(item.OrderItemId);
            item.Status = OrderItem.OrderStatus.Served;
        }

        public string GetItemName(OrderItem item)
        {
            return item?.MenuItemId?.Name ?? "Unknown Item";
        }

        public decimal GetItemPrice(OrderItem item)
        {
            return item?.MenuItemId?.Price ?? 0;
        }

        public string GetItemStatus(OrderItem item)
        {
            return item?.Status.ToString() ?? "Unknown";
        }

        public decimal CalculateItemSubtotal(OrderItem item)
        {
            return item.Quantity * GetItemPrice(item);
        }

        public decimal CalculateOrderTotal(List<OrderItem> orderItems)
        {
            return orderItems.Sum(item => CalculateItemSubtotal(item));
        }

        public void RemoveOrderItem(List<OrderItem> orderItems, OrderItem item)
        {
            orderItems.Remove(item);
        }

        public void UpdateOrderItemDetails(OrderItem item, int quantity, string comment)
        {
            item.Quantity = quantity;
            item.Comment = comment;
        }
    }
}