using ChapeauModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Timer = System.Windows.Forms.Timer;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using ChapeauService;
using static ChapeauModel.OrderItem;

namespace ChapeauG5.ChapeauUI.Forms;

public partial class KitchenBarViewForm : Form
{
    private readonly bool isKitchen;
    private readonly IKitchenBarService orderService;
    private Timer _refreshTimer;
    private CourseType? _activeCourseFilter = null;

    private enum ViewMode { RunningOrders, CompletedOrders, PrepareOrders }
    private ViewMode currentView = ViewMode.RunningOrders;

    public KitchenBarViewForm(bool isKitchenEmployee)
    {
        InitializeComponent();
        isKitchen = isKitchenEmployee;
        orderService = new KitchenBarOrderService();
        InitializeForm();
    }

    #region Initialization

    private void InitializeForm()
    {
        KitchenBarOrdersTitle.Text = isKitchen ? "Kitchen Orders" : "Bar Orders";
        SortBycomboBox.SelectedIndex = -1;
        StatusComboBox.SelectedIndex = -1;
        ShowRunningOrders();
        InitializeTimer();
    }

    private void InitializeTimer()
    {
        _refreshTimer = new Timer();
        _refreshTimer.Interval = 60000; // 60 seconds
        _refreshTimer.Tick += (sender, e) => RefreshData();
        _refreshTimer.Start();
    }

    #endregion

    #region Data Loading Methods

    private void RefreshData()
    {
        try
        {
            switch (currentView)
            {
                case ViewMode.RunningOrders:
                    LoadRunningOrders();
                    break;
                case ViewMode.PrepareOrders:
                    LoadPreparingOrders();
                    break;
                case ViewMode.CompletedOrders:
                    LoadCompletedOrders();
                    break;
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error loading data: {ex.Message}", "Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void LoadRunningOrders()
    {
        var filteredOrders = orderService.GetActiveOrdersForRole(isKitchen).ToList();

        PopulateOrdersView(flowPanelSelectTabelOrdersRunning, filteredOrders);
        SelectedRunningTableOrderNumber.Text = filteredOrders.Any() ? "Select a table" : "No orders";
    }

    private void LoadCompletedOrders()
    {
        var filteredOrders = orderService.GetCompletedOrdersForRole(isKitchen).ToList();

        PopulateOrdersView(flowPanelSelectTabelOrdersCompleted, filteredOrders);
        SelectedCompletedTableOrderNumber.Text = filteredOrders.Any() ? "Select a table" : "No orders";
    }

    private void LoadPreparingOrders()
    {
        var activeOrderItems = orderService.GetOrderItemsForPreparation(isKitchen);

        var orderItems = activeOrderItems.ToList();

        if (StatusComboBox.SelectedIndex >= 0)
        {
            orderItems = ApplyFiltering(orderItems);
        }

        if (SortBycomboBox.SelectedIndex >= 0)
        {
            orderItems = ApplySorting(orderItems);
        }

        PopulatePreparingOrdersView(orderItems);
    }

    #endregion

    #region UI Population Methods

    private void PopulateOrdersView(FlowLayoutPanel tablePanel, List<Order> orders)
    {
        tablePanel.Controls.Clear();

        foreach (var order in orders)
        {
            var tableControl = new KitchenBarOrderTables
            {
                Order = order,
                IsKitchen = isKitchen
            };

            tableControl.TableSelected += OnTableSelected;
            tablePanel.Controls.Add(tableControl);
        }
    }

    private void PopulateOrderItemsForTable(Order order, FlowLayoutPanel targetPanel)
    {
        targetPanel.Controls.Clear();

        var relevantItems = orderService.GetRelevantOrderItems(order, isKitchen);

        var activeCourseFilter = _activeCourseFilter; 
        if (activeCourseFilter.HasValue)
        {
            relevantItems = relevantItems.Where(item => item.OrderItemCourseType == activeCourseFilter.Value);
        }

        foreach (var item in relevantItems)
        {
            var itemControl = new KitchenBarOrderItems
            {
                OrderItem = item,
                OrderService = orderService
            };

            itemControl.StatusChanged += OnItemStatusChanged;
            itemControl.ActionFailed += OnActionFailed;

            targetPanel.Controls.Add(itemControl);
        }
    }

    private void PopulatePreparingOrdersView(List<OrderItem> orderItems)
    {
        preparingOrdersDataGridView.Rows.Clear();

        foreach (var item in orderItems)
        {
            var row = new DataGridViewRow();
            row.CreateCells(preparingOrdersDataGridView);

            row.Cells[0].Value = item.OrderId?.OrderId ?? 0;
            row.Cells[1].Value = item.OrderItemId;
            row.Cells[2].Value = item.OrderId?.TableId?.TableNumber ?? 0;
            row.Cells[3].Value = item.MenuItemId?.Name ?? "Unknown";
            row.Cells[4].Value = item.Quantity;
            row.Cells[5].Value = item.Status.ToString();
            row.Cells[6].Value = item.Comment ?? "";
            row.Cells[7].Value = item.CreatedAt.ToString("HH:mm");
            row.Cells[8].Value = $"{item.WaitingMinutes} min";
            row.Cells[9] = GetActionButton(item.Status);

            row.Tag = item;
            preparingOrdersDataGridView.Rows.Add(row);
        }

        InitializePreparingOrdersComboBoxes();
    }

    private void InitializePreparingOrdersComboBoxes()
    {
        if (SortBycomboBox.Items.Count == 0)
        {
            string[] sortOptions = ["Waiting Time (Longest First)", "Waiting Time (Shortest First)",
                                   "Order Time (Oldest First)", "Order Time (Newest First)"];
            SortBycomboBox.Items.AddRange(sortOptions);
        }

        if (StatusComboBox.Items.Count == 0)
        {
            StatusComboBox.Items.Add("All");
            StatusComboBox.Items.Add(OrderStatus.Ordered.ToString());
            StatusComboBox.Items.Add(OrderStatus.BeingPrepared.ToString());
        }
    }

    private DataGridViewButtonCell GetActionButton(OrderStatus orderItemStatus)
    {
        var actionCell = new DataGridViewButtonCell();

        switch (orderItemStatus)
        {
            case OrderStatus.Ordered:
                actionCell.Value = "Start";
                actionCell.Style.BackColor = Color.FromArgb(74, 144, 226);
                break;
            case OrderStatus.BeingPrepared:
                actionCell.Value = "Ready";
                actionCell.Style.BackColor = Color.FromArgb(40, 167, 69);
                break;
            case OrderStatus.ReadyToBeServed:
                actionCell.Value = "Served";
                actionCell.Style.BackColor = Color.FromArgb(40, 167, 69);
                break;
        }

        return actionCell;
    }

    #endregion

    #region Event Handlers

    private void OnTableSelected(object sender, EventArgs e)
    {
        _activeCourseFilter = null;
        if (sender is KitchenBarOrderTables tableControl && tableControl.Order != null)
        {
            var order = tableControl.Order;

            if (currentView == ViewMode.RunningOrders)
            {
                SelectedRunningTableOrderNumber.Text = $"Table {order.TableId?.TableNumber ?? 0}";
                PopulateOrderItemsForTable(order, flowPanelSelectedTabelOrderItemsRunning);
            }
            else if (currentView == ViewMode.CompletedOrders)
            {
                SelectedCompletedTableOrderNumber.Text = $"Table {order.TableId?.TableNumber ?? 0}";
                PopulateOrderItemsForTable(order, flowPanelSelectedTabelOrderItemsCompleted);
            }
        }
    }

    private void OnItemStatusChanged(object sender, EventArgs e)
    {
        RefreshData();
    }

    private void OnActionFailed(object sender, string errorMessage)
    {
        MessageBox.Show(errorMessage, "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
    }

    #endregion

    #region Button Event Handlers

    private void btnRefresh_Click(object sender, EventArgs e)
    {
        RefreshData();
    }

    private void btnStartAll_Click(object sender, EventArgs e)
    {
        UpdateMultipleOrderItemsStatus(OrderStatus.Ordered, OrderStatus.BeingPrepared);
    }

    private void btnReadyAll_Click(object sender, EventArgs e)
    {
        UpdateMultipleOrderItemsStatus(OrderStatus.BeingPrepared, OrderStatus.ReadyToBeServed);
    }

    private void btnRunningOrders_Click(object sender, EventArgs e)
    {
        ShowRunningOrders();
    }

    private void btnPreparingOrders_Click(object sender, EventArgs e)
    {
        ShowPreparingOrders();
    }

    private void btnCompletedOrders_Click(object sender, EventArgs e)
    {
        ShowCompletedOrders();
    }

    // Course filter buttons
    private void btnFilterStarters_Click(object sender, EventArgs e)
    {
        OnCourseTypeFilterSelected(CourseType.Starter);
    }

    private void btnStartersFilter_Click(object sender, EventArgs e)
    {
        OnCourseTypeFilterSelected(CourseType.Starter);
    }

    private void btnFilterMains_Click(object sender, EventArgs e)
    {
        OnCourseTypeFilterSelected(CourseType.Main);
    }

    private void btnMainsFilter_Click(object sender, EventArgs e)
    {
        OnCourseTypeFilterSelected(CourseType.Main);
    }

    private void btnFilterDesserts_Click(object sender, EventArgs e)
    {
        OnCourseTypeFilterSelected(CourseType.Dessert);
    }

    private void btnDessertsFilter_Click(object sender, EventArgs e)
    {
        OnCourseTypeFilterSelected(CourseType.Dessert);
    }

    private void btnFilterAll_Click(object sender, EventArgs e)
    {
        _activeCourseFilter = null; 
        RefreshCurrentTableItems();
    }

    private void btnAllFilter_Click(object sender, EventArgs e)
    {
        _activeCourseFilter = null; 
        RefreshCurrentTableItems();
    }

    private void StatusComboBox_SelectedIndexChanged(object sender, EventArgs e)
    {
        RefreshData();
    }

    private void SortBycomboBox_SelectedIndexChanged(object sender, EventArgs e)
    {
        RefreshData();
    }

    private void preparingOrdersDataGridView_CellContentClick(object sender, DataGridViewCellEventArgs e)
    {
        if (e.ColumnIndex == preparingOrdersDataGridView.Columns["OrderItemActions"].Index && e.RowIndex >= 0)
        {
            var row = preparingOrdersDataGridView.Rows[e.RowIndex];
            var orderItem = row.Tag as OrderItem;

            if (orderItem == null) return;

            OrderStatus newStatus;
            if (orderItem.Status == OrderStatus.Ordered)
            {
                newStatus = OrderStatus.BeingPrepared;
            }
            else if (orderItem.Status == OrderStatus.BeingPrepared)
            {
                newStatus = OrderStatus.ReadyToBeServed;
            }
            else
            {
                return;
            }

            UpdateOrderItemStatus(orderItem.OrderItemId, newStatus);
        }
    }

    #endregion

    #region Helper Methods

    private void OnCourseTypeFilterSelected(CourseType courseType)
    {
        _activeCourseFilter = courseType; 
        RefreshCurrentTableItems();
    }

    private void RefreshCurrentTableItems()
    {
        var selectedOrder = GetCurrentlySelectedOrder();
        if (selectedOrder != null)
        {
            if (currentView == ViewMode.RunningOrders)
            {
                PopulateOrderItemsForTable(selectedOrder, flowPanelSelectedTabelOrderItemsRunning);
            }
            else if (currentView == ViewMode.CompletedOrders)
            {
                PopulateOrderItemsForTable(selectedOrder, flowPanelSelectedTabelOrderItemsCompleted);
            }
        }
    }

    private Order GetCurrentlySelectedOrder()
    {
        // Extract table number from the header label instead of storing it
        var headerText = currentView == ViewMode.RunningOrders ?
            SelectedRunningTableOrderNumber.Text : SelectedCompletedTableOrderNumber.Text;

        if (headerText.StartsWith("Table ") && int.TryParse(headerText.Substring(6), out int tableNumber))
        {
            // Get the order from the current panel controls
            var panel = currentView == ViewMode.RunningOrders ?
                flowPanelSelectTabelOrdersRunning : flowPanelSelectTabelOrdersCompleted;

            var tableControl = panel.Controls.OfType<KitchenBarOrderTables>()
                .FirstOrDefault(tc => tc.Order?.TableId?.TableNumber == tableNumber);

            return tableControl?.Order;
        }

        return null;
    }

    private void UpdateMultipleOrderItemsStatus(OrderStatus oldStatus, OrderStatus newStatus)
    {
        var selectedOrder = GetCurrentlySelectedOrder();
        if (selectedOrder == null) return;

        var relevantItems = orderService.GetRelevantOrderItems(selectedOrder, isKitchen)
            .Where(item => item.Status == oldStatus)
            .Select(item => item.OrderItemId);

        var success = orderService.MarkMultipleItemsStatus(relevantItems, newStatus);
        if (success)
        {
            RefreshData();
        }
        else
        {
            MessageBox.Show("Failed to update some items.", "Warning",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
    }

    private void UpdateOrderItemStatus(int orderItemId, OrderStatus newStatus)
    {
        try
        {
            var success = orderService.UpdateOrderItemStatus(orderItemId, newStatus);
            if (success)
            {
                RefreshData();
            }
            else
            {
                MessageBox.Show("Failed to update order status.", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error updating status: {ex.Message}", "Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private List<OrderItem> ApplySorting(List<OrderItem> orderItems)
    {
        var sortFilter = SortBycomboBox.SelectedItem?.ToString() ?? "Waiting Time (Longest First)";

        return sortFilter switch
        {
            "Waiting Time (Longest First)" => orderItems.OrderBy(oi => oi.CreatedAt).ToList(),
            "Waiting Time (Shortest First)" => orderItems.OrderByDescending(oi => oi.CreatedAt).ToList(),
            "Order Time (Oldest First)" => orderItems.OrderBy(oi => oi.CreatedAt).ToList(),
            "Order Time (Newest First)" => orderItems.OrderByDescending(oi => oi.CreatedAt).ToList(),
            _ => orderItems.OrderBy(oi => oi.CreatedAt).ToList()
        };
    }

    private List<OrderItem> ApplyFiltering(List<OrderItem> orderItems)
    {
        var statusFilter = StatusComboBox.SelectedItem?.ToString() ?? "All";

        if (statusFilter != "All")
        {
            if (Enum.TryParse<OrderStatus>(statusFilter, out var status))
            {
                orderItems = orderItems.Where(oi => oi.Status == status).ToList();
            }
        }

        return orderItems;
    }

    #endregion

    #region View Management

    private void ShowRunningOrders()
    {
        currentView = ViewMode.RunningOrders;
        UpdateButtonColors();
        UpdatePanelVisibility();
        RefreshData();
    }

    private void ShowPreparingOrders()
    {
        currentView = ViewMode.PrepareOrders;
        UpdateButtonColors();
        UpdatePanelVisibility();
        RefreshData();
    }

    private void ShowCompletedOrders()
    {
        currentView = ViewMode.CompletedOrders;
        UpdateButtonColors();
        UpdatePanelVisibility();
        RefreshData();
    }

    private void UpdateButtonColors()
    {
        btnRunningOrders.BackColor = Color.DarkGray;
        btnPreparingOrders.BackColor = Color.DarkGray;
        btnCompletedOrders.BackColor = Color.DarkGray;

        switch (currentView)
        {
            case ViewMode.RunningOrders:
                btnRunningOrders.BackColor = Color.MediumTurquoise;
                break;
            case ViewMode.PrepareOrders:
                btnPreparingOrders.BackColor = Color.MediumTurquoise;
                break;
            case ViewMode.CompletedOrders:
                btnCompletedOrders.BackColor = Color.MediumTurquoise;
                break;
        }
    }

    private void UpdatePanelVisibility()
    {
        RunningOrdersPanel.Visible = currentView == ViewMode.RunningOrders;
        PreparingOrdersPanel.Visible = currentView == ViewMode.PrepareOrders;
        CompletedOrdersPanel.Visible = currentView == ViewMode.CompletedOrders;
    }

    #endregion

    #region Unused Event Handlers (keeping for designer compatibility)

    private void preparingOrdersDataGridView_Click(object sender, EventArgs e)
    {
       
    }

    private void KitchenBarOrderTables_Load(object sender, EventArgs e)
    {

    }

    private void OrderedItemStatusStatsLabel_Click(object sender, EventArgs e)
    {
    }




    private void CompletedOrdersPanel_Paint(object sender, PaintEventArgs e)
    {

    }

    #endregion
}