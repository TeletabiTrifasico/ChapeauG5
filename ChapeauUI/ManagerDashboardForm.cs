using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ChapeauUI
{
    public partial class ManagerDashboardForm : Form
    {
        public ManagerDashboardForm()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {

            MenuManagementForm menuForm = new MenuManagementForm();
            menuForm.ShowDialog();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            EmployeeManagementForm employeeForm = new EmployeeManagementForm();
            employeeForm.ShowDialog();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            FinancialOverviewForm financeForm = new FinancialOverviewForm();
            financeForm.ShowDialog();
        }
    }
}
