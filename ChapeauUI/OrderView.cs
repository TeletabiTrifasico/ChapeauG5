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
            if (item?.MenuItemId == null)
                return "Unknown Item";
                
            // Try to get the name from the property, or use a placeholder
            return item.MenuItemId.Name ?? "Unnamed Item";
        }

        private decimal GetItemPrice(OrderItem item)
        {
            if (item?.MenuItemId == null)
                return 0;
                
            return item.MenuItemId.Price;
        }
        
        private string GetItemStatus(OrderItem item)
        {
            return item.Status.ToString();
        }
        
        private void btnPayment_Click(object sender, EventArgs e)
        {
            // Implementation for payment processing
        }
        
        private void btnCancel_Click(object sender, EventArgs e)
        {
            try {
                // Ask for confirmation if the order has items
                if (orderItems.Count > 0)
                {
                    DialogResult result = MessageBox.Show(
                        "Are you sure you want to close this order view? All unsaved changes will be lost.",
                        "Confirm Close",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question);
                        
                    if (result == DialogResult.No)
                    {
                        return;
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
        
        private void OrderView_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Any cleanup code needed when the form closes
        }
    }
}