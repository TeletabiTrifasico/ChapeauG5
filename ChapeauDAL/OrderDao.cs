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
                new SqlParameter("@TableId", order.TableId != null ? order.TableId.TableId : (object)DBNull.Value),
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

        public Order GetOrderWithItemsById(int orderId)
        {
            string query = @"
                SELECT o.*, t.table_number
                FROM [Order] o
                JOIN [Table] t ON o.table_id = t.table_id
                WHERE o.order_id = @OrderId";

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@OrderId", orderId)
            };
            DataTable dataTable = ExecuteSelectQuery(query, parameters);
            if (dataTable == null || dataTable.Rows.Count == 0)
                return null;
                
            Order order = ReadOrder(dataTable.Rows[0]);
            if (order.TableId != null)
            {
                order.TableId.TableNumber = (int)dataTable.Rows[0]["table_number"];
            }
            order.OrderItems = GetOrderItemsByOrderId(orderId);
            return order;
        }

        public List<OrderItem> GetOrderItemsByOrderId(int orderId)
        {
            string query = @"
                SELECT oi.*, mi.*, m.menu_card, m.category_id, m.name as category_name,
                       CASE WHEN d.drink_item_id IS NOT NULL THEN 1 ELSE 0 END as is_drink,
                       d.is_alcoholic
                FROM Order_Item oi
                JOIN Menu_Item mi ON oi.menu_item_id = mi.menu_item_id
                JOIN Menu m ON mi.category_id = m.category_id
                LEFT JOIN Drink_Item d ON mi.menu_item_id = d.menu_item_id
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
                    Description = dr["description"] != DBNull.Value ? (string)dr["description"] : string.Empty,
                    Price = (decimal)dr["price"],
                    Stock = (int)dr["stock"],
                    CategoryId = new Menu 
                    { 
                        CategoryId = (int)dr["category_id"],
                        CategoryName = (string)dr["category_name"],
                        MenuCard = ParseMenuCard((string)dr["menu_card"])
                    },
                    VatPercentage = (int)dr["vat_percentage"],
                    IsActive = (bool)dr["is_active"],
                    CourseType = dr["course_type"] != DBNull.Value ? ParseCourseType((string)dr["course_type"]) : CourseType.Main,
                    IsAlcoholic = (bool)(dr["is_drink"] != DBNull.Value && (int)dr["is_drink"] == 1 ? dr["is_alcoholic"] : false)
                };

                OrderItem item = new OrderItem
                {
                    OrderItemId = (int)dr["order_item_id"],
                    OrderId = new Order { OrderId = (int)dr["order_id"] },
                    MenuItemId = menuItem,
                    Quantity = (int)dr["quantity"],
                    Comment = dr["comment"] != DBNull.Value ? (string)dr["comment"] : string.Empty,
                    CreatedAt = (DateTime)dr["created_at"],
                    Status = ParseOrderStatus((string)dr["status"])
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
                EmployeeId = new Employee
                {
                    EmployeeId = employeeId,
                    Username = "temp",
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

        public List<TableOrderStatus> GetTableOrderStatuses()
        {
            string query = @"
            SELECT 
                t.table_id, 
                t.table_number,
                CASE WHEN o.order_id IS NOT NULL THEN 1 ELSE 0 END AS has_running_order,
                MAX(CASE WHEN m.menu_card IN ('Lunch', 'Dinner') THEN oi.status END) AS food_status,
                MAX(CASE WHEN m.menu_card = 'Drinks' THEN oi.status END) AS drink_status
            FROM [Table] t
            LEFT JOIN [Order] o ON t.table_id = o.table_id AND o.is_done = 0
            LEFT JOIN Order_Item oi ON o.order_id = oi.order_id
            LEFT JOIN Menu_Item mi ON oi.menu_item_id = mi.menu_item_id
            LEFT JOIN Menu m ON mi.category_id = m.category_id
            GROUP BY t.table_id, t.table_number, o.order_id
            ORDER BY t.table_number";

            DataTable dt = ExecuteSelectQuery(query, null);

            List<TableOrderStatus> result = new List<TableOrderStatus>();
            foreach (DataRow dr in dt.Rows)
            {
                result.Add(new TableOrderStatus
                {
                    TableId = (int)dr["table_id"],
                    TableNumber = (int)dr["table_number"],
                    HasRunningOrder = (int)dr["has_running_order"] == 1,
                    FoodOrderStatus = dr["food_status"] != DBNull.Value ? dr["food_status"].ToString() : null,
                    DrinkOrderStatus = dr["drink_status"] != DBNull.Value ? dr["drink_status"].ToString() : null
                });
            }
            return result;
        }

        public bool HasActiveOrder(int tableId)
        {
            string query = @"
                SELECT COUNT(*) 
                FROM [Order] 
                WHERE table_id = @TableId AND is_done = 0";
            SqlParameter[] parameters = {
            new SqlParameter("@TableId", tableId)
            };

            object result = ExecuteScalar(query, parameters);
            int count = Convert.ToInt32(result);

            return count > 0;
        }

        private object ExecuteScalar(string query, SqlParameter[] parameters)
        {
            SqlConnection connection = new SqlConnection(connectionString);
            SqlCommand command = new SqlCommand(query, connection);

            if (parameters != null)
            {
                command.Parameters.AddRange(parameters);
            }

            try
            {
                connection.Open();
                object result = command.ExecuteScalar();
                return result;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                connection.Close();
            }
        }

        public void MarkOrderAsDone(int orderId)
        {
            string query = "UPDATE [Order] SET is_done = 1 WHERE order_id = @OrderId";
            
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@OrderId", orderId)
            };
            
            ExecuteEditQuery(query, parameters);
        }

        public void MarkOrderItemAsServed(int orderItemId)
        {
            string query = @"
                UPDATE Order_Item 
                SET status = 'Served' 
                WHERE order_item_id = @OrderItemId";

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@OrderItemId", orderItemId)
            };

            ExecuteEditQuery(query, parameters);
        }

        private CourseType ParseCourseType(string courseType)
        {
            if (string.IsNullOrEmpty(courseType) || 
                !Enum.TryParse<CourseType>(courseType, true, out CourseType result))
            {
                return CourseType.Main;
            }
            
            return result;
        }

        private OrderItem.OrderStatus ParseOrderStatus(string status)
        {
            if (string.IsNullOrEmpty(status) || 
                !Enum.TryParse<OrderItem.OrderStatus>(status, true, out OrderItem.OrderStatus result))
            {
                return OrderItem.OrderStatus.Ordered;
            }
            
            return result;
        }

        private MenuCard ParseMenuCard(string menuCard)
        {
            if (string.IsNullOrEmpty(menuCard) || 
                !Enum.TryParse<MenuCard>(menuCard, true, out MenuCard result))
            {
                return MenuCard.Food;
            }
            
            return result;
        }

    }
}