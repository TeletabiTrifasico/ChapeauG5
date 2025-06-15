using System;
using System.Drawing;
using System.Windows.Forms;

namespace ChapeauG5
{
    public partial class SplitBillDialog : Form
    {
        private int numberOfPeople = 2;
        
        public int NumberOfPeople
        {
            get { return numberOfPeople; }
        }
        
        public SplitBillDialog()
        {
            InitializeComponent();
        }
        
        private void btnOK_Click(object sender, EventArgs e)
        {
            if (int.TryParse(txtNumberOfPeople.Text, out int result) && result > 0)
            {
                numberOfPeople = result;
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else
            {
                MessageBox.Show("Please enter a valid number greater than 0.", 
                    "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtNumberOfPeople.Focus();
                txtNumberOfPeople.SelectAll();
            }
        }
    }
}