using System;
using System.Drawing;
using System.Windows.Forms;

namespace ChapeauG5
{
    partial class OrderView
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
            
            this.Text = "Order Management";
            this.Size = new Size(900, 700);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            
            // Table Label
            this.lblTable = new Label();
            this.lblTable.Location = new Point(20, 20);
            this.lblTable.Size = new Size(200, 30);
            this.lblTable.Font = new Font("Segoe UI", 14, FontStyle.Bold);
            this.lblTable.Text = "Table X";
            
            // Category Selection
            this.lblCategory = new Label();
            this.lblCategory.Location = new Point(20, 60);
            this.lblCategory.Size = new Size(100, 25);
            this.lblCategory.Text = "Menu Category:";
            
            this.cmbCategory = new ComboBox();
            this.cmbCategory.Location = new Point(130, 60);
            this.cmbCategory.Size = new Size(200, 25);
            this.cmbCategory.DropDownStyle = ComboBoxStyle.DropDownList;
            this.cmbCategory.DisplayMember = "Name";
            this.cmbCategory.ValueMember = "CategoryId";
            this.cmbCategory.SelectedIndexChanged += new EventHandler(this.cmbCategory_SelectedIndexChanged);
            
            // Menu Items List
            this.lvMenuItems = new ListView();
            this.lvMenuItems.Location = new Point(20, 100);
            this.lvMenuItems.Size = new Size(400, 300);
            this.lvMenuItems.View = View.Details;
            this.lvMenuItems.FullRowSelect = true;
            this.lvMenuItems.MultiSelect = false;
            this.lvMenuItems.Columns.Add("Item", 150);
            this.lvMenuItems.Columns.Add("Price", 70);
            this.lvMenuItems.Columns.Add("Description", 180);
            
            // Quantity Selection
            this.lblQuantity = new Label();
            this.lblQuantity.Location = new Point(20, 410);
            this.lblQuantity.Size = new Size(70, 25);
            this.lblQuantity.Text = "Quantity:";
            
            this.nudQuantity = new NumericUpDown();
            this.nudQuantity.Location = new Point(100, 410);
            this.nudQuantity.Size = new Size(60, 25);
            this.nudQuantity.Minimum = 1;
            this.nudQuantity.Maximum = 20;
            this.nudQuantity.Value = 1;
            
            // Comment
            this.lblComment = new Label();
            this.lblComment.Location = new Point(170, 410);
            this.lblComment.Size = new Size(70, 25);
            this.lblComment.Text = "Comment:";
            
            this.txtComment = new TextBox();
            this.txtComment.Location = new Point(250, 410);
            this.txtComment.Size = new Size(170, 25);
            
            // Add to Order Button
            this.btnAddToOrder = new Button();
            this.btnAddToOrder.Location = new Point(250, 450);
            this.btnAddToOrder.Size = new Size(170, 40);
            this.btnAddToOrder.Text = "Add to Order";
            this.btnAddToOrder.BackColor = Color.LightGreen;
            this.btnAddToOrder.Click += new EventHandler(this.btnAddToOrder_Click);
            
            // Order Items List
            this.lvOrderItems = new ListView();
            this.lvOrderItems.Location = new Point(450, 100);
            this.lvOrderItems.Size = new Size(420, 400);
            this.lvOrderItems.View = View.Details;
            this.lvOrderItems.FullRowSelect = true;
            this.lvOrderItems.Columns.Add("Item", 120);
            this.lvOrderItems.Columns.Add("Qty", 40);
            this.lvOrderItems.Columns.Add("Price", 60);
            this.lvOrderItems.Columns.Add("Subtotal", 70);
            this.lvOrderItems.Columns.Add("Status", 60);
            this.lvOrderItems.Columns.Add("Comment", 70);
            
            // Order Total
            this.lblOrderTotal = new Label();
            this.lblOrderTotal.Location = new Point(450, 510);
            this.lblOrderTotal.Size = new Size(250, 30);
            this.lblOrderTotal.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            this.lblOrderTotal.Text = "Order Total: €0.00";
            this.lblOrderTotal.Visible = true;
            
            // Payment Button
            this.btnPayment = new Button();
            this.btnPayment.Location = new Point(750, 510);
            this.btnPayment.Size = new Size(120, 40);
            this.btnPayment.Text = "Payment";
            this.btnPayment.BackColor = Color.LightBlue;
            this.btnPayment.Click += new EventHandler(this.btnPayment_Click);
            
            // Cancel Button
            this.btnCancel = new Button();
            this.btnCancel.Location = new Point(750, 560);
            this.btnCancel.Size = new Size(120, 40);
            this.btnCancel.Text = "Close";
            this.btnCancel.BackColor = Color.LightGray;
            this.btnCancel.Click += new EventHandler(this.btnCancel_Click);
            
            this.Controls.Add(this.lblTable);
            this.Controls.Add(this.lblCategory);
            this.Controls.Add(this.cmbCategory);
            this.Controls.Add(this.lvMenuItems);
            this.Controls.Add(this.lblQuantity);
            this.Controls.Add(this.nudQuantity);
            this.Controls.Add(this.lblComment);
            this.Controls.Add(this.txtComment);
            this.Controls.Add(this.btnAddToOrder);
            this.Controls.Add(this.lvOrderItems);
            this.Controls.Add(this.lblOrderTotal);
            this.Controls.Add(this.btnPayment);
            this.Controls.Add(this.btnCancel);
            
            this.Load += new EventHandler(this.OrderView_Load);
            this.FormClosing += new FormClosingEventHandler(this.OrderView_FormClosing);
        }
        
        #endregion
        
        private Label lblTable;
        private Label lblCategory;
        private ComboBox cmbCategory;
        private ListView lvMenuItems;
        private Label lblQuantity;
        private NumericUpDown nudQuantity;
        private Label lblComment;
        private TextBox txtComment;
        private Button btnAddToOrder;
        private ListView lvOrderItems;
        private Label lblOrderTotal;
        private Button btnPayment;
        private Button btnCancel;
    }
}