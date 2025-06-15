using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChapeauDAL;
using ChapeauModel;
using System.Data.SqlClient;
using Microsoft.Data.SqlClient;

namespace ChapeauDAL
{
    public class KitchenBarOrderDao : BaseDao
    {
        #region Orders
        public async Task<IEnumerable<Order>> GetActiveOrdersAsync()
        {
            string query = @"
                SELECT o.*, e.first_name, e.last_name, e.role,
                       t.table_number, t.capacity
                FROM [ORDER] o
                INNER JOIN EMPLOYEE e ON o.employee_id = e.employee_id
                INNER JOIN [TABLE] t ON o.table_id = t.table_id
                WHERE o.is_done = 0
                ORDER BY o.date_created_at DESC";

            var orders = await ExecuteQueryAsync(query, MapOrderWithRelations);

            foreach (var order in orders)
            {
                order.OrderItems = (await GetOrderItemsByOrderIdAsync(order.OrderId)).ToList();
            }

            return orders;
        }

        public async Task<IEnumerable<Order>> GetCompletedOrdersAsync()
        {
            string query = @"
                SELECT o.*, e.first_name, e.last_name, e.role,
                       t.table_number, t.capacity
                FROM [ORDER] o
                INNER JOIN EMPLOYEE e ON o.employee_id = e.employee_id
                INNER JOIN [TABLE] t ON o.table_id = t.table_id
                WHERE o.is_done = 1
                ORDER BY o.date_created_at DESC";

            var orders = await ExecuteQueryAsync(query, MapOrderWithRelations);

            foreach (var order in orders)
            {
                order.OrderItems = (await GetOrderItemsByOrderIdAsync(order.OrderId)).ToList();
            }

            return orders;
        }

        public async Task<IEnumerable<Order>> GetActiveOrdersForTableAsync(int tableId)
        {
            string query = @"
                SELECT o.*, e.first_name, e.last_name, e.role,
                       t.table_number, t.capacity
                FROM [ORDER] o
                INNER JOIN EMPLOYEE e ON o.employee_id = e.employee_id
                INNER JOIN [TABLE] t ON o.table_id = t.table_id
                WHERE o.table_id = @TableId AND o.is_done = 0
                ORDER BY o.date_created_at DESC";

            var parameters = new SqlParameter[]
            {
                CreateParameter("@TableId", tableId)
            };

            var orders = await ExecuteQueryAsync(query, MapOrderWithRelations, parameters);

            foreach (var order in orders)
            {
                order.OrderItems = (await GetOrderItemsByOrderIdAsync(order.OrderId)).ToList();
            }

            return orders;
        }
        #endregion

        #region Order Items
        public async Task<OrderItem> GetOrderItemByIdAsync(int orderItemId)
        {
            string query = @"
                SELECT oi.*, mi.name, mi.price, mi.vat_percentage, mi.course_type
                FROM ORDER_ITEM oi
                INNER JOIN MENU_ITEM mi ON oi.menu_item_id = mi.menu_item_id
                WHERE oi.order_item_id = @OrderItemId";

            var parameters = new SqlParameter[]
            {
                CreateParameter("@OrderItemId", orderItemId)
            };

            return await ExecuteQuerySingleAsync(query, MapOrderItemWithMenuItem, parameters);
        }

        public async Task<IEnumerable<OrderItem>> GetOrderItemsByOrderIdAsync(int orderId)
        {
            string query = @"
                SELECT oi.*, mi.name, mi.price, mi.vat_percentage, mi.course_type,
                       mc.name as CategoryName, mc.menu_card
                FROM ORDER_ITEM oi
                INNER JOIN MENU_ITEM mi ON oi.menu_item_id = mi.menu_item_id
                INNER JOIN MENU mc ON mi.category_id = mc.category_id
                WHERE oi.order_id = @OrderId
                ORDER BY oi.date_created_at";

            var parameters = new SqlParameter[]
            {
                CreateParameter("@OrderId", orderId)
            };

            return await ExecuteQueryAsync(query, MapOrderItemWithFullDetails, parameters);
        }

        public async Task<bool> UpdateOrderItemAsync(OrderItem orderItem)
        {
            string query = @"
                UPDATE ORDER_ITEM 
                SET quantity = @Quantity,
                    comment = @Comment,
                    status = @Status
                WHERE order_item_id = @OrderItemId";

            var parameters = new SqlParameter[]
            {
                CreateParameter("@OrderItemId", orderItem.OrderItemId),
                CreateParameter("@Quantity", orderItem.Quantity),
                CreateParameter("@Comment", orderItem.Comment),
                CreateParameter("@Status", orderItem.Status.ToString())
            };

            var rowsAffected = await ExecuteNonQueryAsync(query, parameters);
            return rowsAffected > 0;
        }

        public async Task<IEnumerable<OrderItem>> GetOrderItemsForKitchenAsync()
        {
            string query = @"
                SELECT oi.*, mi.name, mi.price, mi.vat_percentage, mi.course_type,
                       mc.name as CategoryName, mc.menu_card,
                       o.table_id, t.table_number, o.date_created_at
                FROM ORDER_ITEM oi
                INNER JOIN MENU_ITEM mi ON oi.menu_item_id = mi.menu_item_id
                INNER JOIN MENU mc ON mi.category_id = mc.category_id
                INNER JOIN [ORDER] o ON oi.order_id = o.order_id
                INNER JOIN [TABLE] t ON o.table_id = t.table_id
                WHERE mc.menu_card IN ('Lunch', 'Dinner') 
                  AND oi.status IN ('Ordered', 'BeingPrepared')
                  AND o.is_done = 0
                ORDER BY oi.date_created_at";

            return await ExecuteQueryAsync(query, MapOrderItemWithFullDetailsAndOrder);
        }

        public async Task<IEnumerable<OrderItem>> GetOrderItemsForBarAsync()
        {
            string query = @"
                SELECT oi.*, mi.name, mi.price, mi.vat_percentage, mi.course_type,
                       mc.name as CategoryName, mc.menu_card,
                       o.table_id, t.table_number, o.date_created_at
                FROM ORDER_ITEM oi
                INNER JOIN MENU_ITEM mi ON oi.menu_item_id = mi.menu_item_id
                INNER JOIN MENU mc ON mi.category_id = mc.category_id
                INNER JOIN [ORDER] o ON oi.order_id = o.order_id
                INNER JOIN [TABLE] t ON o.table_id = t.table_id
                WHERE mc.menu_card = 'Drinks' 
                  AND oi.status IN ('Ordered', 'BeingPrepared')
                  AND o.is_done = 0
                ORDER BY oi.date_created_at";

            return await ExecuteQueryAsync(query, MapOrderItemWithFullDetailsAndOrder);
        }
        #endregion

        #region Mapping Methods
        private Order MapOrder(SqlDataReader reader)
        {
            return new Order
            {
                OrderId = reader.GetInt32(reader.GetOrdinal("order_id")),
                TableId = new Table { TableId = reader.GetInt32(reader.GetOrdinal("table_id")) },
                EmployeeId = new Employee
                {
                    EmployeeId = reader.GetInt32(reader.GetOrdinal("employee_id")),
                    Username = "temp",
                    PasswordHash = "temp",
                    FirstName = "temp",
                    LastName = "temp",
                    Email = "temp"
                },
                CreatedAt = reader.GetDateTime(reader.GetOrdinal("date_created_at")),
                IsDone = reader.GetBoolean(reader.GetOrdinal("is_done"))
            };
        }

        private Order MapOrderWithRelations(SqlDataReader reader)
        {
            var order = MapOrder(reader);

            // Map Employee if available
            if (!reader.IsDBNull(reader.GetOrdinal("first_name")))
            {
                order.EmployeeId = new Employee
                {
                    EmployeeId = order.EmployeeId.EmployeeId,
                    Username = "temp", // Required field
                    PasswordHash = "temp", // Required field
                    FirstName = reader.GetString(reader.GetOrdinal("first_name")),
                    LastName = reader.GetString(reader.GetOrdinal("last_name")),
                    Email = "temp", // Required field
                    Role = Enum.Parse<EmployeeRole>(reader.GetString(reader.GetOrdinal("role")))
                };
            }

            // Map Table if available
            if (!reader.IsDBNull(reader.GetOrdinal("table_number")))
            {
                order.TableId = new Table
                {
                    TableId = order.TableId.TableId,
                    TableNumber = reader.GetInt32(reader.GetOrdinal("table_number")),
                    Capacity = reader.GetInt32(reader.GetOrdinal("capacity"))
                };
            }

            // Initialize OrderItems list
            order.OrderItems = new List<OrderItem>();

            return order;
        }

        private OrderItem MapOrderItem(SqlDataReader reader)
        {
            return new OrderItem
            {
                OrderItemId = reader.GetInt32(reader.GetOrdinal("order_item_id")),
                OrderId = new Order { OrderId = reader.GetInt32(reader.GetOrdinal("order_id")) },
                MenuItemId = new MenuItem { MenuItemId = reader.GetInt32(reader.GetOrdinal("menu_item_id")) },
                Quantity = reader.GetInt32(reader.GetOrdinal("quantity")),
                Comment = reader.IsDBNull(reader.GetOrdinal("comment")) ? null : reader.GetString(reader.GetOrdinal("comment")),
                CreatedAt = reader.GetDateTime(reader.GetOrdinal("date_created_at")),
                Status = Enum.Parse<OrderStatus>(reader.GetString(reader.GetOrdinal("status")))
            };
        }

        private OrderItem MapOrderItemWithMenuItem(SqlDataReader reader)
        {
            var orderItem = MapOrderItem(reader);

            orderItem.MenuItemId = new MenuItem
            {
                MenuItemId = orderItem.MenuItemId.MenuItemId,
                Name = reader.GetString(reader.GetOrdinal("name")),
                Price = reader.GetDecimal(reader.GetOrdinal("price")),
                VatPercentage = reader.GetInt32(reader.GetOrdinal("vat_percentage")),
                CourseType = reader.IsDBNull(reader.GetOrdinal("course_type")) ?
                           CourseType.Starter : Enum.Parse<CourseType>(reader.GetString(reader.GetOrdinal("course_type")))
            };

            return orderItem;
        }

        private OrderItem MapOrderItemWithFullDetails(SqlDataReader reader)
        {
            var orderItem = MapOrderItemWithMenuItem(reader);

            // Create a menu category for the menu item
            orderItem.MenuItemId.CategoryId = new Menu
            {
                CategoryId = 0, // Not retrieved in this query
                CategoryName = reader.GetString(reader.GetOrdinal("CategoryName")),
                MenuCard = Enum.Parse<MenuCard>(reader.GetString(reader.GetOrdinal("menu_card")))

            };

            return orderItem;
        }

        private OrderItem MapOrderItemWithFullDetailsAndOrder(SqlDataReader reader)
        {
            var orderItem = MapOrderItemWithFullDetails(reader);

            // Add Order information minimal
            orderItem.OrderId = new Order
            {
                OrderId = orderItem.OrderId.OrderId,
                TableId = new Table
                {
                    TableId = reader.GetInt32(reader.GetOrdinal("table_id")),
                    TableNumber = reader.GetInt32(reader.GetOrdinal("table_number"))
                },
                CreatedAt = reader.GetDateTime(reader.GetOrdinal("date_created_at"))
            };

            return orderItem;
        }
        #endregion
    }
}