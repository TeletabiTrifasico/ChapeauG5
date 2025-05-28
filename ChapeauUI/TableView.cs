using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using ChapeauModel;
using ChapeauService;

namespace ChapeauG5
{
    public partial class TableView : Form
    {
        private TableService tableService;
        private Employee loggedInEmployee;
        private List<Button> tableButtons;
        
        public TableView(Employee employee)
        {
            InitializeComponent();
            loggedInEmployee = employee;
            tableService = new TableService();
            tableButtons = new List<Button>();
            
            lblWelcome.Text = $"Welcome, {employee.FirstName}!";
        }
        
        private void TableView_Load(object sender, EventArgs e)
        {
            RefreshTables();
        }
        
        private void RefreshTables()
        {
            tlpTables.Controls.Clear();
            tableButtons.Clear();
            
            List<Table> tables = tableService.GetAllTables();
            
            // Calculate positions to create a 5x2 grid
            int maxTables = Math.Min(tables.Count, 10); // Maximum of 10 tables in a 5x2 grid
            
            for (int i = 0; i < maxTables; i++)
            {
                Table table = tables[i];
                
                Button btnTable = new Button();
                btnTable.Text = $"Table {table.TableNumber}";
                btnTable.Size = new Size(100, 100); // Consistent square size
                btnTable.Tag = table;
                
                // Use Anchor instead of Dock for centering
                btnTable.Dock = DockStyle.None;
                btnTable.Anchor = AnchorStyles.None; // This centers the button in its cell
                btnTable.FlatStyle = FlatStyle.Flat; // Keep flat styleyle
                btnTable.FlatAppearance.BorderSize = 0; // Remove border completelyy
                
                // Style the button based on the table status
                if (table.Status == TableStatus.Available)
                {
                    btnTable.BackColor = Color.LightGreen;
                    // No border color needed since border size is 0
                }
                else if (table.Status == TableStatus.Occupied)
                {
                    btnTable.BackColor = Color.LightCoral;
                    // No border color needed since border size is 0
                }
                
                btnTable.Click += BtnTable_Click;
                tableButtons.Add(btnTable);
                
                // Calculate row and column for the 5x2 grid
                int row = i / 5;
                int col = i % 5;
                
                tlpTables.Controls.Add(btnTable, col, row);
            }
        }
        
        // Add this check before trying to instantiate OrderView
        private bool IsOrderViewImplemented()
        {
            // Check if the OrderView type exists and can be instantiated
            try
            {
                Type orderViewType = typeof(OrderView);
                return true;
            }
            catch
            {
                return false;
            }
        }
        
        private void BtnTable_Click(object sender, EventArgs e)
        {
            try
            {
                Button clickedButton = (Button)sender;
                Table selectedTable = (Table)clickedButton.Tag;
                
                if (IsOrderViewImplemented())
                {
                    // OrderView exists, try to open it
                    OrderView orderView = new OrderView(loggedInEmployee, selectedTable);
                    orderView.FormClosed += (s, args) => RefreshTables();
                    orderView.Show();
                }
                else
                {
                    // OrderView doesn't exist yet, show a message
                    MessageBox.Show($"Table {selectedTable.TableNumber} selected. OrderView form is not implemented yet.", 
                        "Development in Progress", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        
        private void btnLogout_Click(object sender, EventArgs e)
        {
            this.Close();
            Application.Restart();
        }
        
        private Image GetRefreshIcon()
        {
            // Use a system icon or a custom one if available
            try
            {
                // Try to use a standard system icon
                return SystemIcons.Information.ToBitmap();
            }
            catch
            {
                // Return null if icon can't be loaded
                return null;
            }
        }
    }
}