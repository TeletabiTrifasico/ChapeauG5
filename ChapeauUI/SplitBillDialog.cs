using System;
using System.Drawing;
using System.Windows.Forms;

namespace ChapeauG5
{
    public partial class SplitBillDialog : Form
    {
        public int NumberOfPeople { get; private set; }
        
        public SplitBillDialog()
        {
            InitializeComponent();
            NumberOfPeople = 2; // Default value
        }
        
        private void btnOK_Click(object sender, EventArgs e)
        {
            try
            {
                NumberOfPeople = int.Parse(txtNumberOfPeople.Text);
                
                if (NumberOfPeople < 2)
                {
                    MessageBox.Show("Please enter a number greater than or equal to 2.", 
                        "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch
            {
                MessageBox.Show("Please enter a valid number.", 
                    "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    }
    
    public partial class SplitBillDialog
    {
        private Label lblPrompt;
        private TextBox txtNumberOfPeople;
        private Button btnOK;
        private Button btnCancel;
        
        private void InitializeComponent()
        {
            this.Text = "Split Bill";
            this.Size = new Size(300, 150);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.StartPosition = FormStartPosition.CenterParent;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            
            this.lblPrompt = new Label();
            this.lblPrompt.Location = new Point(20, 20);
            this.lblPrompt.Size = new Size(200, 20);
            this.lblPrompt.Text = "Number of people to split the bill:";
            
            this.txtNumberOfPeople = new TextBox();
            this.txtNumberOfPeople.Location = new Point(20, 50);
            this.txtNumberOfPeople.Size = new Size(60, 25);
            this.txtNumberOfPeople.Text = "2";
            
            this.btnOK = new Button();
            this.btnOK.Location = new Point(100, 80);
            this.btnOK.Size = new Size(80, 30);
            this.btnOK.Text = "OK";
            this.btnOK.Click += new EventHandler(this.btnOK_Click);
            
            this.btnCancel = new Button();
            this.btnCancel.Location = new Point(190, 80);
            this.btnCancel.Size = new Size(80, 30);
            this.btnCancel.Text = "Cancel";
            this.btnCancel.DialogResult = DialogResult.Cancel;
            
            this.Controls.Add(this.lblPrompt);
            this.Controls.Add(this.txtNumberOfPeople);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.btnCancel);
            
            this.AcceptButton = this.btnOK;
            this.CancelButton = this.btnCancel;
        }
    }
}