using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ChapeauG5.ChapeauUI
{
    public partial class KitchenOrderTables : UserControl
    {
        public KitchenOrderTables()
        {
            InitializeComponent();

            // Make the entire control clickable
            this.Cursor = Cursors.Hand;

            // Add hover effects
            this.MouseEnter += OnMouseEnter;
            this.MouseLeave += OnMouseLeave;

            // Make all child controls clickable too
            foreach (Control control in this.Controls)
            {
                control.MouseEnter += OnMouseEnter;
                control.MouseLeave += OnMouseLeave;
                control.Cursor = Cursors.Hand;
            }
        }

        private void OnMouseEnter(object sender, EventArgs e)
        {
            this.BackColor = Color.LightBlue;
        }

        private void OnMouseLeave(object sender, EventArgs e)
        {
            this.BackColor = SystemColors.ControlLight;
        }

        private void KitchenOrderTables_Load(object sender, EventArgs e)
        {
            // Load event - can be used for additional initialization
        }
    }
}