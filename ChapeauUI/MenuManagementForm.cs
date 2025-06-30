using ChapeauDAL;

using ChapeauModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
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


        private void CourseFilterComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadMenuItems();
        }

        private async void LoadMenuItems()
        {
            var items = await menuItemDao.GetAllMenuItemsAsync();

            // ✅ Check if courseFilterComboBox has a selected item before using it
            if (courseFilterComboBox?.SelectedItem != null && courseFilterComboBox.SelectedItem.ToString() != "All")
            {
                if (Enum.TryParse<CourseType>(courseFilterComboBox.SelectedItem.ToString(), out var selectedCourse))
                {
                    items = items.Where(item => item.CourseType == selectedCourse).ToList();
                }
            }

            menuDataGridView.AutoGenerateColumns = false;
            menuDataGridView.Columns.Clear();

            menuDataGridView.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "MenuItemId",
                HeaderText = "ID",
                Width = 50,
            });
            menuDataGridView.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "Name",
                HeaderText = "Name"
            });
            menuDataGridView.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "Description",
                HeaderText = "Description",
                Width = 400, 
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
            });
            menuDataGridView.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "Price",
                HeaderText = "Price",
                Width = 50,
                DefaultCellStyle = new DataGridViewCellStyle { Format = "C2", FormatProvider = CultureInfo.CreateSpecificCulture("en-IE") }
            });
            menuDataGridView.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "Stock",
                HeaderText = "Stock",
                Width = 50

            });
            menuDataGridView.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "VatPercentage",
                HeaderText = "VAT",
                DefaultCellStyle = new DataGridViewCellStyle { Format = "0\\%" },
                 Width = 50
            }); 
            menuDataGridView.Columns.Add(new DataGridViewComboBoxColumn
            {
                DataPropertyName = "CourseType",
                HeaderText = "Course",
                DataSource = Enum.GetValues(typeof(CourseType))
            });

            menuDataGridView.Columns.Add(new DataGridViewCheckBoxColumn
            {
                DataPropertyName = "IsActive",
                HeaderText = "Active"
            });

            menuDataGridView.DataSource = items;

            menuDataGridView.RowValidated -= menuDataGridView_RowValidated;
            menuDataGridView.RowValidated += menuDataGridView_RowValidated;

            menuDataGridView.RowPrePaint -= MenuDataGridView_RowPrePaint;
            menuDataGridView.RowPrePaint += MenuDataGridView_RowPrePaint;

            menuDataGridView.CellDoubleClick += MenuDataGridView_CellDoubleClick;
        }


        private void MenuDataGridView_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            // Ensure the clicked cell is valid
            if (e.RowIndex >= 0 && menuDataGridView.Columns[e.ColumnIndex].DataPropertyName == "Description")
            {
                var row = menuDataGridView.Rows[e.RowIndex];
                var item = (MenuItem)row.DataBoundItem;

                string newDescription = ShowMultilineEditor(item.Description);
                if (newDescription != null && newDescription != item.Description)
                {
                    item.Description = newDescription;

                    try
                    {
                        menuItemDao.UpdateMenuItem(item); // Save change
                        LoadMenuItems(); // Refresh
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Failed to update description:\n" + ex.Message);
                    }
                }
            }
        }

        private string ShowMultilineEditor(string currentText)
        {
            Form editForm = new Form
            {
                Text = "Edit Description",
                Size = new Size(400, 300),
                StartPosition = FormStartPosition.CenterParent,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MinimizeBox = false,
                MaximizeBox = false
            };

            TextBox editor = new TextBox
            {
                Multiline = true,
                Dock = DockStyle.Fill,
                Text = currentText,
                ScrollBars = ScrollBars.Vertical
            };
            editForm.Controls.Add(editor);

            Button okButton = new Button
            {
                Text = "OK",
                Dock = DockStyle.Bottom,
                DialogResult = DialogResult.OK,
                Height = 30
            };
            editForm.Controls.Add(okButton);
            editForm.AcceptButton = okButton;

            return editForm.ShowDialog() == DialogResult.OK ? editor.Text : null;
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

        private void MenuDataGridView_RowPrePaint(object sender, DataGridViewRowPrePaintEventArgs e)
        {
            var row = menuDataGridView.Rows[e.RowIndex];
            if (row.DataBoundItem is MenuItem item)
            {
                if (item.Stock == 0)
                {
                    row.DefaultCellStyle.BackColor = Color.LightCoral; // out of stock
                }
                else if (item.Stock <= 10)
                {
                    row.DefaultCellStyle.BackColor = Color.Khaki; // low stock
                }
                else
                {
                    row.DefaultCellStyle.BackColor = Color.White; // reset
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
            // Simple validation
            if (string.IsNullOrWhiteSpace(nameTextBox.Text) ||
                string.IsNullOrWhiteSpace(priceTextBox.Text) ||
                string.IsNullOrWhiteSpace(stockTextBox.Text) ||
                string.IsNullOrWhiteSpace(vatTextBox.Text))
            {
                MessageBox.Show("Please fill in all required fields: Name, Price, Stock, and VAT.", "Invalid input", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!decimal.TryParse(priceTextBox.Text, out decimal price) || price < 0)
            {
                MessageBox.Show("Please enter a valid positive price.", "Invalid input", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!int.TryParse(stockTextBox.Text, out int stock) || stock < 0)
            {
                MessageBox.Show("Please enter a valid stock quantity.", "Invalid input", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!int.TryParse(vatTextBox.Text, out int vat) || vat < 0 || vat > 100)
            {
                MessageBox.Show("Please enter a valid VAT percentage (0–100).", "Invalid input", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var newItem = new MenuItem
            {
                Name = nameTextBox.Text,
                Description = descriptionTextBox.Text,
                Price = decimal.Parse(priceTextBox.Text),
                Stock = int.Parse(stockTextBox.Text),
                VatPercentage = int.Parse(vatTextBox.Text),
                CategoryId = new Menu { CategoryId = 1 }, // still hardcoded
                CourseType = (CourseType)courseTypeComboBox.SelectedItem,
                IsAlcoholic = false,
                IsActive = true
            };

            try
            {
                menuItemDao.AddMenuItem(newItem);
                LoadMenuItems();
                MessageBox.Show("Menu item added successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error adding menu item: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
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

        private void deleteButton_Click(object sender, EventArgs e)
        {
            if (menuDataGridView.SelectedRows.Count > 0)
            {
                var selectedItem = (MenuItem)menuDataGridView.SelectedRows[0].DataBoundItem;

                var confirmResult = MessageBox.Show(
                    $"Are you sure you want to delete '{selectedItem.Name}'?",
                    "Confirm Delete",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning);

                if (confirmResult == DialogResult.Yes)
                {
                    try
                    {
                        menuItemDao.DeleteMenuItem(selectedItem.MenuItemId);
                        LoadMenuItems();
                        MessageBox.Show("Item deleted successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error deleting menu item: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Please select an item to delete.", "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
    }

}
