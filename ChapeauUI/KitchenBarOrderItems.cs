using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ChapeauModel;
using ChapeauService;
using static ChapeauModel.OrderItem;

namespace ChapeauG5.ChapeauUI
{
    public partial class KitchenBarOrderItems : UserControl
    {
        private OrderItem _orderItem;
        private IKitchenBarService _orderService;
        private bool _isProcessing;

        #region Properties

        public OrderItem OrderItem
        {
            get => _orderItem;
            set
            {
                _orderItem = value;
                RefreshDisplay();
            }
        }

        public IKitchenBarService OrderService
        {
            get => _orderService;
            set => _orderService = value;
        }

        #endregion

        #region Events

        public event EventHandler StatusChanged;
        public event EventHandler<string> ActionFailed;

        #endregion

        #region Constructor

        public KitchenBarOrderItems()
        {
            InitializeComponent();
            this.BorderStyle = BorderStyle.FixedSingle;
            this.Margin = new Padding(5);
        }

        #endregion

        #region Display Methods

        private void RefreshDisplay()
        {
            if (_orderItem == null)
            {
                ResetDisplay();
                return;
            }

            UpdateLabels();
            UpdateButtonStates();
            UpdateVisualIndicators();
        }

        private void ResetDisplay()
        {
            orderItemName.Text = "No Item";
            orderItemQuantity.Text = "0";
            orderItemComment.Text = "";
            orderItemStatus.Text = "Unknown";
            btnStartOrderItem.Enabled = false;
            btnReadyOrderItem.Enabled = false;
            UpdateButtonColors();
            this.BackColor = SystemColors.ControlLight;
        }

        private void UpdateLabels()
        {
            orderItemName.Text = _orderItem.MenuItemId?.Name ?? "Unknown Item";
            orderItemQuantity.Text = _orderItem.Quantity.ToString();
            orderItemComment.Text = _orderItem.Comment ?? "";
            orderItemStatus.Text = _orderItem.Status.ToString();
        }

        private void UpdateButtonStates()
        {
            if (_isProcessing)
            {
                btnStartOrderItem.Enabled = false;
                btnReadyOrderItem.Enabled = false;
                btnStartOrderItem.Text = "Processing...";
                btnReadyOrderItem.Text = "Processing...";
            }
            else
            {
                var canStart = _orderItem.Status == OrderStatus.Ordered;
                var canReady = _orderItem.Status == OrderStatus.BeingPrepared;

                btnStartOrderItem.Enabled = canStart;
                btnReadyOrderItem.Enabled = canReady;
                btnStartOrderItem.Text = "Start 🔥";
                btnReadyOrderItem.Text = "Ready ✓";
            }

            UpdateButtonColors();
        }

        private void UpdateButtonColors()
        {
            // Start button
            if (btnStartOrderItem.Enabled && !_isProcessing)
            {
                btnStartOrderItem.BackColor = Color.Orange;
                btnStartOrderItem.ForeColor = Color.Black;
            }
            else
            {
                btnStartOrderItem.BackColor = Color.LightGray;
                btnStartOrderItem.ForeColor = Color.DarkGray;
            }

            // Ready button
            if (btnReadyOrderItem.Enabled && !_isProcessing)
            {
                btnReadyOrderItem.BackColor = Color.LightGreen;
                btnReadyOrderItem.ForeColor = Color.Black;
            }
            else
            {
                btnReadyOrderItem.BackColor = Color.LightGray;
                btnReadyOrderItem.ForeColor = Color.DarkGray;
            }
        }

        private void UpdateVisualIndicators()
        {
            if (_orderItem == null)
            {
                this.BackColor = SystemColors.ControlLight;
                return;
            }

            var waitingMinutes = _orderItem.WaitingMinutes ?? 0;

            // Set background color based on status and urgency
            switch (_orderItem.Status)
            {
                case OrderStatus.Ordered:
                    this.BackColor = waitingMinutes > 15 ?
                        Color.FromArgb(255, 230, 230) : // Light red for urgent
                        SystemColors.ControlLight;
                    break;

                case OrderStatus.BeingPrepared:
                    this.BackColor = Color.FromArgb(255, 248, 220); // Light orange/yellow
                    break;

                case OrderStatus.ReadyToBeServed:
                    this.BackColor = Color.FromArgb(220, 255, 220); // Light green
                    break;

                case OrderStatus.Served:
                    this.BackColor = Color.FromArgb(240, 240, 240); // Light gray
                    break;

                default:
                    this.BackColor = SystemColors.ControlLight;
                    break;
            }

            // Add border emphasis for very urgent items
            this.BorderStyle = waitingMinutes > 20 ? BorderStyle.Fixed3D : BorderStyle.FixedSingle;
        }

        #endregion

        #region Event Handlers
        private void KitchenBarOrderItems_Load(object sender, EventArgs e)
        {

        }

        private void btnStartOrderItem_Click(object sender, EventArgs e)
        {
            UpdateOrderItemStatus(OrderStatus.BeingPrepared);
        }

        private void btnReadyOrderItem_Click(object sender, EventArgs e)
        {
            UpdateOrderItemStatus(OrderStatus.ReadyToBeServed);
        }

        private void UpdateOrderItemStatus(OrderStatus newStatus)
        {
            if (_orderItem == null || _orderService == null || _isProcessing) return;

            // Use service level validation instead of local validation
            if (!_orderService.IsValidStatusTransition(_orderItem.Status, newStatus))
            {
                ActionFailed?.Invoke(this, $"Invalid status transition from {_orderItem.Status} to {newStatus}");
                return;
            }

            _isProcessing = true;
            UpdateButtonStates();

            try
            {
                var success = _orderService.UpdateOrderItemStatus(_orderItem.OrderItemId, newStatus);
                if (success)
                {
                    _orderItem.Status = newStatus;
                    RefreshDisplay();
                    StatusChanged?.Invoke(this, EventArgs.Empty);
                }
                else
                {
                    ActionFailed?.Invoke(this, "Failed to update order status. Please try again.");
                }
            }
            catch (Exception ex)
            {
                ActionFailed?.Invoke(this, $"Error updating status: {ex.Message}");
            }
            finally
            {
                _isProcessing = false;
                UpdateButtonStates();
            }
        }

        #endregion

        #region Public Helper Methods (for potential external use)

        // Checks if this item is urgent (waiting > 15 minutes)
        public bool IsUrgent()
        {
            return (_orderItem?.WaitingMinutes ?? 0) > 15;
        }


        // Checks if this item is ready to be served

        public bool IsReadyToServe()
        {
            return _orderItem?.Status == OrderStatus.ReadyToBeServed;
        }


        // Gets the waiting time in minutes
        public int GetWaitingMinutes()
        {
            return _orderItem?.WaitingMinutes ?? 0;
        }


        public void ForceRefresh()
        {
            RefreshDisplay();
        }


        public bool IsRelevantForRole(bool isKitchen)
        {
            if (_orderItem == null || _orderService == null)
                return false;

            return _orderService.IsRelevantItem(_orderItem, isKitchen);
        }


        public MenuCard? GetMenuCard()
        {
            return _orderItem?.MenuItemId?.CategoryId?.MenuCard;
        }


        //  clicking the start button 
        public void TriggerStart()
        {
            if (btnStartOrderItem.Enabled && !_isProcessing)
            {
                UpdateOrderItemStatus(OrderStatus.BeingPrepared);
            }
        }


        //  clicking the ready button
        public void TriggerReady()
        {
            if (btnReadyOrderItem.Enabled && !_isProcessing)
            {
                UpdateOrderItemStatus(OrderStatus.ReadyToBeServed);
            }
        }

        #endregion
    }
}