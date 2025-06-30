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

            // --- Navbar Panel ---
            this.panelNavBar = new Panel();
            this.panelNavBar.Dock = DockStyle.Top;
            this.panelNavBar.Height = 70;
            this.panelNavBar.BackColor = Color.FromArgb(120, 120, 120);

            // --- Title Label (Tables) ---
            this.lblTables = new Label();
            this.lblTables.Text = "Tables";
            this.lblTables.Font = new Font("Segoe UI", 14, FontStyle.Bold);
            this.lblTables.Size = new Size(140, 50);
            this.lblTables.Location = new Point(20, 10);
            this.lblTables.ForeColor = Color.White;
            this.lblTables.TextAlign = ContentAlignment.MiddleCenter;
            this.lblTables.Cursor = Cursors.Hand; // 

            // --- Logout Button ---
            this.btnLogout = new Button();
            this.btnLogout.Size = new Size(100, 40);
            this.btnLogout.Text = "Logout";
            this.btnLogout.BackColor = Color.LightGray;
            this.btnLogout.Location = new Point(this.panelNavBar.Width - this.btnLogout.Width - 20, 10);
            this.btnLogout.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            this.btnLogout.Click += new EventHandler(this.btnLogout_Click);

            // --- Menu View Button ---
            this.btnMenuView = new Button();
            this.btnMenuView.Size = new Size(200, 40);
            this.btnMenuView.Text = "Menu View";
            this.btnMenuView.BackColor = Color.LightGray;
            this.btnMenuView.Location = new Point(this.btnLogout.Left - this.btnMenuView.Width - 10, 10);
            this.btnMenuView.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            this.btnMenuView.Click += new EventHandler(this.btnMenuView_Click);

            // Add controls to navbar
            this.panelNavBar.Controls.Add(this.btnLogout);
            this.panelNavBar.Controls.Add(this.btnMenuView);
            this.Controls.Add(this.panelNavBar);
            this.panelNavBar.Controls.Add(this.lblTables);

            // --- Welcome Label ---
            this.lblWelcome = new Label();
            this.lblWelcome.Location = new Point(20, 70);
            this.lblWelcome.Size = new Size(400, 60);
            this.lblWelcome.Font = new Font("Segoe UI", 14, FontStyle.Bold);
            this.lblWelcome.Text = "Welcome!";
            this.Controls.Add(this.lblWelcome);

            // --- Tables Panel ---
            this.tlpTables = new TableLayoutPanel();
            this.tlpTables.Location = new Point(80, 150);
            this.tlpTables.Size = new Size(1000, 600);
            this.tlpTables.Anchor = AnchorStyles.None;
            this.tlpTables.AutoSize = false;
            this.tlpTables.ColumnCount = 5;
            this.tlpTables.RowCount = 2;
            this.tlpTables.CellBorderStyle = TableLayoutPanelCellBorderStyle.None;

            this.tlpTables.ColumnCount = 5;
            this.tlpTables.RowCount = 2;

            this.tlpTables.Dock = DockStyle.None;
            this.tlpTables.Anchor = AnchorStyles.None;
            this.tlpTables.Margin = new Padding(0);
            this.tlpTables.Padding = new Padding(10);
            this.Controls.Add(this.tlpTables);

            this.Load += new EventHandler(this.TableView_Load);
        }

        #endregion

        private TableLayoutPanel tlpTables;
        private Label lblWelcome;
        private Button btnLogout;
        private Button btnMenuView;
        private Panel panelNavBar;
        private Label lblTables;
    }
}