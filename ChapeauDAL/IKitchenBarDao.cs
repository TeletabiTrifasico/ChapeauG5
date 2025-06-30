using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChapeauModel;

namespace ChapeauDAL
{
    public interface IKitchenBarDao
    {
        IEnumerable<Order> GetActiveOrders();
        IEnumerable<Order> GetCompletedOrders();
        IEnumerable<OrderItem> GetOrderItemsForKitchen();
        IEnumerable<OrderItem> GetOrderItemsForBar();
        OrderItem GetOrderItemByOrderItemId(int orderItemId);
        bool UpdateOrderItemStatus(OrderItem orderItem);
    }
}