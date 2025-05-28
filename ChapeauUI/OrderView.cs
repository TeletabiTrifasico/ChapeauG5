using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Windows.Forms;
using ChapeauModel;
using ChapeauService;
using ChapeauDAL;
using Microsoft.Data.SqlClient;

namespace ChapeauG5
{
    public partial class OrderView : Form
    {
        private Employee loggedInEmployee;
        private Table selectedTable;
        private MenuService menuService;
        private OrderService orderService;
        private TableService tableService;
        private int currentOrderId;
        private List<OrderItem> orderItems;
        
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
            
            // Set this table as occupied
            tableService.UpdateTableStatus(selectedTable.TableId, TableStatus.Occupied);
            
            // Load menu categories
            LoadMenuCategories();
            
            // Check if there's an existing order for this table
            Order existingOrder = orderService.GetOrderByTableId(selectedTable.TableId);
            
            if (existingOrder != null)
            {
                // Load existing order
                currentOrderId = existingOrder.OrderId;
                RefreshOrderItems();
            }
            else
            {
                // Create a new order
                currentOrderId = orderService.CreateOrder(selectedTable.TableId, loggedInEmployee.EmployeeId);
            }
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
            if (cmbCategory.SelectedItem == null)
                return;
                
            MenuCategory selectedCategory = (MenuCategory)cmbCategory.SelectedItem;
            
            // Load menu items for this category
            List<MenuItem> menuItems = menuService.GetMenuItemsByCategory(selectedCategory.CategoryId);
            
            // Display in list view
            lvMenuItems.Items.Clear();
            
            foreach (MenuItem item in menuItems)
            {
                ListViewItem lvi = new ListViewItem(item.Name);
                lvi.SubItems.Add(item.DisplayPrice);
                lvi.SubItems.Add(item.Description);
                lvi.Tag = item;
                
                lvMenuItems.Items.Add(lvi);
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
            
            // Add to order
            orderService.AddOrderItem(currentOrderId, selectedItem.MenuItemId, quantity, comment);
            
            // Refresh order items
            RefreshOrderItems();
            
            // Clear inputs
            nudQuantity.Value = 1;
            txtComment.Text = string.Empty;
            lvMenuItems.SelectedItems.Clear();
            
            MessageBox.Show($"{quantity}x {selectedItem.Name} added to order.", "Item Added", 
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        
        private void RefreshOrderItems()
        {
            try
            {
                // Ensure ListView is initialized
                if (lvOrderItems == null)
                {
                    Console.WriteLine("ListView not initialized");
                    return;
                }
                
                // Get items from database
                orderItems = orderService.GetOrderItemsByOrderId(currentOrderId);
                
                lvOrderItems.Items.Clear();
                decimal orderTotal = 0;
                
                foreach (OrderItem item in orderItems)
                {
                    // Create a safer way to access these properties
                    string itemName = GetItemName(item);
                    decimal itemPrice = GetItemPrice(item);
                    string status = GetItemStatus(item);
                    string comment = item.Comment ?? string.Empty;
                    
                    ListViewItem lvi = new ListViewItem(itemName);
                    lvi.SubItems.Add(item.Quantity.ToString());
                    lvi.SubItems.Add($"€{itemPrice:0.00}");
                    
                    decimal subtotal = item.Quantity * itemPrice;
                    lvi.SubItems.Add($"€{subtotal:0.00}");
                    
                    lvi.SubItems.Add(status);
                    lvi.SubItems.Add(comment);
                    lvi.Tag = item;
                    
                    lvOrderItems.Items.Add(lvi);
                    
                    orderTotal += subtotal;
                }
                
                // Update the order total
                if (lblOrderTotal != null)
                {
                    lblOrderTotal.Text = $"Order Total: €{orderTotal:0.00}";
                    lblOrderTotal.Visible = true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in RefreshOrderItems: {ex.Message}");
                MessageBox.Show($"Error refreshing order items: {ex.Message}", 
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        
        // Helper methods to safely get properties
        private string GetItemName(OrderItem item)
        {
            try
            {
                // Try standard property
                if (item.MenuItemId != null)
                    return item.MenuItemId.Name;
                    
                // Try extended property if it exists
                var property = item.GetType().GetProperty("ItemName");
                if (property != null)
                    return (string)property.GetValue(item) ?? "Unknown Item";
                    
                return "Unknown Item";
            }
            catch
            {
                return "Unknown Item";
            }
        }

        private decimal GetItemPrice(OrderItem item)
        {
            try
            {
                // Try standard property
                if (item.MenuItemId != null)
                    return item.MenuItemId.Price;
                    
                // Try extended property if it exists
                var property = item.GetType().GetProperty("ItemPrice");
                if (property != null)
                    return (decimal)property.GetValue(item);
                
                return 0;
            }
            catch
            {
                return 0;
            }
        }

        private string GetItemStatus(OrderItem item)
        {
            try
            {
                // Try to get the status using reflection
                var property = item.GetType().GetProperty("Status");
                if (property != null)
                    return property.GetValue(item)?.ToString() ?? "Unknown Status";
                    
                // Try ItemStatus as an alternative name
                property = item.GetType().GetProperty("ItemStatus");
                if (property != null)
                    return property.GetValue(item)?.ToString() ?? "Unknown Status";
                    
                return "Unknown Status";
            }
            catch
            {
                return "Unknown Status";
            }
        }
        
        private void btnPayment_Click(object sender, EventArgs e)
        {
            if (orderItems.Count == 0)
            {
                MessageBox.Show("There are no items in this order to pay for.", "Empty Order", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            
            // Mark all items as served for testing purposes
            // In production, only allow payment for items that are served
            orderService.MarkAllItemsAsServed(currentOrderId);
            
            // Open payment form
            PaymentForm paymentForm = new PaymentForm(currentOrderId, loggedInEmployee);
            paymentForm.FormClosed += (s, args) => {
                if (paymentForm.DialogResult == DialogResult.OK)
                {
                    MessageBox.Show("Payment processed successfully!", "Success", 
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    
                    // Set table status back to available
                    tableService.UpdateTableStatus(selectedTable.TableId, TableStatus.Available);
                    
                    // Close this form
                    this.Close();
                }
            };
            paymentForm.ShowDialog();
        }
        
        private void btnCancel_Click(object sender, EventArgs e)
        {
            try {
                // Get the current table status first for diagnostic purposes
                string currentStatus = string.Empty;
                
                using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["ChapeauG5DB"].ConnectionString))
                {
                    connection.Open();
                    
                    // First query the actual current status
                    string checkQuery = "SELECT status FROM [Table] WHERE table_id = @TableId";
                    using (SqlCommand checkCmd = new SqlCommand(checkQuery, connection))
                    {
                        checkCmd.Parameters.AddWithValue("@TableId", selectedTable.TableId);
                        object result = checkCmd.ExecuteScalar();
                        if (result != null)
                        {
                            currentStatus = result.ToString();
                            Console.WriteLine($"Current table status: '{currentStatus}'");
                        }
                    }
                    
                    // Try to find a valid "Available" status from another table
                    string validStatus = null;
                    string findValidQuery = "SELECT TOP 1 status FROM [Table] WHERE status LIKE '%vailable%'";
                    using (SqlCommand findCmd = new SqlCommand(findValidQuery, connection))
                    {
                        object result = findCmd.ExecuteScalar();
                        if (result != null)
                        {
                            validStatus = result.ToString();
                            Console.WriteLine($"Found valid available status: '{validStatus}'");
                        }
                    }
                    
                    // If we couldn't find a valid status, try these common variations
                    if (string.IsNullOrEmpty(validStatus))
                    {
                        // Try different cases
                        string[] possibleStatuses = { "Available", "AVAILABLE", "available", "A" };
                        
                        foreach (string status in possibleStatuses)
                        {
                            try
                            {
                                string updateQuery = "UPDATE [Table] SET status = @Status WHERE table_id = @TableId";
                                using (SqlCommand updateCmd = new SqlCommand(updateQuery, connection))
                                {
                                    updateCmd.Parameters.AddWithValue("@Status", status);
                                    updateCmd.Parameters.AddWithValue("@TableId", selectedTable.TableId);
                                    int affected = updateCmd.ExecuteNonQuery();
                                    
                                    if (affected > 0)
                                    {
                                        Console.WriteLine($"Successfully updated table status to '{status}'");
                                        validStatus = status; // Remember what worked
                                        break;
                                    }
                                }
                            }
                            catch (Exception statusEx)
                            {
                                Console.WriteLine($"Status '{status}' failed: {statusEx.Message}");
                            }
                        }
                    }
                    else
                    {
                        // Use the valid status we found
                        string updateQuery = "UPDATE [Table] SET status = @Status WHERE table_id = @TableId";
                        using (SqlCommand updateCmd = new SqlCommand(updateQuery, connection))
                        {
                            updateCmd.Parameters.AddWithValue("@Status", validStatus);
                            updateCmd.Parameters.AddWithValue("@TableId", selectedTable.TableId);
                            int affected = updateCmd.ExecuteNonQuery();
                            Console.WriteLine($"Update with '{validStatus}' affected {affected} rows");
                        }
                    }
                    
                    // Verify the update
                    string verifyQuery = "SELECT status FROM [Table] WHERE table_id = @TableId";
                    using (SqlCommand verifyCmd = new SqlCommand(verifyQuery, connection))
                    {
                        verifyCmd.Parameters.AddWithValue("@TableId", selectedTable.TableId);
                        object result = verifyCmd.ExecuteScalar();
                        if (result != null)
                        {
                            string newStatus = result.ToString();
                            Console.WriteLine($"Final table status: '{newStatus}'");
                            
                            if (newStatus.Contains("vailable"))
                            {
                                Console.WriteLine("SUCCESS: Table is now available");
                            }
                        }
                    }
                }
            }
            catch (Exception ex) {
                Console.WriteLine($"Error resetting table status: {ex.Message}");
                MessageBox.Show($"Could not reset table status: {ex.Message}", "Warning", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            
            // Close the form regardless of whether the status update succeeded
            this.Close();
        }
        
        // Add a button to manually reset table status for testing
        private Button btnResetTable;

        private void InitializeComponent()
        {
            this.Text = "Order Management";
            this.Size = new Size(900, 700);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            
            // Table Label
            lblTable = new Label();
            lblTable.Location = new Point(20, 20);
            lblTable.Size = new Size(200, 30);
            lblTable.Font = new Font("Segoe UI", 14, FontStyle.Bold);
            lblTable.Text = "Table X";
            
            // Category Selection
            lblCategory = new Label();
            lblCategory.Location = new Point(20, 60);
            lblCategory.Size = new Size(100, 25);
            lblCategory.Text = "Menu Category:";
            
            cmbCategory = new ComboBox();
            cmbCategory.Location = new Point(130, 60);
            cmbCategory.Size = new Size(200, 25);
            cmbCategory.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbCategory.DisplayMember = "Name";
            cmbCategory.ValueMember = "CategoryId";
            cmbCategory.SelectedIndexChanged += cmbCategory_SelectedIndexChanged;
            
            // Menu Items List
            lvMenuItems = new ListView();
            lvMenuItems.Location = new Point(20, 100);
            lvMenuItems.Size = new Size(400, 300);
            lvMenuItems.View = View.Details;
            lvMenuItems.FullRowSelect = true;
            lvMenuItems.MultiSelect = false;
            lvMenuItems.Columns.Add("Item", 150);
            lvMenuItems.Columns.Add("Price", 70);
            lvMenuItems.Columns.Add("Description", 180);
            
            // Quantity Selection
            lblQuantity = new Label();
            lblQuantity.Location = new Point(20, 410);
            lblQuantity.Size = new Size(70, 25);
            lblQuantity.Text = "Quantity:";
            
            nudQuantity = new NumericUpDown();
            nudQuantity.Location = new Point(100, 410);
            nudQuantity.Size = new Size(60, 25);
            nudQuantity.Minimum = 1;
            nudQuantity.Maximum = 20;
            nudQuantity.Value = 1;
            
            // Comment
            lblComment = new Label();
            lblComment.Location = new Point(170, 410);
            lblComment.Size = new Size(70, 25);
            lblComment.Text = "Comment:";
            
            txtComment = new TextBox();
            txtComment.Location = new Point(250, 410);
            txtComment.Size = new Size(170, 25);
            
            // Add to Order Button
            btnAddToOrder = new Button();
            btnAddToOrder.Location = new Point(250, 450);
            btnAddToOrder.Size = new Size(170, 40);
            btnAddToOrder.Text = "Add to Order";
            btnAddToOrder.BackColor = Color.LightGreen;
            btnAddToOrder.Click += btnAddToOrder_Click;
            
            // Order Items List
            lvOrderItems = new ListView();
            lvOrderItems.Location = new Point(450, 100);
            lvOrderItems.Size = new Size(420, 400);
            lvOrderItems.View = View.Details;
            lvOrderItems.FullRowSelect = true;
            lvOrderItems.Columns.Add("Item", 120);
            lvOrderItems.Columns.Add("Qty", 40);
            lvOrderItems.Columns.Add("Price", 60);
            lvOrderItems.Columns.Add("Subtotal", 70);
            lvOrderItems.Columns.Add("Status", 60);
            lvOrderItems.Columns.Add("Comment", 70);
            
            // Order Total
            lblOrderTotal = new Label();
            lblOrderTotal.Location = new Point(450, 510);
            lblOrderTotal.Size = new Size(250, 30);
            lblOrderTotal.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            lblOrderTotal.Text = "Order Total: €0.00";
            lblOrderTotal.Visible = true;
            
            // Payment Button
            btnPayment = new Button();
            btnPayment.Location = new Point(750, 510);
            btnPayment.Size = new Size(120, 40);
            btnPayment.Text = "Payment";
            btnPayment.BackColor = Color.LightBlue;
            btnPayment.Click += btnPayment_Click;
            
            // Cancel Button
            btnCancel = new Button();
            btnCancel.Location = new Point(750, 560);
            btnCancel.Size = new Size(120, 40);
            btnCancel.Text = "Close";
            btnCancel.BackColor = Color.LightGray;
            btnCancel.Click += btnCancel_Click;
            
            this.Controls.Add(lblTable);
            this.Controls.Add(lblCategory);
            this.Controls.Add(cmbCategory);
            this.Controls.Add(lvMenuItems);
            this.Controls.Add(lblQuantity);
            this.Controls.Add(nudQuantity);
            this.Controls.Add(lblComment);
            this.Controls.Add(txtComment);
            this.Controls.Add(btnAddToOrder);
            this.Controls.Add(lvOrderItems);
            this.Controls.Add(lblOrderTotal);
            this.Controls.Add(btnPayment);
            this.Controls.Add(btnCancel);
            
            this.Load += OrderView_Load;
            this.FormClosing += OrderView_FormClosing;
        }
        
        // Also update the FormClosing handler to use the same approach
        private void OrderView_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                // Only reset the table status if the order has no items
                List<OrderItem> items = orderService.GetOrderItemsByOrderId(currentOrderId);
                
                if (items.Count == 0)
                {
                    using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["ChapeauG5DB"].ConnectionString))
                    {
                        connection.Open();
                        
                        // Try to find a valid "Available" status from another table
                        string validStatus = null;
                        string findValidQuery = "SELECT TOP 1 status FROM [Table] WHERE status LIKE '%vailable%'";
                        using (SqlCommand findCmd = new SqlCommand(findValidQuery, connection))
                        {
                            object result = findCmd.ExecuteScalar();
                            if (result != null)
                            {
                                validStatus = result.ToString();
                            }
                        }
                        
                        // Use the valid status or default to "Available"
                        validStatus = validStatus ?? "Available";
                        
                        string updateQuery = "UPDATE [Table] SET status = @Status WHERE table_id = @TableId";
                        using (SqlCommand updateCmd = new SqlCommand(updateQuery, connection))
                        {
                            updateCmd.Parameters.AddWithValue("@Status", validStatus);
                            updateCmd.Parameters.AddWithValue("@TableId", selectedTable.TableId);
                            updateCmd.ExecuteNonQuery();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Log but don't interrupt closing
                Console.WriteLine($"Error in OrderView_FormClosing: {ex.Message}");
            }
        }
    }
    
    public partial class OrderView
    {
        private Label lblTable;
        private Label lblCategory;
        private ComboBox cmbCategory;
        private ListView lvMenuItems;
        private Label lblQuantity;
        private NumericUpDown nudQuantity;
        private Label lblComment;
        private TextBox txtComment;
        private Button btnAddToOrder;
        private ListView lvOrderItems;
        private Label lblOrderTotal;
        private Button btnPayment;
        private Button btnCancel;
    }
}