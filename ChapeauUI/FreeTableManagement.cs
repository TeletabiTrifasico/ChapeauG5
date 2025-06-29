using ChapeauModel;
using ChapeauService;
using System;
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
            currentTable = table ?? throw new ArgumentNullException(nameof(table));
            tableService = new TableService();

            InitializeForm();
        }

        private void InitializeForm()
        {
            tableNumberlbl.Text = $"Table {currentTable.TableNumber}";
            occupyTableBtn.Enabled = true;
        }

        private void occupyTableBtn_Click(object sender, EventArgs e)
        {
            try
            {
                OccupyTable();
                ShowInfoMessage($"Table {currentTable.TableNumber} is now occupied.", "Success");
                Close();
            }
            catch (Exception ex)
            {
                ShowInfoMessage($"Error: {ex.Message}", "Error", MessageBoxIcon.Error);
            }
        }

        private void OccupyTable()
        {
            currentTable.Status = TableStatus.Occupied;
            tableService.UpdateTableStatus(currentTable.TableId, TableStatus.Occupied);
        }

        private void ShowInfoMessage(string message, string caption, MessageBoxIcon icon = MessageBoxIcon.Information)
        {
            MessageBox.Show(message, caption, MessageBoxButtons.OK, icon);
        }
    }
}
