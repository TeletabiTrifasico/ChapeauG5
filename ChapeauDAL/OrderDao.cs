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
                new SqlParameter("@TableId", order.TableId),
                new SqlParameter("@EmployeeId", order.EmployeeId),
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
                new SqlParameter("@OrderId", item.OrderId),
                new SqlParameter("@MenuItemId", item.MenuItemId),
                new SqlParameter("@Quantity", item.Quantity),
                new SqlParameter("@Comment", string.IsNullOrEmpty(item.Comment) ? (object)DBNull.Value : item.Comment),
                new SqlParameter("@CreatedAt", item.CreatedAt),
                new SqlParameter("@Status", item.Status)
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
                SELECT oi.*, mi.name, mi.price
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
                OrderItem item = new OrderItem
                {
                    OrderItemId = (int)dr["order_item_id"],
                    OrderId = (int)dr["order_id"],
                    MenuItemId = (int)dr["menu_item_id"],
                    Quantity = (int)dr["quantity"],
                    Comment = dr["comment"] != DBNull.Value ? (string)dr["comment"] : string.Empty,
                    CreatedAt = (DateTime)dr["created_at"],
                    Status = (string)dr["status"],
                    ItemName = (string)dr["name"],
                    ItemPrice = (decimal)dr["price"]
                };
                
                items.Add(item);
            }
            
            return items;
        }
        
        private Order ReadOrder(DataRow dr)
        {
            return new Order
            {
                OrderId = (int)dr["order_id"],
                TableId = (int)dr["table_id"],
                EmployeeId = (int)dr["employee_id"],
                CreatedAt = (DateTime)dr["created_at"],
                IsDone = (bool)dr["is_done"]
            };
        }

        // Add this method to mark all items as served (Temporary when clicking pay button) (TESTING PURPOSES)
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
    }
}