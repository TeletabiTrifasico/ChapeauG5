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
    public partial class EmployeeForm : Form
    {
        public EmployeeDummy Employee { get; private set; }
        private TextBox txtName;
        private ComboBox cmbRole;

        public EmployeeForm(EmployeeDummy emp = null)
        {
            this.Text = emp == null ? "Add Employee" : "Edit Employee";
            this.Width = 300;
            this.Height = 200;

            Label lblName = new Label() { Text = "Name:", Top = 20, Left = 20 };
            txtName = new TextBox() { Top = 40, Left = 20, Width = 240 };

            Label lblRole = new Label() { Text = "Role:", Top = 70, Left = 20 };
            cmbRole = new ComboBox() { Top = 90, Left = 20, Width = 150 };
            cmbRole.Items.AddRange(new string[] { "Waiter", "Kitchen", "Bar", "Manager" });

            Button btnOk = new Button() { Text = "OK", Top = 130, Left = 100 };
            btnOk.Click += (s, e) =>
            {
                Employee = new EmployeeDummy { Name = txtName.Text, Role = cmbRole.SelectedItem?.ToString() ?? "" };
                this.DialogResult = DialogResult.OK;
                this.Close();
            };

            this.Controls.Add(lblName);
            this.Controls.Add(txtName);
            this.Controls.Add(lblRole);
            this.Controls.Add(cmbRole);
            this.Controls.Add(btnOk);

            if (emp != null)
            {
                txtName.Text = emp.Name;
                cmbRole.SelectedItem = emp.Role;
            }
        }
    }
}
