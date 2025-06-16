using System;
using System.Drawing;
using System.Windows.Forms;

namespace ChapeauG5
{
    partial class SplitBillDialog
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;
        
        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }
        
        #region Windows Form Designer generated code
        
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            
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
        
        #endregion
        
        private Label lblPrompt;
        private TextBox txtNumberOfPeople;
        private Button btnOK;
        private Button btnCancel;
    }
}