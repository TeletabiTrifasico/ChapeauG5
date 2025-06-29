using ChapeauDAL;
using ChapeauModel;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChapeauG5.ChapeauDAL
{
    public class OrderItemDao : BaseDao
    {

        public void UpdateOrderItemStatus(int orderItemId, OrderItem.OrderStatus newStatus)
        {
            string query = "UPDATE Order_Item SET status = @Status WHERE order_item_id = @OrderItemId";
            SqlParameter[] parameters = {
        new SqlParameter("@Status", newStatus.ToString()),
        new SqlParameter("@OrderItemId", orderItemId)
            };
            ExecuteEditQuery(query, parameters);
        }

        public List<OrderItem> GetReadyToBeServedItemsByTable(int tableId)
        {
            string query = @"
        SELECT oi.order_item_id, oi.order_id, oi.menu_item_id, mi.name, oi.quantity, oi.comment, oi.created_at, oi.status
        FROM Order_Item oi
        INNER JOIN [Order] o ON oi.order_id = o.order_id
        INNER JOIN Menu_Item mi ON oi.menu_item_id = mi.menu_item_id
        WHERE o.table_id = @TableId AND oi.status = @Status";
            SqlParameter[] parameters = {
        new SqlParameter("@TableId", tableId),
        new SqlParameter("@Status", OrderItem.OrderStatus.ReadyToBeServed.ToString())};
            DataTable table = ExecuteSelectQuery(query, parameters);
            return MapOrderItemsFromTable(table);
        }

        private List<OrderItem> MapOrderItemsFromTable(DataTable table)
        {
            List<OrderItem> items = new List<OrderItem>();
            foreach (DataRow row in table.Rows)
            {
                items.Add(MapOrderItemFromDataRow(row));
            }
            return items;
        }

        private OrderItem MapOrderItemFromDataRow(DataRow row)
        {
            return new OrderItem
            {
                OrderItemId = (int)row["order_item_id"],
                OrderId = new Order { OrderId = (int)row["order_id"] },
                MenuItemId = new MenuItem
                {
                    MenuItemId = (int)row["menu_item_id"],
                    Name = row["name"].ToString()
                },
                Quantity = (int)row["quantity"],
                Comment = row["comment"] != DBNull.Value ? row["comment"].ToString() : string.Empty,
                CreatedAt = (DateTime)row["created_at"],
                Status = Enum.Parse<OrderItem.OrderStatus>(row["status"].ToString())
            };
        }

    }
}
