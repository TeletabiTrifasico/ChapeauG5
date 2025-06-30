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
    public partial class EmployeeManagementForm : Form
    {
        private EmployeeDao employeeDao = new EmployeeDao();

        public EmployeeManagementForm()
        {
            InitializeComponent();

            this.Load += EmployeeManagementForm_Load;

            employeeDataGridView.CellValueChanged += employeeDataGridView_CellValueChanged;
            employeeDataGridView.CellEndEdit += employeeDataGridView_CellEndEdit;
        }

        private async void EmployeeManagementForm_Load(object sender, EventArgs e)
        {
            employeeDataGridView.ReadOnly = false;
            employeeDataGridView.AllowUserToAddRows = false;
            employeeDataGridView.AutoGenerateColumns = false;
            employeeDataGridView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

            roleComboBox.DataSource = Enum.GetValues(typeof(EmployeeRole));
            await LoadEmployeesAsync();
        }

        private async Task LoadEmployeesAsync()
        {
            var employees = await employeeDao.GetAllEmployeesAsync();

            employeeDataGridView.DataSource = null;
            employeeDataGridView.Columns.Clear();
            employeeDataGridView.AutoGenerateColumns = false;

            employeeDataGridView.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "FirstName",
                HeaderText = "First Name"
            });

            employeeDataGridView.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "LastName",
                HeaderText = "Last Name"
            });

            employeeDataGridView.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "Username",
                HeaderText = "Username"
            });

            employeeDataGridView.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "Email",
                HeaderText = "Email",
                Width = 200
            });

            employeeDataGridView.Columns.Add(new DataGridViewComboBoxColumn
            {
                DataPropertyName = "Role",
                Name = "Role",
                HeaderText = "Role",
                DataSource = Enum.GetValues(typeof(EmployeeRole)),
                ValueType = typeof(EmployeeRole),
                DisplayStyle = DataGridViewComboBoxDisplayStyle.DropDownButton
            });

            employeeDataGridView.Columns.Add(new DataGridViewCheckBoxColumn
            {
                DataPropertyName = "IsActive",
                HeaderText = "Active"
            });

            employeeDataGridView.DataSource = employees;
        }

        private void employeeDataGridView_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
            {
                UpdateEmployeeSafe(e.RowIndex);
            }
        }

        private void employeeDataGridView_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            employeeDataGridView.CommitEdit(DataGridViewDataErrorContexts.Commit);
        }

        private void UpdateEmployeeSafe(int rowIndex)
        {
            if (rowIndex < 0 || rowIndex >= employeeDataGridView.Rows.Count)
                return;

            var row = employeeDataGridView.Rows[rowIndex];

            if (row.DataBoundItem is Employee employee)
            {
                employeeDataGridView.EndEdit();
                employeeDataGridView.CommitEdit(DataGridViewDataErrorContexts.Commit);

                if (row.Cells["Role"] is DataGridViewComboBoxCell roleCell &&
                    roleCell.Value != null &&
                    Enum.TryParse(roleCell.Value.ToString(), out EmployeeRole parsedRole))
                {
                    employee.Role = parsedRole;
                }
                else
                {
                    MessageBox.Show("Invalid or missing role selection.", "Validation Error");
                    return;
                }

                try
                {
                    employeeDao.UpdateEmployee(employee);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Update failed: " + ex.Message, "Database Error");
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (!Enum.TryParse<EmployeeRole>(roleComboBox.SelectedItem?.ToString(), out var role))
            {
                MessageBox.Show("Please select a valid role.");
                return;
            }

            if (string.IsNullOrWhiteSpace(nameTextBox.Text) ||
                string.IsNullOrWhiteSpace(lastNameTextBox.Text) ||
                string.IsNullOrWhiteSpace(usernameTextBox.Text) ||
                string.IsNullOrWhiteSpace(passwordTextBox.Text))
            {
                MessageBox.Show("Please fill in all required fields.");
                return;
            }

            Employee emp = new Employee
            {
                FirstName = nameTextBox.Text,
                LastName = lastNameTextBox.Text,
                Username = usernameTextBox.Text,
                PasswordHash = passwordTextBox.Text,
                Email = emailTextBox.Text,
                Role = role,
                IsActive = true
            };

            try
            {
                employeeDao.AddEmployee(emp);
                _ = LoadEmployeesAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to add employee: " + ex.Message);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (employeeDataGridView.SelectedRows.Count > 0)
            {
                employeeDataGridView.EndEdit();

                var selected = (Employee)employeeDataGridView.SelectedRows[0].DataBoundItem;

                try
                {
                    employeeDao.UpdateEmployee(selected);
                    _ = LoadEmployeesAsync();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Update failed: " + ex.Message);
                }
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (employeeDataGridView.SelectedRows.Count > 0)
            {
                var selected = (Employee)employeeDataGridView.SelectedRows[0].DataBoundItem;

                try
                {
                    employeeDao.SetEmployeeActiveStatus(selected.EmployeeId, !selected.IsActive);
                    _ = LoadEmployeesAsync();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Failed to update status: " + ex.Message);
                }
            }
        }

        

        private void deleteButton_Click_1(object sender, EventArgs e)
        {
            if (employeeDataGridView.SelectedRows.Count > 0)
            {
                var selected = (Employee)employeeDataGridView.SelectedRows[0].DataBoundItem;

                DialogResult result = MessageBox.Show(
                    $"Are you sure you want to delete {selected.FirstName} {selected.LastName}?",
                    "Confirm Deletion",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning);

                if (result == DialogResult.Yes)
                {
                    try
                    {
                        employeeDao.DeleteEmployee(selected.EmployeeId);
                        _ = LoadEmployeesAsync();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Deletion failed: " + ex.Message);
                    }
                }
            }
            else
            {
                MessageBox.Show("Please select an employee to delete.");
            }
        }
    }
}
