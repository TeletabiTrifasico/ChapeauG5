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
        private List<OrderItem> orderItems; // This will be our temporary in-memory list
        private bool isExistingOrder = false;
        
        public OrderView(Employee employee, Table table)
        {
            InitializeComponent();
            loggedInEmployee = employee;
            selectedTable = table;
            menuService = new MenuService();
            orderService = new OrderService();
            tableService = new TableService();
            orderItems = new List<OrderItem>();
        }
        
        private void OrderView_Load(object sender, EventArgs e)
        {
            lblTable.Text = $"Table {selectedTable.TableNumber}";
            
            // Don't mark as occupied yet - only when order is confirmed
            
            // Load menu categories
            LoadMenuCategories();
            
            // Check if there's an existing order for this table
            Order existingOrder = orderService.GetOrderByTableId(selectedTable.TableId);
            
            if (existingOrder != null)
            {
                // Load existing order
                currentOrderId = existingOrder.OrderId;
                isExistingOrder = true;
                
                // Load existing items from database
                orderItems = orderService.GetOrderItemsByOrderId(existingOrder.OrderId);
                RefreshOrderItemsView();
                
                // If it's an existing order, set the table as occupied
                tableService.UpdateTableStatus(selectedTable.TableId, TableStatus.Occupied);
            }
            // Don't create a new order yet - wait for confirmation
            
            // Initially disable the payment button until all items are served
            UpdatePaymentButtonState();
        }
        
        private void LoadMenuCategories()
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
        
        private void cmbCategory_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbCategory.SelectedItem is MenuCategory selectedCategory)
            {
                // Load menu items for the selected category
                List<MenuItem> menuItems = menuService.GetMenuItemsByCategory(selectedCategory.CategoryId);
                
                lvMenuItems.Items.Clear();
                
                foreach (MenuItem item in menuItems)
                {
                    ListViewItem lvi = new ListViewItem(item.Name);
                    lvi.SubItems.Add($"€{item.Price:0.00}");
                    lvi.SubItems.Add(item.Description);
                    lvi.Tag = item;
                    
                    lvMenuItems.Items.Add(lvi);
                }
            }
        }
        
        private void btnAddToOrder_Click(object sender, EventArgs e)
        {
            if (lvMenuItems.SelectedItems.Count == 0)
            {
                MessageBox.Show("Please select a menu item first.", "No Item Selected", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            
            MenuItem selectedItem = (MenuItem)lvMenuItems.SelectedItems[0].Tag;
            int quantity = (int)nudQuantity.Value;
            string comment = txtComment.Text;
            
            if (quantity <= 0)
            {
                MessageBox.Show("Please enter a valid quantity.", "Invalid Quantity", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            
            // Create a local OrderItem object but don't save to database yet
            OrderItem newItem = new OrderItem
            {
                OrderId = new Order { OrderId = currentOrderId ?? 0 }, // Use 0 as a temporary ID if no order exists yet
                MenuItemId = selectedItem,
                Quantity = quantity,
                Comment = comment,
                CreatedAt = DateTime.Now,
                Status = OrderItem.OrderStatus.Ordered
            };
            
            // Add to our local list
            orderItems.Add(newItem);
            
            // Refresh the display only
            RefreshOrderItemsView();
            
            // Clear inputs
            nudQuantity.Value = 1;
            txtComment.Text = string.Empty;
            lvMenuItems.SelectedItems.Clear();
            
            // Ensure Save button is visible
            btnSaveOrder.Visible = true;
            
            MessageBox.Show($"{quantity}x {selectedItem.Name} added to order.", "Item Added", 
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        
        private void RefreshOrderItemsView()
        {
            try
            {
                // Ensure ListView is initialized
                if (lvOrderItems == null)
                {
                    Console.WriteLine("ListView not initialized");
                    return;
                }
                
                lvOrderItems.Items.Clear();
                decimal orderTotal = 0;
                
                foreach (OrderItem item in orderItems)
                {
                    // Create a safer way to access these properties
                    string itemName = GetItemName(item);
                    string quantity = item.Quantity.ToString();
                    string status = GetItemStatus(item);
                    //string comment = item.Comment ?? string.Empty;

                    ListViewItem lvi = new ListViewItem(itemName);
                    lvi.SubItems.Add(quantity);
                    lvi.SubItems.Add(status);
                    //lvi.SubItems.Add(item.Comment ?? string.Empty); // Optional

                    lvi.Tag = item;
                    lvOrderItems.Items.Add(lvi);

                }
                
                // Update the order total
                if (lblOrderTotal != null)
                {
                    lblOrderTotal.Text = $"Order Total: €{orderTotal:0.00}";
                    lblOrderTotal.Visible = true;
                }
                
                // Enable/disable buttons based on whether we have items
                btnRemoveItem.Enabled = orderItems.Count > 0;
                btnEditItem.Enabled = orderItems.Count > 0;
                btnSaveOrder.Enabled = orderItems.Count > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in RefreshOrderItemsView: {ex.Message}");
                MessageBox.Show($"Error refreshing order items: {ex.Message}", 
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            
            // Enable/disable Mark as Served button if we have items
            btnMarkServed.Enabled = orderItems.Count > 0 && isExistingOrder;
            
            // Update payment button state
            UpdatePaymentButtonState();
        }
        
        // Add this method to save the entire order at once
        private void btnSaveOrder_Click(object sender, EventArgs e)
        {
            try
            {
                if (orderItems.Count == 0)
                {
                    MessageBox.Show("Please add at least one item to the order.", 
                        "Empty Order", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                
                // If this is a new order, create it first
                if (!currentOrderId.HasValue)
                {
                    // Create a new order
                    int newOrderId = orderService.CreateOrder(selectedTable.TableId, loggedInEmployee.EmployeeId);
                    currentOrderId = newOrderId;
                    
                    // Mark the table as occupied
                    tableService.UpdateTableStatus(selectedTable.TableId, TableStatus.Occupied);
                }
                
                // Now save each item
                foreach (OrderItem item in orderItems)
                {
                    // Update the order ID in case we just created it
                    item.OrderId = new Order { OrderId = currentOrderId.Value };
                    
                    // Only save items that don't have an OrderItemId yet (new items)
                    if (item.OrderItemId == 0)
                    {
                        orderService.AddOrderItem(currentOrderId.Value, 
                                                 item.MenuItemId.MenuItemId, 
                                                 item.Quantity, 
                                                 item.Comment);
                    }
                }
                
                isExistingOrder = true;
                MessageBox.Show("Order saved successfully!", "Success", 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                
                // Refresh the order from the database with full MenuItem details
                orderItems = orderService.GetOrderItemsByOrderId(currentOrderId.Value);
                RefreshOrderItemsView();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving order: {ex.Message}", 
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        
        // Add this method to remove items from the order
        private void btnRemoveItem_Click(object sender, EventArgs e)
        {
            if (lvOrderItems.SelectedItems.Count == 0)
            {
                MessageBox.Show("Please select an item to remove.", 
                    "No Item Selected", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            
            OrderItem selectedItem = (OrderItem)lvOrderItems.SelectedItems[0].Tag;
            
            // If it's an existing order with items already in the database, we should warn the user
            if (isExistingOrder && selectedItem.OrderItemId != 0)
            {
                DialogResult result = MessageBox.Show(
                    "This item is already saved in the database. Are you sure you want to remove it?",
                    "Confirm Removal",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);
                    
                if (result == DialogResult.No)
                {
                    return;
                }
                
                // Here we would need to implement a delete method in the OrderDao
                // For now, we'll just remove it locally
            }
            
            // Remove from our local list
            orderItems.Remove(selectedItem);
            
            // Refresh the display
            RefreshOrderItemsView();
        }
        
        // Add this method to edit items in the order
        private void btnEditItem_Click(object sender, EventArgs e)
        {
            if (lvOrderItems.SelectedItems.Count == 0)
            {
                MessageBox.Show("Please select an item to edit.", 
                    "No Item Selected", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            
            OrderItem selectedItem = (OrderItem)lvOrderItems.SelectedItems[0].Tag;
            
            // Create a simple dialog to edit quantity and comment
            using (Form editForm = new Form())
            {
                editForm.Text = "Edit Order Item";
                editForm.Size = new Size(300, 200);
                editForm.StartPosition = FormStartPosition.CenterParent;
                editForm.FormBorderStyle = FormBorderStyle.FixedDialog;
                editForm.MaximizeBox = false;
                editForm.MinimizeBox = false;
                
                Label lblQuantity = new Label();
                lblQuantity.Text = "Quantity:";
                lblQuantity.Location = new Point(20, 20);
                lblQuantity.Size = new Size(70, 20);
                
                NumericUpDown nudEditQuantity = new NumericUpDown();
                nudEditQuantity.Minimum = 1;
                nudEditQuantity.Maximum = 20;
                nudEditQuantity.Value = selectedItem.Quantity;
                nudEditQuantity.Location = new Point(100, 20);
                nudEditQuantity.Size = new Size(60, 20);
                
                Label lblComment = new Label();
                lblComment.Text = "Comment:";
                lblComment.Location = new Point(20, 50);
                lblComment.Size = new Size(70, 20);
                
                TextBox txtEditComment = new TextBox();
                txtEditComment.Text = selectedItem.Comment;
                txtEditComment.Location = new Point(100, 50);
                txtEditComment.Size = new Size(170, 20);
                
                Button btnSave = new Button();
                btnSave.Text = "Save";
                btnSave.DialogResult = DialogResult.OK;
                btnSave.Location = new Point(100, 90);
                btnSave.Size = new Size(80, 30);
                
                Button btnCancel = new Button();
                btnCancel.Text = "Cancel";
                btnCancel.DialogResult = DialogResult.Cancel;
                btnCancel.Location = new Point(190, 90);
                btnCancel.Size = new Size(80, 30);
                
                editForm.Controls.Add(lblQuantity);
                editForm.Controls.Add(nudEditQuantity);
                editForm.Controls.Add(lblComment);
                editForm.Controls.Add(txtEditComment);
                editForm.Controls.Add(btnSave);
                editForm.Controls.Add(btnCancel);
                
                editForm.AcceptButton = btnSave;
                editForm.CancelButton = btnCancel;
                
                if (editForm.ShowDialog() == DialogResult.OK)
                {
                    int newQuantity = (int)nudEditQuantity.Value;
                    string newComment = txtEditComment.Text;
                    
                    // Update the selected item's properties
                    selectedItem.Quantity = newQuantity;
                    selectedItem.Comment = newComment;
                    
                    // If it's an existing order with items already in the database
                    if (isExistingOrder && selectedItem.OrderItemId != 0)
                    {
                        try
                        {
                            orderService.UpdateOrderItem(selectedItem.OrderItemId, newQuantity, newComment);
                            MessageBox.Show("Order item updated successfully.", 
                                "Item Updated", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"Error updating order item: {ex.Message}", 
                                "Update Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    
                    RefreshOrderItemsView();
                }
            }
        }
        
        // Helper methods to safely access OrderItem properties
        private string GetItemName(OrderItem item)
        {
            return item?.MenuItemId?.Name ?? "Unknown Item";
        }
        
        private decimal GetItemPrice(OrderItem item)
        {
            return item?.MenuItemId?.Price ?? 0;
        }
        
        private string GetItemStatus(OrderItem item)
        {
            return item?.Status.ToString() ?? "Unknown";
        }
        
        private void btnCancel_Click(object sender, EventArgs e)
        {
            try {
                // Only ask for confirmation if there are unsaved changes
                if (orderItems.Count > 0 && !isExistingOrder)
                {
                    DialogResult result = MessageBox.Show(
                        "Are you sure you want to close this order view? All unsaved changes will be lost.",
                        "Confirm Close",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question) ;
                        
                    if (result == DialogResult.No)
                    {
                        return;
                    }
                }
                
                // If we created a table but never added items to it, we should make it available again
                if (!isExistingOrder)
                {
                    tableService.UpdateTableStatus(selectedTable.TableId, TableStatus.Free);
                }
            }
            catch (Exception ex) {
                Console.WriteLine($"Error resetting table status: {ex.Message}");
                MessageBox.Show($"Could not reset table status: {ex.Message}", "Warning", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            
            // Close the form
            this.Close();
        }

        // Add this new method for the button click event

        private void btnMarkServed_Click(object sender, EventArgs e)
        {
            if (lvOrderItems.SelectedItems.Count == 0)
            {
                MessageBox.Show("Please select an item to mark as served.", 
                    "No Item Selected", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            
            OrderItem selectedItem = (OrderItem)lvOrderItems.SelectedItems[0].Tag;
            
            // Check if the item is already served
            if (selectedItem.Status == OrderItem.OrderStatus.Served)
            {
                MessageBox.Show("This item has already been served.", 
                    "Already Served", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            
            // Check if it's an existing order with items already in the database
            if (!isExistingOrder || selectedItem.OrderItemId == 0)
            {
                MessageBox.Show("You need to save the order before marking items as served.", 
                    "Save Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            
            try
            {
                // Update the item status in the database
                orderService.MarkOrderItemAsServed(selectedItem.OrderItemId);
                
                // Update the local item status
                selectedItem.Status = OrderItem.OrderStatus.Served;
                
                // Refresh the display
                RefreshOrderItemsView();
                
                // Update Payment button state
                UpdatePaymentButtonState();
                
                MessageBox.Show("Item marked as served.", "Status Updated", 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating item status: {ex.Message}", 
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Add this method to check if all items are served and update the Payment button state
        private void UpdatePaymentButtonState()
        {
            bool allServed = true;
            
            foreach (OrderItem item in orderItems)
            {
                if (item.Status != OrderItem.OrderStatus.Served)
                {
                    allServed = false;
                    break;
                }
            }
            
            btnPayment.Enabled = allServed && orderItems.Count > 0;
            
            // Set tooltip or label to indicate why payment might be disabled
            if (!allServed && orderItems.Count > 0)
            {
                btnPayment.Text = "Serve items first";
                btnPayment.BackColor = Color.Gray;
            }
            else if (orderItems.Count > 0)
            {
                btnPayment.Text = "Payment";
                btnPayment.BackColor = Color.LightBlue;
            }
        }
    }
}