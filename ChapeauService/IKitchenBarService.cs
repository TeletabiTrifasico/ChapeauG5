using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChapeauModel;
using static ChapeauModel.OrderItem;

namespace ChapeauService
{
    public interface IKitchenBarService
    {
        #region Core Order Methods


        IEnumerable<Order> GetActiveOrdersForRole(bool isKitchen);

        IEnumerable<Order> GetCompletedOrdersForRole(bool isKitchen);

        IEnumerable<OrderItem> GetOrderItemsForPreparation(bool isKitchen);

        #endregion

        #region Order Item Operations


        bool UpdateOrderItemStatus(int orderItemId, OrderStatus newStatus);


        bool MarkMultipleItemsStatus(IEnumerable<int> orderItemIds, OrderStatus newStatus);

        #endregion

        #region Helper Methods


        IEnumerable<OrderItem> GetRelevantOrderItems(Order order, bool isKitchen);


        bool IsRelevantItem(OrderItem item, bool isKitchen);


        bool IsValidStatusTransition(OrderStatus currentStatus, OrderStatus newStatus);

        #endregion
    }
}