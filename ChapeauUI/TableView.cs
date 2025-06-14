using ChapeauG5.ChapeauUI;
using ChapeauModel;
using ChapeauService;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace ChapeauG5
{
    public partial class TableView : Form
    {
        private OrderService orderService;
        private TableService tableService;
        private Employee loggedInEmployee;
        private List<Button> tableButtons;
        
        public TableView(Employee employee)
        {
            InitializeComponent();

            if (employee == null)
            {
                MessageBox.Show("Employee is null in TableView constructor!");
            }

            loggedInEmployee = employee;
           

            tableService = new TableService();
            tableButtons = new List<Button>();
            orderService = new OrderService();

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
            List<TableOrderStatus> tableOrderStatuses = orderService.GetTableOrderStatuses();

            // Calculate positions to create a 5x2 grid
            int maxTables = Math.Min(tables.Count, 10); // Maximum of 10 tables in a 5x2 grid
            
            for (int i = 0; i < maxTables; i++)
            {
                Table table = tables[i];
                
                Button btnTable = new Button();
                btnTable.Text = $"Table {table.TableNumber}";
                btnTable.Size = new Size(200, 200); // Adjust size for better visibility
                btnTable.Tag = table;

                
                // Use Anchor instead of Dock for centering
                btnTable.Dock = DockStyle.None;
                btnTable.Anchor = AnchorStyles.None; // This centers the button in its cell
                btnTable.FlatStyle = FlatStyle.Flat; // Keep flat styleyle
                btnTable.FlatAppearance.BorderSize = 0; // Remove border completelyy
                
                // Style the button based on the table status
                if (table.Status == TableStatus.Free)
                {
                    btnTable.BackColor = Color.LightGreen;
                    // No border color needed since border size is 0
                }
                else if (table.Status == TableStatus.Occupied)
                {
                    btnTable.BackColor = Color.LightCoral;
                    // No border color needed since border size is 0
                }

                //----Order Status Info------

                TableOrderStatus status = tableOrderStatuses.Find(s => s.TableId == table.TableId);
                if (status != null && status.HasRunningOrder)
                {
                    string foodStatus = status.FoodOrderStatus ?? "-";
                    string drinkStatus = status.DrinkOrderStatus ?? "-";
                    btnTable.Text += $"\nFood: {foodStatus}\nDrink: {drinkStatus}";
                }
                else
                {
                    btnTable.Text += "\nNo active order";
                }
                //----------------

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

                // Order status of the table
                TableOrderStatus status = orderService.GetTableOrderStatuses()
                    .Find(s => s.TableId == selectedTable.TableId);

                bool hasOpenOrder = status != null && status.HasRunningOrder; // is_done == false

                if (selectedTable.Status == TableStatus.Free)
                {
                    // Open FreeTableManagement, Occcupt button active
                    var freeForm = new FreeTableManagement(selectedTable);
                    freeForm.ShowDialog();

                    RefreshTables(); // Refresh the table view after closing the form
                }
                else if (selectedTable.Status == TableStatus.Occupied)
                {
                    var occupiedForm = new OccupiedTableManagement(loggedInEmployee,selectedTable);

                    // Take Order button
                    occupiedForm.SetTakeOrderButtonEnabled(true);

                    // Free Table butonu durumu
                    occupiedForm.SetFreeTableButtonEnabled(!hasOpenOrder);
                    // if (hasOpenOrder=true), freeTable button is going to be disabled 

                    occupiedForm.ShowDialog();
                }
                else
                {
                    MessageBox.Show("Table status unknown.");
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