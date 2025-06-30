using ChapeauDAL;
using ChapeauModel;
using Microsoft.Data.SqlClient;
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
    public partial class FinancialOverviewForm : Form
    {
        private InvoiceDao invoiceDao = new InvoiceDao();

        public FinancialOverviewForm()
        {
            InitializeComponent();
            InitializePeriodComboBox();

            viewButton.Click += viewButton_Click;
            periodComboBox.SelectedIndexChanged += PeriodComboBox_SelectedIndexChanged;
        }

        private void InitializePeriodComboBox()
        {
            periodComboBox.Items.AddRange(new string[] { "Custom", "This Month", "This Quarter", "This Year" });
            periodComboBox.SelectedIndex = 0; // Default to Custom
        }

        private void PeriodComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            DateTime now = DateTime.Now;

            switch (periodComboBox.SelectedItem.ToString())
            {
                case "This Month":
                    startDatePicker.Value = new DateTime(now.Year, now.Month, 1);
                    endDatePicker.Value = startDatePicker.Value.AddMonths(1).AddDays(-1);
                    break;

                case "This Quarter":
                    int currentQuarter = (now.Month - 1) / 3 + 1;
                    int startMonth = (currentQuarter - 1) * 3 + 1;
                    startDatePicker.Value = new DateTime(now.Year, startMonth, 1);
                    endDatePicker.Value = startDatePicker.Value.AddMonths(3).AddDays(-1);
                    break;

                case "This Year":
                    startDatePicker.Value = new DateTime(now.Year, 1, 1);
                    endDatePicker.Value = new DateTime(now.Year, 12, 31);
                    break;

                case "Custom":
                    // Leave current selections
                    break;
            }
        }

        private void viewButton_Click(object sender, EventArgs e)
        {
            DateTime start = startDatePicker.Value.Date;
            DateTime end = endDatePicker.Value.Date.AddDays(1).AddTicks(-1); // include full day

            List<Invoice> invoices = invoiceDao.GetInvoicesBetween(start, end);

            if (invoices == null || invoices.Count == 0)
            {
                salesLabel.Text = "€0.00";
                incomeLabel.Text = "€0.00";
                tipsLabel.Text = "€0.00";
                return;
            }

            decimal totalSales = invoices.Sum(i => i.TotalAmount);
            decimal totalVat = invoices.Sum(i => i.TotalVat);
            decimal totalIncome = totalSales + totalVat;
            decimal totalTips = invoices.Sum(i => i.TotalTipAmount);

            salesLabel.Text = $"€{totalSales:0.00}";
            incomeLabel.Text = $"€{totalIncome:0.00}";
            tipsLabel.Text = $"€{totalTips:0.00}";
        }
} 


}