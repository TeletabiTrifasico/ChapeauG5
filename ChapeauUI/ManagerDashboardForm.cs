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
        private ListBox lstMenuItems;
        private Button btnAddMenuItem, btnEditMenuItem, btnDeleteMenuItem;
        private ListBox lstEmployees;
        private Button btnAddEmployee, btnEditEmployee, btnDeleteEmployee;

        public ManagerDashboardForm()
        {
            this.Text = "Restaurant Management";
            this.Width = 800;
            this.Height = 600;
            this.StartPosition = FormStartPosition.CenterScreen;

            InitializeComponents();
        }

        private void InitializeComponents()
        {
            Label lblTitle = new Label() { Text = "Restaurant Management", AutoSize = true, Font = new System.Drawing.Font("Segoe UI", 18), Top = 20, Left = 20 };
            this.Controls.Add(lblTitle);

            // Menu Items Group
            GroupBox groupMenu = new GroupBox() { Text = "Manage Menu", Width = 350, Height = 400, Top = 80, Left = 20 };
            lstMenuItems = new ListBox() { Width = 310, Height = 250, Top = 30, Left = 20 };
            btnAddMenuItem = new Button() { Text = "Add", Top = 290, Left = 20 };
            btnEditMenuItem = new Button() { Text = "Edit", Top = 290, Left = 100 };
            btnDeleteMenuItem = new Button() { Text = "Delete", Top = 290, Left = 180 };

            btnAddMenuItem.Click += BtnAddMenuItem_Click;
            btnEditMenuItem.Click += BtnEditMenuItem_Click;
            btnDeleteMenuItem.Click += BtnDeleteMenuItem_Click;

            groupMenu.Controls.Add(lstMenuItems);
            groupMenu.Controls.Add(btnAddMenuItem);
            groupMenu.Controls.Add(btnEditMenuItem);
            groupMenu.Controls.Add(btnDeleteMenuItem);
            this.Controls.Add(groupMenu);

            // Employees Group
            GroupBox groupEmployees = new GroupBox() { Text = "Manage Employees", Width = 350, Height = 400, Top = 80, Left = 400 };
            lstEmployees = new ListBox() { Width = 310, Height = 250, Top = 30, Left = 20 };
            btnAddEmployee = new Button() { Text = "Add", Top = 290, Left = 20 };
            btnEditEmployee = new Button() { Text = "Edit", Top = 290, Left = 100 };
            btnDeleteEmployee = new Button() { Text = "Delete", Top = 290, Left = 180 };

            btnAddEmployee.Click += BtnAddEmployee_Click;
            btnEditEmployee.Click += BtnEditEmployee_Click;
            btnDeleteEmployee.Click += BtnDeleteEmployee_Click;

            groupEmployees.Controls.Add(lstEmployees);
            groupEmployees.Controls.Add(btnAddEmployee);
            groupEmployees.Controls.Add(btnEditEmployee);
            groupEmployees.Controls.Add(btnDeleteEmployee);
            this.Controls.Add(groupEmployees);
        }

        // Menu Item Handlers
        private void BtnAddMenuItem_Click(object sender, EventArgs e)
        {
            using (var dialog = new MenuItemForm())
            {
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    lstMenuItems.Items.Add(dialog.MenuItem);
                }
            }
        }

        private void BtnEditMenuItem_Click(object sender, EventArgs e)
        {
            if (lstMenuItems.SelectedItem is MenuItem item)
            {
                using (var dialog = new MenuItemForm(item))
                {
                    if (dialog.ShowDialog() == DialogResult.OK)
                    {
                        int index = lstMenuItems.SelectedIndex;
                        lstMenuItems.Items[index] = dialog.MenuItem;
                    }
                }
            }
        }

        private void BtnDeleteMenuItem_Click(object sender, EventArgs e)
        {
            if (lstMenuItems.SelectedItem != null)
            {
                lstMenuItems.Items.Remove(lstMenuItems.SelectedItem);
            }
        }

        // Employee Handlers
        private void BtnAddEmployee_Click(object sender, EventArgs e)
        {
            using (var dialog = new EmployeeForm())
            {
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    lstEmployees.Items.Add(dialog.Employee);
                }
            }
        }

        private void BtnEditEmployee_Click(object sender, EventArgs e)
        {
            if (lstEmployees.SelectedItem is EmployeeDummy emp)
            {
                using (var dialog = new EmployeeForm(emp))
                {
                    if (dialog.ShowDialog() == DialogResult.OK)
                    {
                        int index = lstEmployees.SelectedIndex;
                        lstEmployees.Items[index] = dialog.Employee;
                    }
                }
            }
        }

        private void BtnDeleteEmployee_Click(object sender, EventArgs e)
        {
            if (lstEmployees.SelectedItem != null)
            {
                lstEmployees.Items.Remove(lstEmployees.SelectedItem);
            }
        }
    }
}
