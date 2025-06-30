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
        private Employee loggedInEmployee;
        private Table selectedTable;
        private MenuService menuService;
        private OrderService orderService;
        private TableService tableService;
        private int? currentOrderId; // Make nullable since we might not have an order yet
        private List<OrderItem> orderedItems;
        private List<OrderItem> newOrderItems;
        private bool isExistingOrder = false;
        
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
        
        // Loading the page
        private void OrderView_Load(object sender, EventArgs e)
        {
            try
            {
                lblTable.Text = $"Table {selectedTable.TableNumber}";

                LoadMenuCategories();
                
                Order existingOrder = orderService.GetOrderByTableId(selectedTable.TableId);
                
                if (existingOrder != null)
                {
                    currentOrderId = existingOrder.OrderId;
                    isExistingOrder = true;
                    
                    orderedItems = orderService.GetOrderItemsByOrderId(existingOrder.OrderId);
                    LoadOrderedItems();
                    
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


        // Loading the menu categories
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


        // Loading the menu items based on selected category
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

        // Adding item to order
        private void btnAddToOrder_Click(object sender, EventArgs e)
        {
            try
            {
                if (!ValidateSelection(out MenuItem selectedItem, out int quantity, out string comment)) return;

                var existingItem = orderService.FindExistingOrderItem(newOrderItems, selectedItem, comment);

                if (existingItem != null)
                {
                    orderService.UpdateOrderItemQuantity(existingItem, quantity);
                }
                else
                {
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
        private void CreateNewOrder()
        {
            currentOrderId = orderService.CreateOrder(selectedTable.TableId, loggedInEmployee.EmployeeId);
            tableService.UpdateTableStatus(selectedTable.TableId, TableStatus.Occupied);
        }
        private void SaveOrderItems()
        {
            orderService.SaveOrderItems(currentOrderId.Value, newOrderItems);
        }
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

        // Removing item from order
        private void btnRemoveItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (!ValidateItemSelection(out OrderItem selectedItem)) return;

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

        // Editing an item in the order
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

        // Canceling the order view
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
        private bool ShouldResetTable()
        {
            return !isExistingOrder;
        }
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

        // Marking an item as served
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

        // Payment processing
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







        // Listing new added rders
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

        // Loading ordered items
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

        // Updating the payment button state
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