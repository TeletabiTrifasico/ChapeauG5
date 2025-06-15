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

namespace ChapeauG5.ChapeauUI.Forms;

public partial class KitchenBarViewForm : Form
{
    private readonly bool isKitchen;
    private readonly KitchenBarOrderService orderService;
    private Timer _refreshTimer;
    private int _selectedOrderId, _selectedTableNumber = -1;
    private CourseType? _activeCourseFilter = null;
    private enum ViewMode { RunningOrders, CompletedOrders, PrepareOrders }
    private ViewMode currentView = ViewMode.RunningOrders;

    private List<Order> currentOrders = new List<Order>();
    private List<OrderItem> currentOrderItems = new List<OrderItem>();

    public KitchenBarViewForm(bool isKitchenEmployee)
    {
        InitializeComponent();
        isKitchen = isKitchenEmployee;
        orderService = new KitchenBarOrderService();
        InitializeForm();
    }

    private async void InitializeForm()
    {
        // Set title based on kitchen or bar
        KitchenBarOrdersTitle.Text = isKitchen ? "Kitchen Orders" : "Bar Orders";

        // Initialize combo boxes with default selections
        SortBycomboBox.SelectedIndex = -1;
        StatusComboBox.SelectedIndex = -1;

        // Set default view
        ShowRunningOrders();

        // Initialize refresh timer
        InitializeTimer();
    }

    private void InitializeTimer()
    {
        _refreshTimer = new Timer();
        _refreshTimer.Interval = 30000; // 30 seconds
        _refreshTimer.Tick += async (sender, e) => await RefreshData();
        _refreshTimer.Start();
    }

    #region Data Loading Methods

    private async Task RefreshData()
    {
        try
        {
            switch (currentView)
            {
                case ViewMode.RunningOrders:
                    await LoadRunningOrders();
                    break;
                case ViewMode.PrepareOrders:
                    await LoadPreparingOrders();
                    break;
                case ViewMode.CompletedOrders:
                    await LoadCompletedOrders();
                    break;
            }      
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error loading data: {ex.Message}", "Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private async Task LoadRunningOrders()
    {
        var activeOrders = await orderService.GetActiveOrdersAsync();
        currentOrders = FilterOrdersByRole(activeOrders).ToList();
        PopulateOrdersView(flowPanelSelectTabelOrdersRunning,
                          flowPanelSelectedTabelOrderItemsRunning,
                          SelectedRunningTableOrderNumber);
        RestoreSelected_TableOrder_Fiilters();
    }

    private async Task LoadCompletedOrders()
    {
        var completedOrders = await orderService.GetCompletedOrdersAsync();
        currentOrders = FilterOrdersByRole(completedOrders).ToList();
        PopulateOrdersView(flowPanelSelectTabelOrdersCompleted,
                          flowPanelSelectedTabelOrderItemsCompleted,
                          SelectedCompletedTableOrderNumber);
        RestoreSelected_TableOrder_Fiilters();
    }

    private async Task LoadPreparingOrders()
    {
        IEnumerable<OrderItem> activieOrderItems;
        if (isKitchen)
        {
            activieOrderItems = await orderService.GetKitchenOrdersAsync();
        }
        else
        {
            activieOrderItems = await orderService.GetBarOrdersAsync();
        }

        currentOrderItems = activieOrderItems.ToList();

        if (StatusComboBox.SelectedIndex >= 0)
        {
            currentOrderItems = ApplyFiltering(activieOrderItems.ToList());
        }
        else if (SortBycomboBox.SelectedIndex >= 0)
        {
            currentOrderItems = ApplySorting(activieOrderItems.ToList());

        }

        PopulatePreparingOrdersView();
    }

    private void RestoreSelected_TableOrder_Fiilters()
    {
        if (_selectedTableNumber > 0 && (currentView == ViewMode.RunningOrders || currentView == ViewMode.CompletedOrders))
        {
            var orderToRestore = currentOrders.FirstOrDefault(o => o.TableId?.TableNumber == _selectedTableNumber);
            if (orderToRestore != null)
            {
                OnTableOrderSelected(orderToRestore.OrderId, clearFilter: false);

                // Restore course filter if we had one
                if (_activeCourseFilter.HasValue)
                {
                    OnCourseTypeFilterSelected(_activeCourseFilter.Value);
                }
            }
            else
            {
                // Clear if table no longer exists
                _selectedTableNumber = -1;
                _activeCourseFilter = null;
            }
        }
    }

    private IEnumerable<Order> FilterOrdersByRole(IEnumerable<Order> orders)
    {
        return orders.Where(order => order.OrderItems.Any(item =>
            isKitchen ? IsKitchenItem(item) : IsBarItem(item)));
    }

    private bool IsKitchenItem(OrderItem item)
    {
        return item.MenuItemId?.CategoryId?.MenuCard == MenuCard.Food;
    }

    private bool IsBarItem(OrderItem item)
    {
        return item.MenuItemId?.CategoryId?.MenuCard == MenuCard.Drinks;
    }

    #endregion

    #region UI Population Methods

    private void PopulateOrdersView(FlowLayoutPanel tablePanel, FlowLayoutPanel itemPanel, Label headerLabel)
    {
        // Clear both panels
        tablePanel.Controls.Clear();
        itemPanel.Controls.Clear();

        // Populate table orders
        foreach (var order in currentOrders)
        {
            var tableControl = CreateTableOrderControl(order);
            tablePanel.Controls.Add(tableControl);
        }

        // Set appropriate header text
        headerLabel.Text = currentOrders.Any() ? "Select a table" : "No orders";
    }

    private void PopulateOrderItemsForTable(Order order, FlowLayoutPanel targetPanel)
    {
        targetPanel.Controls.Clear();

        var relevantItems = order.OrderItems.Where(item =>
            isKitchen ? IsKitchenItem(item) : IsBarItem(item));

        currentOrderItems = relevantItems.ToList();

        foreach (var item in relevantItems)
        {
            var itemControl = CreateOrderItemControl(item);
            targetPanel.Controls.Add(itemControl);
        }
    }

    private void PopulatePreparingOrdersView()
    {
        // Clear existing rows
        preparingOrdersDataGridView.Rows.Clear();

        // Populate DataGridView
        foreach (var item in currentOrderItems)
        {
            var row = new DataGridViewRow();
            row.CreateCells(preparingOrdersDataGridView);

            row.Cells[0].Value = item.OrderId; // Order#
            row.Cells[1].Value = item.OrderId?.TableId?.TableNumber ?? 0; // Table #
            row.Cells[2].Value = item.OrderItemId;
            row.Cells[3].Value = item.MenuItemId?.Name ?? "Unknown"; // ItemName
            row.Cells[4].Value = item.Quantity; // Quantity
            row.Cells[5].Value = item.Status.ToString(); // Status
            row.Cells[6].Value = item.Comment ?? ""; // Comment
            row.Cells[7].Value = item.CreatedAt.ToString("HH:mm"); // Order Time
            row.Cells[8].Value = $"{item.WaitingMinutes} min"; // Waiting
            row.Cells[9] = GetActionButton(item.Status);

            row.Tag = item; // Store the OrderItem for later use
            preparingOrdersDataGridView.Rows.Add(row);
        }
        PreparingOrdersComboboxItems();


    }

    private void PreparingOrdersComboboxItems()
    {
        SortBycomboBox.Items.Clear();
        StatusComboBox.Items.Clear();

        string[] sortList = ["Waiting Time (Longest First)", "Waiting Time (Shortest First)", "Order Time (Oldest First)", "Order Time (Newest First)"];
        OrderStatus [] statusFilterList = [OrderStatus.Ordered, OrderStatus.BeingPrepared];


        foreach (var sort in sortList)
        {
            SortBycomboBox.Items.Add(sort).ToString();
        }
        foreach (var statusFilter in statusFilterList)
        {
            StatusComboBox.Items.Add("All").ToString();
            StatusComboBox.Items.Add(statusFilter).ToString();
        }
    }
    

    private UserControl CreateTableOrderControl(Order order)
    {
        var control = new KitchenOrderTables();

        // Update the labels in the control
        var tableLabel = control.Controls.Find("OrderTableNumberLabel", true).FirstOrDefault() as Label;
        var countLabel = control.Controls.Find("totalOrderItemsCountLabel", true).FirstOrDefault() as Label;
        var statsLabel = control.Controls.Find("OrderedItemStatusStatsLabel", true).FirstOrDefault() as Label;

        if (tableLabel != null)
            tableLabel.Text = $"Table {order.TableId?.TableNumber ?? 0}";

        var relevantItems = order.OrderItems.Where(item =>
            isKitchen ? IsKitchenItem(item) : IsBarItem(item)).ToList();

        if (countLabel != null)
            countLabel.Text = relevantItems.Count.ToString();

        if (statsLabel != null)
        {
            var ordered = relevantItems.Count(i => i.Status == OrderStatus.Ordered);
            var preparing = relevantItems.Count(i => i.Status == OrderStatus.BeingPrepared);
            var ready = relevantItems.Count(i => i.Status == OrderStatus.ReadyToBeServed);

            statsLabel.Text = $"Ordered: {ordered} Preparing: {preparing} Ready: {ready}";
        }

        // Add click event
        control.Click += (sender, e) => OnTableOrderSelected(order.OrderId);
        control.Tag = order;

        return control;
    }

    private UserControl CreateOrderItemControl(OrderItem orderItem)
    {
        var control = new KitchenBarOrderItems();

        // Update the labels in the control
        var nameLabel = control.Controls.Find("orderItemName", true).FirstOrDefault() as Label;
        var quantityLabel = control.Controls.Find("orderItemQuantity", true).FirstOrDefault() as Label;
        var commentLabel = control.Controls.Find("orderItemComment", true).FirstOrDefault() as Label;
        var statusLabel = control.Controls.Find("orderItemStatus", true).FirstOrDefault() as Label;
        var startButton = control.Controls.Find("btnStartOrderItem", true).FirstOrDefault() as System.Windows.Forms.Button;
        var readyButton = control.Controls.Find("btnReadyOrderItem", true).FirstOrDefault() as System.Windows.Forms.Button; // as Button

        if (nameLabel != null)
            nameLabel.Text = orderItem.MenuItemId?.Name ?? "Unknown";

        if (quantityLabel != null)
            quantityLabel.Text = orderItem.Quantity.ToString();

        if (commentLabel != null)
            commentLabel.Text = orderItem.Comment ?? "";

        if (statusLabel != null)
            statusLabel.Text = orderItem.Status.ToString();

        // set buttons based on status
        if (startButton != null)
        {
            startButton.Enabled = orderItem.Status == OrderStatus.Ordered;
            startButton.Click += async (sender, e) => await UpdateOrderItemStatus(orderItem.OrderItemId, OrderStatus.BeingPrepared);
        }

        if (readyButton != null)
        {
            readyButton.Enabled = orderItem.Status == OrderStatus.BeingPrepared;
            readyButton.Click += async (sender, e) => await UpdateOrderItemStatus(orderItem.OrderItemId, OrderStatus.ReadyToBeServed);
        }

        control.Tag = orderItem;
        return control;
    }

    private DataGridViewButtonCell GetActionButton(OrderStatus OrderItemStatus)
    {
        var actionCell = new DataGridViewButtonCell();

        if (OrderItemStatus != null)
        {
            switch (OrderItemStatus)
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
                default:
                    break;
            }
        }
        return actionCell;
    }

    #endregion

    #region Event Handlers
    private async void preparingOrdersDataGridView_CellContentClick(object sender, DataGridViewCellEventArgs e)
    {
        if (e.ColumnIndex == preparingOrdersDataGridView.Columns["OrderItemActions"].Index && e.RowIndex >= 0)
        {
            var row = preparingOrdersDataGridView.Rows[e.RowIndex];
            var orderItemId = Convert.ToInt32(row.Cells["OrderItemTableNumber"].Value);
            var currentStatus = row.Cells["OrderItemStatus"].Value.ToString();
            MessageBox.Show(orderItemId.ToString());
            OrderStatus newStatus;
            if (currentStatus == "Ordered")
            {
                newStatus = OrderStatus.BeingPrepared;
            }
            else if (currentStatus == "Preparing")
            {
                newStatus = OrderStatus.ReadyToBeServed;
            }
            else
            {
                return; // No action needed
            }

            await UpdateOrderItemStatus(orderItemId, newStatus);
        }

    }

    private async void btnRefresh_Click(object sender, EventArgs e)
    {
        await RefreshData();
    }

    private async void btnStartAll_Click(object sender, EventArgs e)
    {
        UpdateMultipleOrderItemsStatus(OrderStatus.Ordered, OrderStatus.BeingPrepared);

    }

    private async void btnReadyAll_Click(object sender, EventArgs e)
    {
        UpdateMultipleOrderItemsStatus(OrderStatus.BeingPrepared, OrderStatus.ReadyToBeServed);
    }


    private async void UpdateMultipleOrderItemsStatus(OrderStatus oldOrderItemStatus, OrderStatus newOrderItemStatus)
    {
        if (_selectedOrderId <= 0) return;

        var order = currentOrders.FirstOrDefault(o => o.OrderId == _selectedOrderId);
        if (order == null) return;

        var relevantItems = order.OrderItems
            .Where(item => (isKitchen ? IsKitchenItem(item) : IsBarItem(item)) &&
                          item.Status == oldOrderItemStatus)
            .Select(item => item.OrderItemId);

        await orderService.MarkMultipleItemsStatusAsync(relevantItems, newOrderItemStatus);
        await RefreshData();
    }


    private void OnTableOrderSelected(int orderId, bool clearFilter = true)
    {
        _selectedOrderId = orderId;
        var order = currentOrders.FirstOrDefault(o => o.OrderId == orderId);

        if (order == null) return;

        _selectedTableNumber = order.TableId?.TableNumber ?? -1;

        if (clearFilter)
            _activeCourseFilter = null; // Reset filter when selecting new table

        // Update header and populate order items
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

    private void OnCourseTypeFilterSelected(CourseType courseType)
    {
        _activeCourseFilter = courseType;

        if (currentView == ViewMode.RunningOrders)
        {
            // Clear and populate order items
            flowPanelSelectedTabelOrderItemsRunning.Controls.Clear();

            var relevantItems = currentOrderItems.Where(item => item.OrderItemCourseType == courseType);


            foreach (var item in relevantItems)
            {
                var itemControl = CreateOrderItemControl(item);
                flowPanelSelectedTabelOrderItemsRunning.Controls.Add(itemControl);
            }
        }
        else if (currentView == ViewMode.CompletedOrders)
        {
            // Clear and populate order items
            flowPanelSelectedTabelOrderItemsCompleted.Controls.Clear();

            var relevantItems = currentOrderItems.Where(item => item.OrderItemCourseType == courseType);


            foreach (var item in relevantItems)
            {
                var itemControl = CreateOrderItemControl(item);
                flowPanelSelectedTabelOrderItemsCompleted.Controls.Add(itemControl);
            }
        }
    }


    private async Task UpdateOrderItemStatus(int orderItemId, OrderStatus newStatus)
    {
        try
        {
            var success = await orderService.UpdateOrderItemStatusAsync(orderItemId, newStatus);
            if (success)
            {
                await RefreshData();
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

    private void btnPreparingOrders_Click(object sender, EventArgs e)
    {
        ShowPreparingOrders();
    }

    private void btnCompletedOrders_Click(object sender, EventArgs e)
    {
        ShowCompletedOrders();
    }

    private void btnRunningOrders_Click(object sender, EventArgs e)
    {
        ShowRunningOrders();
    }

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

    private void btnDessertsFilter_Click(object sender, EventArgs e)
    {
        OnCourseTypeFilterSelected(CourseType.Dessert);
    }

    private void btnFilterDesserts_Click(object sender, EventArgs e)
    {
        OnCourseTypeFilterSelected(CourseType.Dessert);
    }

    #endregion

    #region View Management

    private void ShowPreparingOrders()
    {
        currentView = ViewMode.PrepareOrders;
        UpdateButtonColors();
        UpdatePanelVisibility();
        _ = RefreshData();
    }

    private void ShowRunningOrders()
    {
        _selectedTableNumber = -1;

        currentView = ViewMode.RunningOrders;
        UpdateButtonColors();
        UpdatePanelVisibility();
        _ = RefreshData();
    }

    private void ShowCompletedOrders()
    {
        _selectedTableNumber = -1; // Clear selection when switching views

        currentView = ViewMode.CompletedOrders;
        UpdateButtonColors();
        UpdatePanelVisibility();
        _ = RefreshData(); // Fire and forget async call
    }

    private void UpdateButtonColors()
    {
        // Reset all buttons
        btnRunningOrders.BackColor = Color.DarkGray;
        btnPreparingOrders.BackColor = Color.DarkGray;
        btnCompletedOrders.BackColor = Color.DarkGray;

        // Set active button
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

    private void preparingOrdersDataGridView_Click(object sender, EventArgs e)
    {

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
            var status = (OrderStatus)Enum.Parse(typeof(OrderStatus), statusFilter.Replace(" ", ""));
            orderItems = orderItems.Where(oi => oi.Status == status).ToList();
        }
        return orderItems;            
    }

    private async void StatusComboBox_SelectedIndexChanged(object sender, EventArgs e)
    {
       await RefreshData();

    }

    private async void SortBycomboBox_SelectedIndexChanged(object sender, EventArgs e)
    {
        await RefreshData();

    }
}