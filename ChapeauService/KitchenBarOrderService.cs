using ChapeauDAL;
using ChapeauModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static ChapeauModel.OrderItem;

namespace ChapeauService
{
    public class KitchenBarOrderService : IKitchenBarService
    {
        private readonly IKitchenBarDao orderDao;

        public KitchenBarOrderService()
        {
            orderDao = new OrderDao();
        }

        #region Core Order Methods


        public IEnumerable<Order> GetActiveOrdersForRole(bool isKitchen)
        {
            var allOrders = orderDao.GetActiveOrders();
            return FilterOrdersByRole(allOrders, isKitchen);
        }

        public IEnumerable<Order> GetCompletedOrdersForRole(bool isKitchen)
        {
            var allOrders = orderDao.GetCompletedOrders();
            return FilterOrdersByRole(allOrders, isKitchen);
        }

        public IEnumerable<OrderItem> GetOrderItemsForPreparation(bool isKitchen)
        {
            if (isKitchen)
            {
                return orderDao.GetOrderItemsForKitchen();
            }
            else
            {
                return orderDao.GetOrderItemsForBar();
            }
        }

        #endregion

        #region Order Item Operations


        public bool UpdateOrderItemStatus(int orderItemId, OrderStatus newStatus)
        {
            try
            {
                var orderItem = orderDao.GetOrderItemByOrderItemId(orderItemId);
                if (orderItem == null)
                    return false;

                // Validate status transition
                if (!IsValidStatusTransition(orderItem.Status, newStatus))
                    return false;

                orderItem.Status = newStatus;
                return orderDao.UpdateOrderItemStatus(orderItem);
            }
            catch
            {
                return false;
            }
        }


        public bool MarkMultipleItemsStatus(IEnumerable<int> orderItemIds, OrderStatus newStatus)
        {
            try
            {
                foreach (var orderItemId in orderItemIds)
                {
                    UpdateOrderItemStatus(orderItemId, newStatus);
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        #endregion

        #region Helper Methods

        public IEnumerable<OrderItem> GetRelevantOrderItems(Order order, bool isKitchen)
        {
            if (order?.OrderItems == null)
                return Enumerable.Empty<OrderItem>();

            return order.OrderItems.Where(item => IsRelevantItem(item, isKitchen));
        }


        public bool IsRelevantItem(OrderItem item, bool isKitchen)
        {
            if (item?.MenuItemId?.CategoryId == null)
                return false;

            var menuCard = item.MenuItemId.CategoryId.MenuCard;

            if (isKitchen)
            {
                return menuCard == MenuCard.Food;
            }
            else
            {
                return menuCard == MenuCard.Drinks;
            }
        }


        public bool IsValidStatusTransition(OrderStatus currentStatus, OrderStatus newStatus)
        {
            switch (currentStatus)
            {
                case OrderStatus.Ordered:
                    return newStatus == OrderStatus.BeingPrepared;

                case OrderStatus.BeingPrepared:
                    return newStatus == OrderStatus.ReadyToBeServed;

                case OrderStatus.ReadyToBeServed:
                    return newStatus == OrderStatus.Served;

                default:
                    return false;
            }
        }

        #endregion

        #region Private Helper Methods


        private IEnumerable<Order> FilterOrdersByRole(IEnumerable<Order> orders, bool isKitchen)
        {
            return orders.Where(order =>
                order.OrderItems != null &&
                order.OrderItems.Any(item => IsRelevantItem(item, isKitchen)));
        }

        #endregion
    }
}