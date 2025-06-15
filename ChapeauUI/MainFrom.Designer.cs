namespace ChapeauG5.ChapeauUI.Forms
{
    partial class MainForm
    {
        private System.ComponentModel.IContainer components = null;
        private MenuStrip menuStrip;
        private ToolStripMenuItem menuTables;
        private ToolStripMenuItem menuOrders;
        private ToolStripMenuItem menuKitchen;
        private ToolStripMenuItem menuBar;
        private ToolStripMenuItem menuPayment;
        private ToolStripMenuItem menuManagement;
        private ToolStripMenuItem menuReports;
        private ToolStripMenuItem menuHelp;
        private ToolStripMenuItem menuLogout;
        private StatusStrip statusStrip;
        private ToolStripStatusLabel labelUserName;
        private ToolStripStatusLabel labelUserRole;
        private ToolStripStatusLabel labelDateTime;
        private Panel panelContent;
        private System.Windows.Forms.Timer timer;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.menuStrip = new MenuStrip();
            this.menuTables = new ToolStripMenuItem();
            this.menuOrders = new ToolStripMenuItem();
            this.menuKitchen = new ToolStripMenuItem();
            this.menuBar = new ToolStripMenuItem();
            this.menuPayment = new ToolStripMenuItem();
            this.menuManagement = new ToolStripMenuItem();
            this.menuReports = new ToolStripMenuItem();
            this.menuHelp = new ToolStripMenuItem();
            this.menuLogout = new ToolStripMenuItem();
            this.statusStrip = new StatusStrip();
            this.labelUserName = new ToolStripStatusLabel();
            this.labelUserRole = new ToolStripStatusLabel();
            this.labelDateTime = new ToolStripStatusLabel();
            this.panelContent = new Panel();
            this.timer = new System.Windows.Forms.Timer(this.components);
            this.menuStrip.SuspendLayout();
            this.statusStrip.SuspendLayout();
            this.SuspendLayout();

            // 
            // menuStrip
            // 
            this.menuStrip.Items.AddRange(new ToolStripItem[] {
                this.menuTables,
                this.menuOrders,
                this.menuKitchen,
                this.menuBar,
                this.menuPayment,
                this.menuManagement,
                this.menuReports,
                this.menuHelp,
                this.menuLogout});
            this.menuStrip.Location = new Point(0, 0);
            this.menuStrip.Size = new Size(1280, 24);
            this.menuStrip.TabIndex = 0;
            this.menuStrip.Text = "menuStrip";

            // 
            // menuTables
            // 
            this.menuTables.Name = "menuTables";
            this.menuTables.Size = new Size(52, 20);
            this.menuTables.Text = "Tables";
            this.menuTables.Click += new EventHandler(this.menuTables_Click);

            // 
            // menuOrders
            // 
            this.menuOrders.Name = "menuOrders";
            this.menuOrders.Size = new Size(54, 20);
            this.menuOrders.Text = "Orders";
            this.menuOrders.Click += new EventHandler(this.menuOrders_Click);

            // 
            // menuKitchen
            // 
            this.menuKitchen.Name = "menuKitchen";
            this.menuKitchen.Size = new Size(60, 20);
            this.menuKitchen.Text = "Kitchen";
            this.menuKitchen.Click += new EventHandler(this.menuKitchen_Click);

            // 
            // menuBar
            // 
            this.menuBar.Name = "menuBar";
            this.menuBar.Size = new Size(35, 20);
            this.menuBar.Text = "Bar";
            this.menuBar.Click += new EventHandler(this.menuBar_Click);

            // 
            // menuPayment
            // 
            this.menuPayment.Name = "menuPayment";
            this.menuPayment.Size = new Size(66, 20);
            this.menuPayment.Text = "Payment";
            this.menuPayment.Click += new EventHandler(this.menuPayment_Click);

            // 
            // menuManagement
            // 
            this.menuManagement.Name = "menuManagement";
            this.menuManagement.Size = new Size(90, 20);
            this.menuManagement.Text = "Management";
            this.menuManagement.Click += new EventHandler(this.menuManagement_Click);

            // 
            // menuReports
            // 
            this.menuReports.Name = "menuReports";
            this.menuReports.Size = new Size(59, 20);
            this.menuReports.Text = "Reports";
            this.menuReports.Click += new EventHandler(this.menuReports_Click);

            // 
            // menuHelp
            // 
            this.menuHelp.Name = "menuHelp";
            this.menuHelp.Size = new Size(44, 20);
            this.menuHelp.Text = "Help";
            this.menuHelp.Click += new EventHandler(this.menuHelp_Click);

            // 
            // menuLogout
            // 
            this.menuLogout.Name = "menuLogout";
            this.menuLogout.Size = new Size(57, 20);
            this.menuLogout.Text = "Logout";
            this.menuLogout.Click += new EventHandler(this.menuLogout_Click);

            // 
            // statusStrip
            // 
            this.statusStrip.Items.AddRange(new ToolStripItem[] {
                this.labelUserName,
                this.labelUserRole,
                this.labelDateTime});
            this.statusStrip.Location = new Point(0, 707);
            this.statusStrip.Size = new Size(1280, 22);
            this.statusStrip.TabIndex = 1;
            this.statusStrip.Text = "statusStrip";

            // 
            // labelUserName
            // 
            this.labelUserName.Name = "labelUserName";
            this.labelUserName.Size = new Size(69, 17);
            this.labelUserName.Text = "User Name";

            // 
            // labelUserRole
            // 
            this.labelUserRole.Name = "labelUserRole";
            this.labelUserRole.Size = new Size(58, 17);
            this.labelUserRole.Text = "User Role";

            // 
            // labelDateTime
            // 
            this.labelDateTime.Name = "labelDateTime";
            this.labelDateTime.Size = new Size(1138, 17);
            this.labelDateTime.Spring = true;
            this.labelDateTime.Text = "Date Time";
            this.labelDateTime.TextAlign = ContentAlignment.MiddleRight;

            // 
            // panelContent
            // 
            this.panelContent.Dock = DockStyle.Fill;
            this.panelContent.Location = new Point(0, 24);
            this.panelContent.Size = new Size(1280, 683);
            this.panelContent.TabIndex = 2;

            // 
            // timer
            // 
            this.timer.Enabled = true;
            this.timer.Interval = 1000;
            this.timer.Tick += new EventHandler(this.timer_Tick);

            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new SizeF(6F, 13F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.ClientSize = new Size(1280, 729);
            this.Controls.Add(this.panelContent);
            this.Controls.Add(this.statusStrip);
            this.Controls.Add(this.menuStrip);
            this.MainMenuStrip = this.menuStrip;
            this.Name = "MainForm";
            this.Text = "Chapeau Restaurant System";
            this.WindowState = FormWindowState.Maximized;
            this.FormClosing += new FormClosingEventHandler(this.MainForm_FormClosing);
            this.menuStrip.ResumeLayout(false);
            this.menuStrip.PerformLayout();
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}