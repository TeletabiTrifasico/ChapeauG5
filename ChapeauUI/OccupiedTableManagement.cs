using ChapeauModel;
using System;
using System.Windows.Forms;
using ChapeauService;

namespace ChapeauG5.ChapeauUI
{
    public partial class OccupiedTableManagement : Form
    {
        private Employee loggedInEmployee;
        private Table currentTable;
        private OrderItemService orderItemService;
        private TableService tableService;

        public OccupiedTableManagement(Employee employee, Table table)
        {
            InitializeComponent();

            loggedInEmployee = employee ?? throw new ArgumentNullException(nameof(employee));
            currentTable = table ?? throw new ArgumentNullException(nameof(table));
            orderItemService = new OrderItemService();
            tableService = new TableService();

            lbltabelNumber.Text = $"Table {currentTable.TableNumber}";
            this.Load += OccupiedTableManagement_Load;
        }

        private void OccupiedTableManagement_Load(object sender, EventArgs e)
        {
            LoadReadyToBeServedItems();
        }

        private void freeTablebtn_Click(object sender, EventArgs e)
        {
            try
            {
                SetTableFree();
                ShowMessage("Table has been set to Free.", "Success", MessageBoxIcon.Information);
                Close();
            }
            catch (Exception ex)
            {
                ShowMessage("Error freeing table: " + ex.Message, "Error", MessageBoxIcon.Error);
            }
        }

        private void takeOrderbtn_Click(object sender, EventArgs e)
        {
            try
            {
                OpenOrderView();
            }
            catch (Exception ex)
            {
                ShowMessage("Error opening OrderView: " + ex.Message, "Error", MessageBoxIcon.Error);
            }
        }

        private void setAsServedBtn_Click(object sender, EventArgs e)
        {
            if (lvReadyToBeServedItems.Items.Count == 0)
            {
                ShowMessage("There are no items ready to be served.", "Info", MessageBoxIcon.Information);
                return;
            }

            MarkAllItemsAsServed();
            ShowMessage("All ready-to-be-served items have been marked as served.", "Success", MessageBoxIcon.Information);
            LoadReadyToBeServedItems();
        }

        private void LoadReadyToBeServedItems()
        {
            lvReadyToBeServedItems.Items.Clear();
            var readyItems = orderItemService.GetReadyToBeServedItemsByTable(currentTable.TableId);

            foreach (var item in readyItems)
            {
                var lvi = new ListViewItem(item.MenuItemId?.Name ?? "Unknown");
                lvi.SubItems.Add(item.Quantity.ToString());
                lvi.SubItems.Add(item.Status.ToString());
                lvi.Tag = item;
                lvReadyToBeServedItems.Items.Add(lvi);
            }
        }

        private void SetTableFree()
        {
            tableService.UpdateTableStatus(currentTable.TableId, TableStatus.Free);
        }

        private void OpenOrderView()
        {
            var orderView = new OrderView(loggedInEmployee, currentTable);
            orderView.Show();
        }

        private void MarkAllItemsAsServed()
        {
            foreach (ListViewItem lvi in lvReadyToBeServedItems.Items)
            {
                if (lvi.Tag is OrderItem item)
                {
                    orderItemService.UpdateOrderItemStatus(item.OrderItemId, OrderItem.OrderStatus.Served);
                }
            }
        }

        private void ShowMessage(string message, string caption, MessageBoxIcon icon)
        {
            MessageBox.Show(message, caption, MessageBoxButtons.OK, icon);
        }

        // Optionally, you can add public methods to enable/disable buttons externally
        public void SetTakeOrderButtonEnabled(bool enabled)
        {
            takeOrderbtn.Enabled = enabled;
        }

        public void SetFreeTableButtonEnabled(bool enabled)
        {
            freeTablebtn.Enabled = enabled;
        }
    }
}

