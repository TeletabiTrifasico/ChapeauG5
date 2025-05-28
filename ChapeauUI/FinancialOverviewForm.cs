using ChapeauDAL;
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
        private OrderDao orderDao = new OrderDao();

        public FinancialOverviewForm()
        {
            InitializeComponent();
        }

        private void viewButton_Click(object sender, EventArgs e)
        {
            DateTime start = startDatePicker.Value;
            DateTime end = endDatePicker.Value;

            decimal drinks = orderDao.GetTotalSales("Drinks", start, end);
            decimal lunch = orderDao.GetTotalSales("Lunch", start, end);
            decimal dinner = orderDao.GetTotalSales("Dinner", start, end);
            decimal tips = orderDao.GetTotalTips(start, end);

            drinksLabel.Text = $"Drinks: €{drinks:0.00}";
            lunchLabel.Text = $"Lunch: €{lunch:0.00}";
            dinnerLabel.Text = $"Dinner: €{dinner:0.00}";
            tipsLabel.Text = $"Tips: €{tips:0.00}";
        }
    }

}
