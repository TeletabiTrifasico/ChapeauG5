using System.Collections.Generic;
using ChapeauG5.ChapeauDAL;
using ChapeauModel;

namespace ChapeauService
{
    public class OrderItemService
    {
        private readonly OrderItemDao orderItemDao;

        public OrderItemService()
        {
            orderItemDao = new OrderItemDao();
        }

        public void UpdateOrderItemStatus(int orderItemId, OrderItem.OrderStatus newStatus)
        {
            // Update the order item status in the database
            orderItemDao.UpdateOrderItemStatus(orderItemId, newStatus);
        }

        public List<OrderItem> GetReadyToBeServedItemsByTable(int tableId)
        {
            return orderItemDao.GetReadyToBeServedItemsByTable(tableId);
        }


    }
}