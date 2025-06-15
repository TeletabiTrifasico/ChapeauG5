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

namespace ChapeauG5.ChapeauUI
{
    public partial class OccupiedTableManagement : Form
    {
        private Employee loggedInEmployee;
        private Table currentTable;

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
    }
}
