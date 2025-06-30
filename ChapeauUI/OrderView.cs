using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using ChapeauModel;
using ChapeauService;

namespace ChapeauG5
{
    public partial class OrderView : Form
    {
        // The currently logged-in employee
        private Employee loggedInEmployee;
        // The table for which the order is being managed
        private Table selectedTable;
        // Services for menu, order, and table operations
        private MenuService menuService;
        private OrderService orderService;
        private TableService tableService;
        // The current order ID (nullable, as there may not be an order yet)
        private int? currentOrderId;
        // List of items already ordered (from the database)
        private List<OrderItem> orderedItems;
        // List of new items added in this session (not yet saved)
        private List<OrderItem> newOrderItems;
        // Indicates if we are editing an existing order
        private bool isExistingOrder = false;
        
        // Constructor: initializes services, state, and references to employee/table
        public OrderView(Employee employee, Table table)
        {
            InitializeComponent();
            loggedInEmployee = employee;
            selectedTable = table;
            menuService = new MenuService();
            orderService = new OrderService();
            tableService = new TableService();
            orderedItems = new List<OrderItem>();
            newOrderItems = new List<OrderItem>();
        }
        
        // Form load event: sets up UI and loads existing order if present
        private void OrderView_Load(object sender, EventArgs e)
        {
            try
            {
                lblTable.Text = $"Table {selectedTable.TableNumber}";

                LoadMenuCategories();
                
                // Check if there's already an order for this table
                Order existingOrder = orderService.GetOrderByTableId(selectedTable.TableId);
                
                if (existingOrder != null)
                {
                    // Load order and its items
                    Order orderWithItems = orderService.GetOrderWithItemsById(existingOrder.OrderId);
                    currentOrderId = orderWithItems.OrderId;
                    isExistingOrder = true;

                    orderedItems = orderWithItems.OrderItems ?? new List<OrderItem>();
                    LoadOrderedItems();

                    // Mark table as occupied
                    tableService.UpdateTableStatus(selectedTable.TableId, TableStatus.Occupied);
                }

                UpdatePaymentButtonState();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading order view: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Loads menu categories into the category combo box
        private void LoadMenuCategories()
        {
            try
            {
                cmbCategory.Items.Clear();
                
                List<MenuCategory> categories = menuService.GetAllCategories();
                
                foreach (MenuCategory category in categories)
                {
                    cmbCategory.Items.Add(category);
                }
                
                if (cmbCategory.Items.Count > 0)
                    cmbCategory.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading menu categories: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // When a menu category is selected, load its items
        private void cmbCategory_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (cmbCategory.SelectedItem is MenuCategory selectedCategory)
                {
                    SetupMenuListView();
                    LoadMenuItems(selectedCategory);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading menu items: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Set up the columns for the menu items ListView
        private void SetupMenuListView()
        {
            try
            {
                lvMenuItems.Columns.Clear();
                lvMenuItems.FullRowSelect = true;
                lvMenuItems.View = View.Details;
                lvMenuItems.Columns.Add("Item");
                lvMenuItems.Columns.Add("Price");
                lvMenuItems.Columns.Add("Description");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error setting up menu list view: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Load menu items for the selected category
        private void LoadMenuItems(MenuCategory category)
        {
            try
            {
                var menuItems = menuService.GetMenuItemsByCategory(category.CategoryId);
                lvMenuItems.Items.Clear();

                foreach (var item in menuItems)
                {
                    lvMenuItems.Items.Add(CreateListViewItem(item));
                }

                lvMenuItems.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading menu items: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Helper to create a ListViewItem for a menu item
        private ListViewItem CreateListViewItem(MenuItem item)
        {
            try
            {
                var lvi = new ListViewItem(item.Name);
                lvi.SubItems.Add($"€{item.Price:0.00}");
                lvi.SubItems.Add(item.Description);
                lvi.Tag = item;
                return lvi;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error creating list view item: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return new ListViewItem("Error loading item");
            }
        }

        // Add selected menu item to the new order list
        private void btnAddToOrder_Click(object sender, EventArgs e)
        {
            try
            {
                if (!ValidateSelection(out MenuItem selectedItem, out int quantity, out string comment)) return;

                // Check if item with same comment already exists in new order
                var existingItem = orderService.FindExistingOrderItem(newOrderItems, selectedItem, comment);

                if (existingItem != null)
                {
                    // If exists, just update the quantity
                    orderService.UpdateOrderItemQuantity(existingItem, quantity);
                }
                else
                {
                    // Otherwise, create a new order item
                    var newItem = orderService.CreateNewOrderItem(selectedItem, quantity, comment, currentOrderId);
                    newOrderItems.Add(newItem);
                }

                ListNewOrders();
                ResetOrderForm();
                btnSaveOrder.Visible = true;

                MessageBox.Show($"{quantity}x {selectedItem.Name} added to order.", "Item Added",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error adding item to order: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Validates that a menu item is selected and quantity is valid
        private bool ValidateSelection(out MenuItem selectedItem, out int quantity, out string comment)
        {
            selectedItem = null;
            quantity = (int)nudQuantity.Value;
            comment = txtComment.Text;

            try
            {
                if (lvMenuItems.SelectedItems.Count == 0)
                {
                    MessageBox.Show("Please select a menu item first.", "No Item Selected",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }

                if (quantity <= 0)
                {
                    MessageBox.Show("Please enter a valid quantity.", "Invalid Quantity",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }

                selectedItem = (MenuItem)lvMenuItems.SelectedItems[0].Tag;
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error validating selection: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        // Resets the add-to-order form fields
        private void ResetOrderForm()
        {
            try
            {
                nudQuantity.Value = 1;
                txtComment.Clear();
                lvMenuItems.SelectedItems.Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error resetting order form: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Save the current order (new or existing)
        private void btnSaveOrder_Click(object sender, EventArgs e)
        {
            try
            {
                if (!ValidateOrder()) return;

                if (!currentOrderId.HasValue)
                    CreateNewOrder();

                SaveOrderItems();

                FinalizeOrder();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving order: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Checks if there are items to save
        private bool ValidateOrder()
        {
            if (!orderService.ValidateOrderHasItems(newOrderItems))
            {
                MessageBox.Show("Please add at least one item to the order.",
                    "Empty Order", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            return true;
        }

        // Creates a new order in the database
        private void CreateNewOrder()
        {
            currentOrderId = orderService.CreateOrder(selectedTable.TableId, loggedInEmployee.EmployeeId);
            tableService.UpdateTableStatus(selectedTable.TableId, TableStatus.Occupied);
        }

        // Saves all new order items to the database
        private void SaveOrderItems()
        {
            orderService.SaveOrderItems(currentOrderId.Value, newOrderItems);
        }

        // Finalizes the order after saving: updates UI and clears new items
        private void FinalizeOrder()
        {
            isExistingOrder = true;
            MessageBox.Show("Order saved successfully!", "Success",
                MessageBoxButtons.OK, MessageBoxIcon.Information);

            orderedItems = orderService.GetOrderItemsByOrderId(currentOrderId.Value);
            newOrderItems.Clear();
            lvOrderItems.Items.Clear();
            LoadOrderedItems();
        }

        // Remove selected item from the new order list
        private void btnRemoveItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (!ValidateItemSelection(out OrderItem selectedItem)) return;

                // Confirm removal if item is already saved in DB
                if (selectedItem.OrderItemId != 0 && !ConfirmItemRemoval())
                    return;

                orderService.RemoveOrderItem(newOrderItems, selectedItem);
                ListNewOrders();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error removing item: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Validates that an order item is selected for removal/editing
        private bool ValidateItemSelection(out OrderItem selectedItem)
        {
            selectedItem = null;

            try
            {
                if (lvOrderItems.SelectedItems.Count == 0)
                {
                    MessageBox.Show("Please select an item to remove.",
                        "No Item Selected", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }

                selectedItem = (OrderItem)lvOrderItems.SelectedItems[0].Tag;
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error validating item selection: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        // Asks user to confirm removal of an already saved item
        private bool ConfirmItemRemoval()
        {
            try
            {
                DialogResult result = MessageBox.Show(
                    "Are you sure you want to remove it?",
                    "Confirm Removal",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                return result == DialogResult.Yes;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error confirming item removal: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        // Edit selected order item (quantity/comment)
        private void btnEditItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (!ValidateItemSelection(out OrderItem selectedItem)) return;

                using (Form editForm = BuildEditForm(selectedItem, out NumericUpDown nudEditQuantity, out TextBox txtEditComment))
                {
                    if (editForm.ShowDialog() == DialogResult.OK)
                    {
                        orderService.UpdateOrderItemDetails(selectedItem, (int)nudEditQuantity.Value, txtEditComment.Text);
                        ListNewOrders();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error editing item: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Builds a modal form for editing an order item
        private Form BuildEditForm(OrderItem selectedItem, out NumericUpDown nudEditQuantity, out TextBox txtEditComment)
        {
            try
            {
                Form editForm = new Form
                {
                    Text = "Edit Order Item",
                    Size = new Size(600, 400),
                    StartPosition = FormStartPosition.CenterParent,
                    FormBorderStyle = FormBorderStyle.FixedDialog,
                    MaximizeBox = false,
                    MinimizeBox = false
                };

                Label lblQuantity = new Label { Text = "Quantity:", Location = new Point(20, 20), Size = new Size(70, 30) };
                nudEditQuantity = new NumericUpDown { Minimum = 1, Maximum = 20, Value = selectedItem.Quantity, Location = new Point(150, 20), Size = new Size(70, 30) };

                Label lblComment = new Label { Text = "Comment:", Location = new Point(20, 80), Size = new Size(70, 50) };
                txtEditComment = new TextBox { Text = selectedItem.Comment, Location = new Point(150, 80), Size = new Size(170, 20) };

                Button btnSave = new Button { Text = "Save", DialogResult = DialogResult.OK, Location = new Point(20, 150), Size = new Size(160, 60) };
                Button btnCancel = new Button { Text = "Cancel", DialogResult = DialogResult.Cancel, Location = new Point(200, 150), Size = new Size(160, 60) };

                editForm.Controls.AddRange(new Control[] { lblQuantity, nudEditQuantity, lblComment, txtEditComment, btnSave, btnCancel });
                editForm.AcceptButton = btnSave;
                editForm.CancelButton = btnCancel;

                return editForm;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error building edit form: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                nudEditQuantity = null;
                txtEditComment = null;
                return new Form();
            }
        }

        // Cancel and close the order view, possibly resetting table status
        private void btnCancel_Click(object sender, EventArgs e)
        {
            try
            {
                if (!ConfirmCancel()) return;

                if (ShouldResetTable())
                {
                    ResetTableStatus();
                }

                this.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error resetting table status: {ex.Message}");
                MessageBox.Show($"Could not reset table status: {ex.Message}", "Warning",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                this.Close();
            }
        }

        // Confirm with user before closing if there are unsaved changes
        private bool ConfirmCancel()
        {
            try
            {
                if (newOrderItems.Count > 0 && !isExistingOrder)
                {
                    DialogResult result = MessageBox.Show(
                        "Are you sure you want to close this order view? All unsaved changes will be lost.",
                        "Confirm Close",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question);

                    return result == DialogResult.Yes;
                }
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error confirming cancel: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        // Determines if the table status should be reset to free
        private bool ShouldResetTable()
        {
            return !isExistingOrder;
        }

        // Sets the table status to free in the database
        private void ResetTableStatus()
        {
            try
            {
                tableService.UpdateTableStatus(selectedTable.TableId, TableStatus.Free);
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to reset table status: {ex.Message}", ex);
            }
        }

        // Mark selected order item as served
        private void btnMarkServed_Click(object sender, EventArgs e)
        {
            try
            {
                if (!TryGetSelectedOrderItem(out OrderItem selectedItem)) return;

                if (orderService.IsAlreadyServed(selectedItem)) 
                {
                    MessageBox.Show("This item has already been served.",
                        "Already Served", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                if (!orderService.CanBeMarkedAsServed(selectedItem, isExistingOrder)) 
                {
                    MessageBox.Show("You need to save the order before marking items as served.",
                        "Save Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                orderService.MarkItemAsServed(selectedItem);
                RefreshOrderViews();

                MessageBox.Show("Item marked as served.", "Status Updated",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating item status: {ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Helper to get the selected order item from the ordered list
        private bool TryGetSelectedOrderItem(out OrderItem selectedItem)
        {
            selectedItem = null;
            try
            {
                if (orderedList.SelectedItems.Count == 0)
                {
                    MessageBox.Show("Please select an item to mark as served.",
                        "No Item Selected", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }

                selectedItem = (OrderItem)orderedList.SelectedItems[0].Tag;
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error getting selected order item: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        // Refreshes both new and ordered items views and payment button
        private void RefreshOrderViews()
        {
            try
            {
                ListNewOrders();
                LoadOrderedItems();
                UpdatePaymentButtonState();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error refreshing order views: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Handles payment button click: checks if all items are served and order is saved
        private void btnPayment_Click(object sender, EventArgs e)
        {
            try
            {
                if (!AreAllItemsServed()) return;

                if (!IsOrderSaved(sender, e)) return;

                if (IsOrderEmpty()) return;

                ProcessPayment();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error processing payment: {ex.Message}",
                    "Payment Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Checks if all items in the new order are marked as served
        private bool AreAllItemsServed()
        {
            if (!orderService.AreAllItemsServed(newOrderItems))
            {
                MessageBox.Show("All order items must be served before proceeding to payment.",
                    "Items Not Served", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            return true;
        }

        // Ensures the order is saved before payment
        private bool IsOrderSaved(object sender, EventArgs e)
        {
            if (!currentOrderId.HasValue || !isExistingOrder)
            {
                DialogResult result = MessageBox.Show(
                    "You need to save the order before proceeding to payment.\nWould you like to save it now?",
                    "Save Order",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    btnSaveOrder_Click(sender, e);

                    if (!currentOrderId.HasValue)
                    {
                        MessageBox.Show("Cannot proceed to payment without a saved order.",
                            "Payment Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return false;
                    }
                }
                else
                {
                    return false; // User chose not to save
                }
            }

            return true;
        }

        // Checks if the order is empty before payment
        private bool IsOrderEmpty()
        {
            if (orderService.IsOrderEmpty(orderedItems))
            {
                MessageBox.Show("Cannot process payment for an empty order.",
                    "Empty Order", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return true;
            }
            return false;
        }

        // Opens the payment form and processes payment
        private void ProcessPayment()
        {
            try
            {
                PaymentForm paymentForm = new PaymentForm(currentOrderId.Value, loggedInEmployee);

                if (paymentForm.ShowDialog() == DialogResult.OK)
                {
                    MessageBox.Show("Payment processed successfully!",
                        "Payment Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error processing payment: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Lists all new order items in the ListView
        private void ListNewOrders()
        {
            try
            {
                SetupOrderListView();

                lvOrderItems.Items.Clear();

                foreach (OrderItem item in newOrderItems)
                {
                    lvOrderItems.Items.Add(CreateOrderListViewItem(item));
                }

                lvOrderItems.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);

                UpdateOrderActionButtons();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error listing new orders: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Sets up columns for the new order items ListView
        private void SetupOrderListView()
        {
            try
            {
                lvOrderItems.Columns.Clear();
                lvOrderItems.FullRowSelect = true;
                lvOrderItems.View = View.Details;

                lvOrderItems.Columns.Add("Item");
                lvOrderItems.Columns.Add("Qty");
                lvOrderItems.Columns.Add("Price");
                lvOrderItems.Columns.Add("Subtotal");
                lvOrderItems.Columns.Add("Status");
                lvOrderItems.Columns.Add("Comment");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error setting up order list view: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Creates a ListViewItem for a new order item
        private ListViewItem CreateOrderListViewItem(OrderItem item)
        {
            try
            {
                string itemName = orderService.GetItemName(item);
                string quantity = item.Quantity.ToString();
                string price = $"€{orderService.GetItemPrice(item):0.00}";
                string subtotal = $"€{orderService.CalculateItemSubtotal(item):0.00}";
                string status = orderService.GetItemStatus(item);
                string comment = item.Comment ?? string.Empty;

                ListViewItem lvi = new ListViewItem(itemName);
                lvi.SubItems.Add(quantity);
                lvi.SubItems.Add(price);
                lvi.SubItems.Add(subtotal);
                lvi.SubItems.Add(status);
                lvi.SubItems.Add(comment);

                lvi.Tag = item;

                return lvi;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error creating order list view item: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return new ListViewItem("Error loading item");
            }
        }

        // Enables/disables order action buttons based on whether there are items
        private void UpdateOrderActionButtons()
        {
            try
            {
                bool hasItems = newOrderItems.Count > 0;

                btnRemoveItem.Enabled = hasItems;
                btnEditItem.Enabled = hasItems;
                btnSaveOrder.Enabled = hasItems;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating order action buttons: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Loads all ordered items (from DB) into the ListView
        private void LoadOrderedItems()
        {
            try
            {
                if (orderedList == null)
                {
                    Console.WriteLine("ListView not initialized");
                    return;
                }

                SetupOrderedListView();

                decimal orderTotal = orderService.CalculateOrderTotal(orderedItems);

                foreach (OrderItem item in orderedItems)
                {
                    orderedList.Items.Add(CreateOrderedListViewItem(item));
                }

                UpdateOrderTotalLabel(orderTotal);

                orderedList.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in LoadOrderedItems: {ex.Message}");
                MessageBox.Show($"Error refreshing order items: {ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Sets up columns for the ordered items ListView
        private void SetupOrderedListView()
        {
            try
            {
                orderedList.Columns.Clear();
                orderedList.FullRowSelect = true;
                orderedList.View = View.Details;

                orderedList.Columns.Add("Item");
                orderedList.Columns.Add("Qty");
                orderedList.Columns.Add("Price");
                orderedList.Columns.Add("Subtotal");
                orderedList.Columns.Add("Status");
                orderedList.Columns.Add("Comment");

                orderedList.Items.Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error setting up ordered list view: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Creates a ListViewItem for an ordered item
        private ListViewItem CreateOrderedListViewItem(OrderItem item)
        {
            try
            {
                string itemName = orderService.GetItemName(item);
                string quantity = item.Quantity.ToString();
                string price = $"€{orderService.GetItemPrice(item):0.00}";
                string subtotal = $"€{orderService.CalculateItemSubtotal(item):0.00}";
                string status = orderService.GetItemStatus(item);
                string comment = item.Comment ?? string.Empty;

                ListViewItem lvi = new ListViewItem(itemName);
                lvi.SubItems.Add(quantity);
                lvi.SubItems.Add(price);
                lvi.SubItems.Add(subtotal);
                lvi.SubItems.Add(status);
                lvi.SubItems.Add(comment);

                lvi.Tag = item;

                return lvi;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error creating ordered list view item: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return new ListViewItem("Error loading item");
            }
        }

        // Updates the order total label in the UI
        private void UpdateOrderTotalLabel(decimal total)
        {
            try
            {
                if (lblOrderTotal != null)
                {
                    lblOrderTotal.Text = $"Order Total: €{total:0.00}";
                    lblOrderTotal.Visible = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating order total label: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Updates the payment button state based on order status
        private void UpdatePaymentButtonState()
        {
            try
            {
                if (orderedItems == null || orderedItems.Count == 0)
                {
                    btnPayment.Enabled = false;
                    btnPayment.Text = "No Items";
                    btnPayment.BackColor = Color.Gray;
                    return;
                }

                bool allServed = orderService.AreAllItemsServed(orderedItems);

                btnPayment.Enabled = allServed;

                if (allServed)
                {
                    btnPayment.Text = "Payment";
                    btnPayment.BackColor = Color.LightBlue;
                }
                else
                {
                    btnPayment.Text = "Serve items first";
                    btnPayment.BackColor = Color.Gray;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating payment button state: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}