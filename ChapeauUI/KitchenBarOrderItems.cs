using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ChapeauG5.ChapeauUI;

public partial class KitchenBarOrderItems : UserControl
{
    public KitchenBarOrderItems()
    {
        InitializeComponent();

        // Add some visual styling
        this.BorderStyle = BorderStyle.FixedSingle;
        this.Margin = new Padding(5);
    }

    public void UpdateButtonStates(bool canStart, bool canReady)
    {
        btnStartOrderItem.Enabled = canStart;
        btnReadyOrderItem.Enabled = canReady;

        // Visual feedback for button states
        btnStartOrderItem.BackColor = canStart ? Color.Orange : Color.LightGray;
        btnReadyOrderItem.BackColor = canReady ? Color.LightGreen : Color.LightGray;
    }

    private void btnStartOrderItem_Click(object sender, EventArgs e)
    {
        // Event handled by parent form
    }

    private void btnReadyOrderItem_Click(object sender, EventArgs e)
    {
        // Event handled by parent form
    }
}