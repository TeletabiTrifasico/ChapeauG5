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
    public partial class MenuItemForm : Form
    {
        public MenuItem MenuItem { get; private set; }
        private TextBox txtName;
        private NumericUpDown numPrice;

        public MenuItemForm(MenuItem item = null)
        {
            this.Text = item == null ? "Add Menu Item" : "Edit Menu Item";
            this.Width = 300;
            this.Height = 200;

            Label lblName = new Label() { Text = "Name:", Top = 20, Left = 20 };
            txtName = new TextBox() { Top = 40, Left = 20, Width = 240 };

            Label lblPrice = new Label() { Text = "Price:", Top = 70, Left = 20 };
            numPrice = new NumericUpDown() { Top = 90, Left = 20, Width = 100, DecimalPlaces = 2, Maximum = 1000 };

            Button btnOk = new Button() { Text = "OK", Top = 130, Left = 100 };
            btnOk.Click += (s, e) =>
            {
                MenuItem = new MenuItem { Name = txtName.Text, Price = numPrice.Value };
                this.DialogResult = DialogResult.OK;
                this.Close();
            };

            this.Controls.Add(lblName);
            this.Controls.Add(txtName);
            this.Controls.Add(lblPrice);
            this.Controls.Add(numPrice);
            this.Controls.Add(btnOk);

            if (item != null)
            {
                txtName.Text = item.Name;
                numPrice.Value = item.Price;
            }
        }
    }
}
