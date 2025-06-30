using ChapeauDAL;

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

    public partial class MenuManagementForm : Form
    {
        private MenuItemDao menuItemDao = new MenuItemDao();


        public MenuManagementForm()
        {
            InitializeComponent();
            LoadMenuItems();
        }

        private async void LoadMenuItems()
        {
            var items = await menuItemDao.GetAllMenuItemsAsync();

            menuDataGridView.AutoGenerateColumns = false;
            menuDataGridView.Columns.Clear();

            menuDataGridView.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "MenuItemId",
                HeaderText = "ID"
            });
            menuDataGridView.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "Name",
                HeaderText = "Name"
            });
            menuDataGridView.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "Description",
                HeaderText = "Description"
            });
            menuDataGridView.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "Price",
                HeaderText = "Price"
            });
            menuDataGridView.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "Stock",
                HeaderText = "Stock"
            });
            menuDataGridView.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "VatPercentage",
                HeaderText = "VAT"
            });
            menuDataGridView.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "CourseType",
                HeaderText = "Course"
            });
            menuDataGridView.Columns.Add(new DataGridViewCheckBoxColumn
            {
                DataPropertyName = "IsAlcoholic",
                HeaderText = "Alcoholic"
            });
            menuDataGridView.Columns.Add(new DataGridViewCheckBoxColumn
            {
                DataPropertyName = "IsActive",
                HeaderText = "Active"
            });

            menuDataGridView.DataSource = items;
            menuDataGridView.RowValidated += menuDataGridView_RowValidated;
        }

        private void menuDataGridView_RowValidated(object sender, DataGridViewCellEventArgs e)
        {
            if (menuDataGridView.CurrentRow?.DataBoundItem is MenuItem updatedItem)
            {
                try
                {
                    menuItemDao.UpdateMenuItem(updatedItem);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error updating menu item: " + ex.Message);
                }
            }
        }

        private void menuDataGridView_SelectionChanged(object sender, EventArgs e)
        {
            if (menuDataGridView.SelectedRows.Count > 0)
            {
                var item = (MenuItem)menuDataGridView.SelectedRows[0].DataBoundItem;
                nameTextBox.Text = item.Name;
                priceTextBox.Text = item.Price.ToString("0.00");
                stockTextBox.Text = item.Stock.ToString();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var newItem = new MenuItem
            {
                Name = nameTextBox.Text,
                Price = decimal.Parse(priceTextBox.Text),
                Stock = int.Parse(stockTextBox.Text),
                CategoryId = new Menu { CategoryId = 1 }, // Required by DB, hardcoded or set some default
                VatPercentage = 21,
                CourseType = CourseType.Main,
                IsAlcoholic = false,
                IsActive = true
            };

            menuItemDao.AddMenuItem(newItem);
            LoadMenuItems();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (menuDataGridView.SelectedRows.Count > 0)
            {
                var selectedItem = (MenuItem)menuDataGridView.SelectedRows[0].DataBoundItem;

                selectedItem.Name = nameTextBox.Text;
                selectedItem.Price = decimal.Parse(priceTextBox.Text);
                selectedItem.Stock = int.Parse(stockTextBox.Text);

                menuItemDao.UpdateMenuItem(selectedItem);
                LoadMenuItems();
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (menuDataGridView.SelectedRows.Count > 0)
            {
                var selected = (MenuItem)menuDataGridView.SelectedRows[0].DataBoundItem;
                menuItemDao.SetMenuItemActiveStatus(selected.MenuItemId, !selected.IsActive);
                LoadMenuItems();
            }
        }
    }

}
