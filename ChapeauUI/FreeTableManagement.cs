using ChapeauModel;
using ChapeauService;
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
    public partial class FreeTableManagement : Form
    {
        private Table currentTable;
        private TableService tableService;

        public FreeTableManagement(Table table)
        {
            InitializeComponent();
            currentTable = table;
            tableService = new TableService();

            // to show the table number selected
            tableNumberlbl.Text = $"Table {currentTable.TableNumber}";

            // leave the occupy button active
            occupyTableBtn.Enabled = true;
        }

        private void occupyTableBtn_Click(object sender, EventArgs e)
        {
            try
            {
                // Change the status of the table to Occupied
                currentTable.Status = TableStatus.Occupied;

                // Update the table status in the database
                tableService.UpdateTableStatus(currentTable.TableId, TableStatus.Occupied);

                MessageBox.Show($"Table {currentTable.TableNumber} is now occupied.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Close the form,
                // The refresh method in the TableView will be called to refresh the table view
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }
    }
}
