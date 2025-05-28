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
            tableService = new TableService();
            loggedInEmployee = employee;
            tableButtons = new List<Button>();
        }
        
        private void TableView_Load(object sender, EventArgs e)
        {
            lblWelcome.Text = $"Welcome, {loggedInEmployee.FirstName}!";
            RefreshTables();            
        }
        
        private void RefreshTables()
        {
            List<Table> tables = tableService.GetAllTables();
            
            // Clear existing buttons if any
            if (tableButtons.Count > 0)
            {
                foreach (Button btn in tableButtons)
                {
                    flpTables.Controls.Remove(btn);
                }
                tableButtons.Clear();
            }
            
            // Create table buttons
            foreach (Table table in tables)
            {
                Button btnTable = new Button();
                btnTable.Text = $"Table {table.TableNumber}\n({table.Capacity} seats)";
                btnTable.Size = new Size(120, 80);
                btnTable.Tag = table;
                btnTable.Font = new Font("Segoe UI", 10, FontStyle.Bold);
                btnTable.FlatStyle = FlatStyle.Flat;
                
                // Set color based on status but ALWAYS keep it clickable
                if (table.IsAvailable)
                {
                    btnTable.BackColor = Color.LightGreen;
                }
                else
                {
                    btnTable.BackColor = Color.LightCoral;
                    // Remove this line to keep the button enabled:
                    // btnTable.Enabled = false;
                }
                
                btnTable.Click += BtnTable_Click;
                tableButtons.Add(btnTable);
                flpTables.Controls.Add(btnTable);
            }
        }
        
        // Add this check before trying to instantiate OrderView
        private bool IsOrderViewImplemented()
        {
            // Check if the OrderView type exists in the current assembly
            Type orderViewType = Type.GetType("ChapeauG5.OrderView");
            return orderViewType != null;
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
    
    public partial class TableView
    {
        private FlowLayoutPanel flpTables;
        private Label lblWelcome;
        private Button btnLogout;
        
        private void InitializeComponent()
        {
            this.Text = "Chapeau - Table View";
            this.Size = new Size(800, 600);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            
            // Welcome Label
            lblWelcome = new Label();
            lblWelcome.Location = new Point(20, 20);
            lblWelcome.Size = new Size(400, 30);
            lblWelcome.Font = new Font("Segoe UI", 14, FontStyle.Bold);
            lblWelcome.Text = "Welcome!";
            
            // Logout Button
            btnLogout = new Button();
            btnLogout.Location = new Point(680, 20);
            btnLogout.Size = new Size(80, 30);
            btnLogout.Text = "Logout";
            btnLogout.BackColor = Color.LightGray;
            btnLogout.Click += btnLogout_Click;
            
            // Tables Panel
            flpTables = new FlowLayoutPanel();
            flpTables.Location = new Point(20, 70);
            flpTables.Size = new Size(740, 480);
            flpTables.Padding = new Padding(10);
            flpTables.AutoScroll = true;
            
            this.Controls.Add(lblWelcome);
            this.Controls.Add(btnLogout);
            this.Controls.Add(flpTables);
            
            this.Load += TableView_Load;
        }
    }
}