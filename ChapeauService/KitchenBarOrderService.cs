using ChapeauDAL;
using ChapeauModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChapeauService
{
    public class KitchenBarOrderService
    {
        private KitchenBarOrderDao orderDao;

        public KitchenBarOrderService()
        {
            orderDao = new KitchenBarOrderDao();
        }

        #region Order Management
        public async Task<IEnumerable<Order>> GetActiveOrdersAsync()
        {
            return await orderDao.GetActiveOrdersAsync();
        }

        public async Task<IEnumerable<Order>> GetCompletedOrdersAsync()
        {
            return await orderDao.GetCompletedOrdersAsync();
        }

        public async Task<Order> GetActiveOrderForTableAsync(int tableId)
        {
            var activeOrders = await orderDao.GetActiveOrdersForTableAsync(tableId);
            return activeOrders.FirstOrDefault();
        }
        #endregion

        #region Order Items Status Management
        private bool IsValidStatusTransition(OrderStatus currentStatus, OrderStatus newStatus)
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

        public async Task<bool> UpdateOrderItemStatusAsync(int orderItemId, OrderStatus newStatus)
        {
            try
            {
                var orderItem = await orderDao.GetOrderItemByIdAsync(orderItemId);
                if (orderItem == null)
                    return false;

                // Validate status transition
                if (!IsValidStatusTransition(orderItem.Status, newStatus))
                    return false;

                orderItem.Status = newStatus;
                return await orderDao.UpdateOrderItemAsync(orderItem);
            }
            catch
            {
                return false;
            }
        }
        #endregion

        #region Kitchen/Bar Operations
        public async Task<IEnumerable<OrderItem>> GetKitchenOrdersAsync()
        {
            return await orderDao.GetOrderItemsForKitchenAsync();
        }

        public async Task<IEnumerable<OrderItem>> GetBarOrdersAsync()
        {
            return await orderDao.GetOrderItemsForBarAsync();
        }

        public async Task<bool> MarkItemAsBeingPreparedAsync(int orderItemId)
        {
            return await UpdateOrderItemStatusAsync(orderItemId, OrderStatus.BeingPrepared);
        }

        public async Task<bool> MarkItemAsReadyAsync(int orderItemId)
        {
            return await UpdateOrderItemStatusAsync(orderItemId, OrderStatus.ReadyToBeServed);
        }

        public async Task<bool> MarkItemAsServedAsync(int orderItemId)
        {
            return await UpdateOrderItemStatusAsync(orderItemId, OrderStatus.Served);
        }

        public async Task<bool> MarkMultipleItemsStatusAsync(IEnumerable<int> orderItemIds, OrderStatus newStatus)
        {
            try
            {
                foreach (var orderItemId in orderItemIds)
                {
                    await UpdateOrderItemStatusAsync(orderItemId, newStatus);
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
        #endregion
    }
}