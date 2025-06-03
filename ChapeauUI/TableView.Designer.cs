using System;
using System.Drawing;
using System.Windows.Forms;

namespace ChapeauG5
{
    partial class TableView
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
            
            this.Text = "Chapeau - Table View";
            this.Size = new Size(1200, 900);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            
            // Welcome Label
            this.lblWelcome = new Label();
            this.lblWelcome.Location = new Point(20, 60);
            this.lblWelcome.Size = new Size(400, 60);
            this.lblWelcome.Font = new Font("Segoe UI", 14, FontStyle.Bold);
            this.lblWelcome.Text = "Welcome!";
            
            // Logout Button
            this.btnLogout = new Button();
            this.btnLogout.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            this.btnLogout.Location = new Point(this.ClientSize.Width - 140, 20);
            this.btnLogout.Size = new Size(120, 80);
            this.btnLogout.Text = "Logout";
            this.btnLogout.BackColor = Color.LightGray;
            this.btnLogout.Click += new EventHandler(this.btnLogout_Click);
            
            // Tables Panel - TableLayoutPanel configuration
            this.tlpTables = new TableLayoutPanel();
            this.tlpTables.Location = new Point(100, 150); // Positioned more centrally
            this.tlpTables.Size = new Size(1000, 600); // Wider to ensure equal spacing
            this.tlpTables.Anchor = AnchorStyles.None;
            this.tlpTables.AutoSize = false;
            this.tlpTables.ColumnCount = 5;
            this.tlpTables.RowCount = 2;
            this.tlpTables.CellBorderStyle = TableLayoutPanelCellBorderStyle.None;
            
            // Equal width columns
            for (int i = 0; i < 5; i++)
            {
                this.tlpTables.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20F));
            }
            
            // Equal height rows
            for (int i = 0; i < 2; i++)
            {
                this.tlpTables.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            }
            
            // Center the TableLayoutPanel in the form
            this.tlpTables.Dock = DockStyle.None;
            this.tlpTables.Anchor = AnchorStyles.None;
            this.tlpTables.Margin = new Padding(0);
            this.tlpTables.Padding = new Padding(10);
            
            this.Controls.Add(this.lblWelcome);
            this.Controls.Add(this.btnLogout);
            this.Controls.Add(this.tlpTables);
            
            this.Load += new EventHandler(this.TableView_Load);
        }
        
        #endregion
        
        private TableLayoutPanel tlpTables;
        private Label lblWelcome;
        private Button btnLogout;
    }
}