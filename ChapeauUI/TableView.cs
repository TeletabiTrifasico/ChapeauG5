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

            loggedInEmployee = employee ?? throw new ArgumentNullException(nameof(employee));
            tableService = new TableService();
            orderService = new OrderService();
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
            List<TableOrderStatus> tableOrderStatuses = orderService.GetTableOrderStatuses();

            int maxTables = Math.Min(tables.Count, 10);

            for (int i = 0; i < maxTables; i++)
            {
                Table table = tables[i];
                TableOrderStatus status = tableOrderStatuses.Find(s => s.TableId == table.TableId);

                Button btnTable = CreateTableButton(table, status);
                btnTable.Click += BtnTable_Click;
                tableButtons.Add(btnTable);

                int row = i / 5;
                int col = i % 5;
                tlpTables.Controls.Add(btnTable, col, row);
            }
        }

        private Button CreateTableButton(Table table, TableOrderStatus status)
        {
            var btnTable = new Button
            {
                Text = GetTableButtonText(table, status),
                Size = new Size(200, 200),
                Tag = table,
                Dock = DockStyle.None,
                Anchor = AnchorStyles.None,
                FlatStyle = FlatStyle.Flat,
            };
            btnTable.FlatAppearance.BorderSize = 0;
            btnTable.BackColor = GetTableButtonColor(table.Status);
            return btnTable;
        }

        private string GetTableButtonText(Table table, TableOrderStatus status)
        {
            if (table.Status == TableStatus.Free)
            {
                return $"Table {table.TableNumber}\nStatus: Free";
            }
            else if (table.Status == TableStatus.Occupied)
            {
                string foodStatus = status?.FoodOrderStatus ?? "-";
                string drinkStatus = status?.DrinkOrderStatus ?? "-";
                if (status != null && status.HasRunningOrder)
                {
                    return $"Table {table.TableNumber}\nFood: {foodStatus}\nDrink: {drinkStatus}";
                }
                else
                {
                    return $"Table {table.TableNumber}\nNo active order";
                }
            }
            return $"Table {table.TableNumber}\nStatus: Unknown";
        }

        private Color GetTableButtonColor(TableStatus status)
        {
            return status switch
            {
                TableStatus.Free => Color.LightGreen,
                TableStatus.Occupied => Color.LightCoral,
                _ => SystemColors.Control
            };
        }

        private void BtnTable_Click(object sender, EventArgs e)
        {
            try
            {
                if (sender is not Button clickedButton || clickedButton.Tag is not Table selectedTable)
                    return;

                TableOrderStatus status = orderService.GetTableOrderStatuses()
                    .Find(s => s.TableId == selectedTable.TableId);

                bool hasOpenOrder = status != null && status.HasRunningOrder;

                if (selectedTable.Status == TableStatus.Free)
                {
                    ShowFreeTableManagement(selectedTable);
                }
                else if (selectedTable.Status == TableStatus.Occupied)
                {
                    ShowOccupiedTableManagement(selectedTable, hasOpenOrder);
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

        private void ShowFreeTableManagement(Table table)
        {
            using var freeForm = new FreeTableManagement(table);
            freeForm.ShowDialog();
            RefreshTables();
        }

        private void ShowOccupiedTableManagement(Table table, bool hasOpenOrder)
        {
            using var occupiedForm = new OccupiedTableManagement(loggedInEmployee, table);
            occupiedForm.SetTakeOrderButtonEnabled(true);
            occupiedForm.SetFreeTableButtonEnabled(!hasOpenOrder);
            occupiedForm.FormClosed += (s, args) => RefreshTables();
            occupiedForm.ShowDialog();
        }

        private void btnLogout_Click(object sender, EventArgs e)
        {
            var loginForm = new LoginForm();
            loginForm.ClearCredentials();
            loginForm.Show();
            this.Hide();
        }

        private Image GetRefreshIcon()
        {
            try
            {
                return SystemIcons.Information.ToBitmap();
            }
            catch
            {
                return null;
            }
        }
    }
}