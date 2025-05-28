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
            LoadEmployees();
        }

        private void LoadEmployees()
        {
            var employees = employeeDao.GetAllEmployees();
            employeeDataGridView.DataSource = employees;
        }



      

       

        private void button1_Click(object sender, EventArgs e)
        {
            {
                Employee emp = new Employee
                {
                    FirstName = nameTextBox.Text,
                    Username = usernameTextBox.Text,
                    PasswordHash = passwordTextBox.Text, // You should hash this in production
                    Role = (EmployeeRole)Enum.Parse(typeof(EmployeeRole), roleComboBox.Text), // Ensure proper type conversion

                    LastName = lastNameTextBox.Text, // Fix for CS9035: Set required 'LastName'

                    Email = emailTextBox.Text, // Fix for CS9035: Set required 'Email'
                };
                employeeDao.AddEmployee(emp);
                LoadEmployees();
            }

        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (employeeDataGridView.SelectedRows.Count > 0)
            {
                var selected = (Employee)employeeDataGridView.SelectedRows[0].DataBoundItem;
                selected.FirstName = nameTextBox.Text;
                selected.Username = usernameTextBox.Text;
                selected.PasswordHash = passwordTextBox.Text;


                selected.Role = (EmployeeRole)Enum.Parse(typeof(EmployeeRole), roleComboBox.Text); // Ensure proper type conversion

                selected.LastName = lastNameTextBox.Text; // Fix for CS9035: Ensure 'LastName' is updated

                selected.Email = emailTextBox.Text; // Fix for CS9035: Ensure 'Email' is updated

                employeeDao.UpdateEmployee(selected);
                LoadEmployees();
            }

        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (employeeDataGridView.SelectedRows.Count > 0)
            {
                var selected = (Employee)employeeDataGridView.SelectedRows[0].DataBoundItem;
                employeeDao.SetEmployeeActiveStatus(selected.EmployeeId, !selected.IsActive);
                LoadEmployees();
            }
        }
    }
}
