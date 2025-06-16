using ChapeauModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ChapeauService;

namespace ChapeauG5.ChapeauUI
{
    public partial class OccupiedTableManagement : Form
    {
        private Employee loggedInEmployee;
        private Table currentTable;
        private readonly OrderItemService orderItemService = new OrderItemService();

        public OccupiedTableManagement(Employee employee, Table table)
        {
            InitializeComponent();

            loggedInEmployee = employee;
            currentTable = table;

            if (loggedInEmployee == null || currentTable == null)
            {
                throw new ArgumentNullException("Employee or Table is null");
            }

            lbltabelNumber.Text = $"Table {currentTable.TableNumber}";

            this.Load += OccupiedTableManagement_Load;
        }

        private void OccupiedTableManagement_Load(object sender, EventArgs e)
        {
            LoadReadyToBeServedItems();
        }



        public void SetFreeTableButtonEnabled(bool enabled)
        {
            freeTablebtn.Enabled = enabled;
        }

        public void SetTakeOrderButtonEnabled(bool enabled)
        {
            takeOrderbtn.Enabled = enabled;
        }


        private void freeTablebtn_Click(object sender, EventArgs e)
        {

            try
            {
                // TableService instance
                var tableService = new ChapeauService.TableService();

                // Update the table status as Free  
                tableService.UpdateTableStatus(currentTable.TableId, ChapeauModel.TableStatus.Free);

                MessageBox.Show("Table has been set to Free.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error freeing table: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void takeOrderbtn_Click(object sender, EventArgs e)
        {
            try
            {
                OrderView orderView = new OrderView(loggedInEmployee, currentTable);
                orderView.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error opening OrderView: " + ex.Message);
            }

        }


        private void setAsServedBtn_Click(object sender, EventArgs e)
        {
            if (lvReadyToBeServedItems.Items.Count == 0)
            {
                MessageBox.Show("There are no items ready to be served.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // Make all ietms in the list served
            foreach (ListViewItem lvi in lvReadyToBeServedItems.Items)
            {
                if (lvi.Tag is OrderItem item)
                {
                    orderItemService.UpdateOrderItemStatus(item.OrderItemId, OrderItem.OrderStatus.Served);
                }
            }

            MessageBox.Show("All ready-to-be-served items have been marked as served.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

            // Update the list
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

    }


}

