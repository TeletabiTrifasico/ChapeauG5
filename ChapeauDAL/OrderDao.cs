using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using ChapeauModel;

namespace ChapeauDAL
{
    public class OrderDao : BaseDao
    {
        public int CreateOrder(Order order)
        {
            string query = @"
                INSERT INTO [Order] (table_id, employee_id, created_at, is_done)
                VALUES (@TableId, @EmployeeId, @CreatedAt, @IsDone);
                SELECT SCOPE_IDENTITY();";
            
            SqlParameter[] parameters = new SqlParameter[]
            {
                // Extract the TableId value from the Table object
                new SqlParameter("@TableId", order.TableId != null ? order.TableId.TableId : (object)DBNull.Value),
                // Extract the EmployeeId value from the Employee object
                new SqlParameter("@EmployeeId", order.EmployeeId != null ? order.EmployeeId.EmployeeId : (object)DBNull.Value),
                new SqlParameter("@CreatedAt", order.CreatedAt),
                new SqlParameter("@IsDone", order.IsDone)
            };
            
            return ExecuteInsertQuery(query, parameters);
        }
        
        public void AddOrderItem(OrderItem item)
        {
            string query = @"
                INSERT INTO Order_Item (order_id, menu_item_id, quantity, comment, created_at, status)
                VALUES (@OrderId, @MenuItemId, @Quantity, @Comment, @CreatedAt, @Status)";
            
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@OrderId", item.OrderId != null ? item.OrderId.OrderId : (object)DBNull.Value),
                new SqlParameter("@MenuItemId", item.MenuItemId != null ? item.MenuItemId.MenuItemId : (object)DBNull.Value),
                new SqlParameter("@Quantity", item.Quantity),
                new SqlParameter("@Comment", string.IsNullOrEmpty(item.Comment) ? (object)DBNull.Value : item.Comment),
                new SqlParameter("@CreatedAt", item.CreatedAt),
                // Fix: Use OrderStatus enum instead of Status
                new SqlParameter("@Status", item.Status.ToString())
            };
            
            ExecuteEditQuery(query, parameters);
        }
        
        public Order GetActiveOrderByTableId(int tableId)
        {
            string query = @"
                SELECT TOP 1 *
                FROM [Order]
                WHERE table_id = @TableId AND is_done = 0
                ORDER BY created_at DESC";
            
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@TableId", tableId)
            };
            
            DataTable dataTable = ExecuteSelectQuery(query, parameters);
            
            if (dataTable.Rows.Count == 0)
                return null;
                
            return ReadOrder(dataTable.Rows[0]);
        }
        
        public List<OrderItem> GetOrderItemsByOrderId(int orderId)
        {
            string query = @"
                SELECT oi.*, mi.name, mi.price, mi.description, mi.stock, mi.vat_percentage, mi.is_active
                FROM Order_Item oi
                JOIN Menu_Item mi ON oi.menu_item_id = mi.menu_item_id
                WHERE oi.order_id = @OrderId
                ORDER BY oi.created_at";
            
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@OrderId", orderId)
            };
            
            DataTable dataTable = ExecuteSelectQuery(query, parameters);
            
            List<OrderItem> items = new List<OrderItem>();
            
            foreach (DataRow dr in dataTable.Rows)
            {
                MenuItem menuItem = new MenuItem
                {
                    MenuItemId = (int)dr["menu_item_id"],
                    Name = (string)dr["name"],
                    Price = (decimal)dr["price"],
                    Description = dr["description"] != DBNull.Value ? (string)dr["description"] : string.Empty,
                    Stock = (int)dr["stock"],
                    VatPercentage = (int)dr["vat_percentage"],
                    IsActive = (bool)dr["is_active"]
                };
                
                OrderItem item = new OrderItem
                {
                    OrderItemId = (int)dr["order_item_id"],
                    OrderId = new Order { OrderId = (int)dr["order_id"] },
                    MenuItemId = menuItem, // Use the fully populated MenuItem
                    Quantity = (int)dr["quantity"],
                    Comment = dr["comment"] != DBNull.Value ? (string)dr["comment"] : string.Empty,
                    CreatedAt = (DateTime)dr["created_at"],
                    Status = Enum.TryParse<OrderItem.OrderStatus>(dr["status"].ToString(), true, out OrderItem.OrderStatus status) 
                            ? status : OrderItem.OrderStatus.Ordered
                };
                
                items.Add(item);
            }
            
            return items;
        }
        
        private Order ReadOrder(DataRow dr)
        {
            int employeeId = (int)dr["employee_id"];
            
            // Create a basic Order object
            Order order = new Order
            {
                OrderId = (int)dr["order_id"],
                TableId = new Table { TableId = (int)dr["table_id"] },
                // Just provide a reference to the ID, we'll load the rest later if needed
                EmployeeId = new Employee { 
                    EmployeeId = employeeId,
                    Username = "temp",  // Required fields with temporary values
                    PasswordHash = "temp", 
                    FirstName = "temp", 
                    LastName = "temp", 
                    Email = "temp@example.com" 
                },
                CreatedAt = (DateTime)dr["created_at"],
                IsDone = (bool)dr["is_done"]
            };
            
            return order;
        }

        public void MarkAllItemsAsServed(int orderId)
        {
            string query = @"
                UPDATE Order_Item 
                SET status = 'Served' 
                WHERE order_id = @OrderId";
            
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@OrderId", orderId)
            };
            
            ExecuteEditQuery(query, parameters);
        }

        public void UpdateOrderItem(int orderItemId, int quantity, string comment)
        {
            string query = @"
                UPDATE Order_Item 
                SET quantity = @Quantity, comment = @Comment
                WHERE order_item_id = @OrderItemId";
            
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@OrderItemId", orderItemId),
                new SqlParameter("@Quantity", quantity),
                new SqlParameter("@Comment", string.IsNullOrEmpty(comment) ? (object)DBNull.Value : comment)
            };
            
            ExecuteEditQuery(query, parameters);
        }

        public void DeleteOrderItem(int orderItemId)
        {
            string query = @"
                DELETE FROM Order_Item 
                WHERE order_item_id = @OrderItemId";
            
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@OrderItemId", orderItemId)
            };
            
            ExecuteEditQuery(query, parameters);
        }
    }
}