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
using static ChapeauModel.OrderItem;

namespace ChapeauG5.ChapeauUI
{
    public partial class KitchenBarOrderTables : UserControl
    {
        private Order _order;
        private bool _isKitchen;

        #region Properties

        public Order Order
        {
            get => _order;
            set
            {
                _order = value;
                RefreshDisplay();
            }
        }

        public bool IsKitchen
        {
            get => _isKitchen;
            set
            {
                _isKitchen = value;
                RefreshDisplay();
            }
        }

        #endregion

        #region Events

        public event EventHandler TableSelected;

        #endregion

        #region Constructor

        public KitchenBarOrderTables()
        {
            InitializeComponent();
            InitializeControl();
        }

        private void InitializeControl()
        {
            this.Cursor = Cursors.Hand;
            this.BorderStyle = BorderStyle.FixedSingle;
            this.Margin = new Padding(5);

            // Add hover effects and click events
            this.MouseEnter += OnMouseEnter;
            this.MouseLeave += OnMouseLeave;
            this.Click += OnControlClick;

            // Make all child controls clickable too
            foreach (Control control in this.Controls)
            {
                control.MouseEnter += OnMouseEnter;
                control.MouseLeave += OnMouseLeave;
                control.Click += OnControlClick;
                control.Cursor = Cursors.Hand;
            }
        }

        #endregion

        #region Display Methods

        private void RefreshDisplay()
        {
            if (_order == null)
            {
                OrderTableNumberLabel.Text = "No Table";
                totalOrderItemsCountLabel.Text = "0";
                OrderedItemStatusStatsLabel.Text = "No items";
                this.BackColor = SystemColors.ControlLight;
                return;
            }

            OrderTableNumberLabel.Text = $"Table {_order.TableId?.TableNumber ?? 0}";

            var relevantItems = GetRelevantOrderItems();
            totalOrderItemsCountLabel.Text = relevantItems.Count().ToString();

            var ordered = relevantItems.Count(i => i.Status == OrderStatus.Ordered);
            var preparing = relevantItems.Count(i => i.Status == OrderStatus.BeingPrepared);
            var ready = relevantItems.Count(i => i.Status == OrderStatus.ReadyToBeServed);

            OrderedItemStatusStatsLabel.Text = $"Ordered: {ordered} Preparing: {preparing} Ready: {ready}";

            UpdateVisualIndicators(relevantItems.ToList());
        }

        private IEnumerable<OrderItem> GetRelevantOrderItems()
        {
            if (_order?.OrderItems == null)
                return Enumerable.Empty<OrderItem>();

            return _order.OrderItems.Where(item => IsRelevantItem(item));
        }

        private bool IsRelevantItem(OrderItem item)
        {
            if (item?.MenuItemId?.CategoryId == null)
                return false;

            var menuCard = item.MenuItemId.CategoryId.MenuCard;

            if (_isKitchen)
            {
                return menuCard == MenuCard.Food;
            }
            else
            {
                return menuCard == MenuCard.Drinks;
            }
        }

        private void UpdateVisualIndicators(List<OrderItem> relevantItems)
        {
            if (!relevantItems.Any())
            {
                this.BackColor = SystemColors.ControlLight;
                return;
            }

            // Check for urgent items (waiting > 15 minutes)
            var hasUrgentItems = relevantItems.Any(item => item.WaitingMinutes > 15);

            // Check if all items are ready
            var allItemsReady = relevantItems.All(item => item.Status == OrderStatus.ReadyToBeServed);

            if (allItemsReady)
            {
                this.BackColor = Color.FromArgb(220, 255, 220); // Light green - ready to serve
            }
            else if (hasUrgentItems)
            {
                this.BackColor = Color.FromArgb(255, 240, 240); // Light red - urgent
            }
            else
            {
                this.BackColor = SystemColors.ControlLight; // Default
            }
        }

        #endregion

        #region Event Handlers

        private void OnMouseEnter(object sender, EventArgs e)
        {
            if (this.BackColor == SystemColors.ControlLight)
            {
                this.BackColor = Color.LightBlue;
            }
        }

        private void OnMouseLeave(object sender, EventArgs e)
        {
            RefreshDisplay(); 
        }

        private void OnControlClick(object sender, EventArgs e)
        {
            if (_order != null)
            {
                TableSelected?.Invoke(this, EventArgs.Empty);
            }
        }

        private void KitchenOrderTables_Load(object sender, EventArgs e)
        {
            
        }

        #endregion


        //count of relevant items for this kitchen/bar role
        public int GetRelevantItemCount()
        {
            return GetRelevantOrderItems().Count();
        }


        /// Checks if this table has urgent items
        public bool HasUrgentItems()
        {
            return GetRelevantOrderItems().Any(item => item.WaitingMinutes > 15);
        }

        /// Checks if all relevant items are ready
        public bool AllItemsReady()
        {
            var items = GetRelevantOrderItems().ToList();
            return items.Any() && items.All(item => item.Status == OrderStatus.ReadyToBeServed);
        }

       
    }
}