using ChapeauDAL;
using ChapeauG5.ChapeauDAL;
using ChapeauG5.ChapeauModel;
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

namespace ChapeauUI
{

    public partial class ManagerDashboardForm : Form
    {
        private MenuItemDao menuItemDao = new MenuItemDao();

        public ManagerDashboardForm()
        {
            InitializeComponent();
            LoadMenuItems();
        }

        private void LoadMenuItems()
        {
            var items = menuItemDao.GetAllMenuItems();
            menuDataGridView.DataSource = items;
        }




        private List<Category> categories;

        private void LoadCategories()
        {
            var dao = new CategoryDao(); // You’ll need to create this
            categories = dao.GetAllCategories();

            categoryComboBox.DataSource = categories;
            categoryComboBox.DisplayMember = "Name";
            categoryComboBox.ValueMember = "CategoryID";
        }



        private void menuDataGridView_SelectionChanged(object sender, EventArgs e)
        {
            if (menuDataGridView.SelectedRows.Count > 0)
            {
                var item = (MenuItem)menuDataGridView.SelectedRows[0].DataBoundItem;
                nameTextBox.Text = item.Name;
                priceTextBox.Text = item.Price.ToString();
                stockTextBox.Text = item.Stock.ToString();
              

                categoryIDComboBox.SelectedIndex = item.CategoryID - 1; // Assuming categories are indexed starting from 1

                categoryComboBox.SelectedItem = item.Category;
              
            }
        }



        private void button1_Click(object sender, EventArgs e)
        {
            Category selectedCategory = (Category)categoryComboBox.SelectedItem;

            MenuItem item = new MenuItem
            {
                Name = nameTextBox.Text,
                Price = decimal.Parse(priceTextBox.Text),
                Stock = int.Parse(stockTextBox.Text),
                CategoryID = selectedCategory.CategoryID,
                Category = selectedCategory.Name
            };

        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (menuDataGridView.SelectedRows.Count > 0)
            {
                var selected = (MenuItem)menuDataGridView.SelectedRows[0].DataBoundItem;
                selected.Name = nameTextBox.Text;
                selected.Price = decimal.Parse(priceTextBox.Text);
                selected.Stock = int.Parse(stockTextBox.Text);
               

                selected.CategoryID = categoryIDComboBox.SelectedIndex + 1; // Assuming categories are indexed starting from 1

                selected.Category = categoryComboBox.Text;
                menuItemDao.UpdateMenuItem(selected);
                LoadMenuItems();
            }

        }

        private void button3_Click(object sender, EventArgs e)
        {
            {
                if (menuDataGridView.SelectedRows.Count > 0)
                {
                    var selected = (MenuItem)menuDataGridView.SelectedRows[0].DataBoundItem;
                    menuItemDao.SetMenuItemActiveStatus(selected.MenuItemID, !selected.IsActive);
                    LoadMenuItems();
                }
            }

        }
    }
}
