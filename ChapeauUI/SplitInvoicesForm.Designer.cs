using System;
using System.Drawing;
using System.Windows.Forms;

namespace ChapeauG5
{
    partial class SplitInvoicesForm
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
            
            this.Text = "Split Bills";
            this.Size = new Size(580, 450);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.StartPosition = FormStartPosition.CenterParent;
            this.MaximizeBox = false;
            
            this.tabSplitInvoices = new TabControl();
            this.tabSplitInvoices.Location = new Point(10, 10);
            this.tabSplitInvoices.Size = new Size(550, 380);
            
            this.btnClose = new Button();
            this.btnClose.Text = "Close";
            this.btnClose.DialogResult = DialogResult.Cancel;
            this.btnClose.Location = new Point(460, 395);
            this.btnClose.Size = new Size(100, 30);
            
            this.Controls.Add(this.tabSplitInvoices);
            this.Controls.Add(this.btnClose);
            
            this.Load += new EventHandler(this.SplitInvoicesForm_Load);
        }
        
        #endregion
        
        private TabControl tabSplitInvoices;
        private Button btnClose;
    }
}