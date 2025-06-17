using System;
using System.Drawing;
using System.Windows.Forms;
using ChapeauModel;

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
            lblTable = new Label();
            lblCategory = new Label();
            cmbCategory = new ComboBox();
            lvMenuItems = new ListView();
            lblQuantity = new Label();
            nudQuantity = new NumericUpDown();
            lblComment = new Label();
            txtComment = new TextBox();
            btnAddToOrder = new Button();
            lvOrderItems = new ListView();
            lblOrderTotal = new Label();
            btnEditItem = new Button();
            btnRemoveItem = new Button();
            btnSaveOrder = new Button();
            btnPayment = new Button();
            btnCancel = new Button();
            btnMarkServed = new Button();
            orderedList = new ListView();
            label1 = new Label();
            label2 = new Label();
            ((System.ComponentModel.ISupportInitialize)nudQuantity).BeginInit();
            SuspendLayout();
            // 
            // lblTable
            // 
            lblTable.Font = new Font("Segoe UI", 14F, FontStyle.Bold);
            lblTable.Location = new Point(20, 20);
            lblTable.Name = "lblTable";
            lblTable.Size = new Size(200, 52);
            lblTable.TabIndex = 0;
            lblTable.Text = "Table X";
            // 
            // lblCategory
            // 
            lblCategory.Location = new Point(20, 80);
            lblCategory.Name = "lblCategory";
            lblCategory.Size = new Size(100, 42);
            lblCategory.TabIndex = 1;
            lblCategory.Text = "Menu Category:";
            // 
            // cmbCategory
            // 
            cmbCategory.DisplayMember = "Name";
            cmbCategory.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbCategory.Location = new Point(126, 80);
            cmbCategory.Name = "cmbCategory";
            cmbCategory.Size = new Size(200, 40);
            cmbCategory.TabIndex = 2;
            cmbCategory.ValueMember = "CategoryId";
            cmbCategory.SelectedIndexChanged += new EventHandler(cmbCategory_SelectedIndexChanged);
            // 
            // lvMenuItems
            // 
            lvMenuItems.FullRowSelect = true;
            lvMenuItems.Location = new Point(20, 149);
            lvMenuItems.MultiSelect = false;
            lvMenuItems.Name = "lvMenuItems";
            lvMenuItems.Size = new Size(505, 331);
            lvMenuItems.TabIndex = 3;
            lvMenuItems.UseCompatibleStateImageBehavior = false;
            lvMenuItems.View = View.Details;
            lvMenuItems.Columns.Add("Item");
            lvMenuItems.Columns.Add("Price");
            lvMenuItems.Columns.Add("Description");
            // 
            // lblQuantity
            // 
            lblQuantity.Location = new Point(20, 502);
            lblQuantity.Name = "lblQuantity";
            lblQuantity.Size = new Size(117, 37);
            lblQuantity.TabIndex = 4;
            lblQuantity.Text = "Quantity:";
            // 
            // nudQuantity
            // 
            nudQuantity.Location = new Point(143, 502);
            nudQuantity.Maximum = new decimal(new int[] { 20, 0, 0, 0 });
            nudQuantity.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            nudQuantity.Name = "nudQuantity";
            nudQuantity.Size = new Size(60, 39);
            nudQuantity.TabIndex = 5;
            nudQuantity.Value = new decimal(new int[] { 1, 0, 0, 0 });
            // 
            // lblComment
            // 
            lblComment.Location = new Point(20, 579);
            lblComment.Name = "lblComment";
            lblComment.Size = new Size(180, 44);
            lblComment.TabIndex = 6;
            lblComment.Text = "Comment:";
            // 
            // txtComment
            // 
            txtComment.Location = new Point(220, 579);
            txtComment.Name = "txtComment";
            txtComment.Size = new Size(305, 39);
            txtComment.TabIndex = 7;
            // 
            // btnAddToOrder
            // 
            btnAddToOrder.BackColor = Color.LightGreen;
            btnAddToOrder.Location = new Point(20, 653);
            btnAddToOrder.Name = "btnAddToOrder";
            btnAddToOrder.Size = new Size(505, 92);
            btnAddToOrder.TabIndex = 8;
            btnAddToOrder.Text = "Add to Order";
            btnAddToOrder.UseVisualStyleBackColor = false;
            btnAddToOrder.Click += new EventHandler(btnAddToOrder_Click);
            // 
            // lvOrderItems
            // 
            lvOrderItems.FullRowSelect = true;
            lvOrderItems.Location = new Point(633, 520);
            lvOrderItems.Name = "lvOrderItems";
            lvOrderItems.Size = new Size(686, 300);
            lvOrderItems.TabIndex = 9;
            lvOrderItems.UseCompatibleStateImageBehavior = false;
            lvOrderItems.View = View.Details;
            lvOrderItems.FullRowSelect = true;
            lvOrderItems.Columns.Add("Item");
            lvOrderItems.Columns.Add("Qty");
            lvOrderItems.Columns.Add("Price");
            lvOrderItems.Columns.Add("Subtotal");
            lvOrderItems.Columns.Add("Status");
            lvOrderItems.Columns.Add("Comment");
            // 
            // lblOrderTotal
            // 
            lblOrderTotal.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            lblOrderTotal.Location = new Point(633, 842);
            lblOrderTotal.Name = "lblOrderTotal";
            lblOrderTotal.Size = new Size(350, 58);
            lblOrderTotal.TabIndex = 10;
            lblOrderTotal.Text = "Order Total: €0.00";
            // 
            // btnEditItem
            // 
            btnEditItem.BackColor = Color.LightYellow;
            btnEditItem.Enabled = false;
            btnEditItem.Location = new Point(825, 931);
            btnEditItem.Name = "btnEditItem";
            btnEditItem.Size = new Size(120, 60);
            btnEditItem.TabIndex = 11;
            btnEditItem.Text = "Edit Item";
            btnEditItem.UseVisualStyleBackColor = false;
            btnEditItem.Click += new EventHandler(btnEditItem_Click);
            // 
            // btnRemoveItem
            // 
            btnRemoveItem.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
            btnRemoveItem.BackColor = Color.LightPink;
            btnRemoveItem.Enabled = false;
            btnRemoveItem.Location = new Point(951, 931);
            btnRemoveItem.Name = "btnRemoveItem";
            btnRemoveItem.Size = new Size(200, 60);
            btnRemoveItem.TabIndex = 12;
            btnRemoveItem.Text = "Remove Item";
            btnRemoveItem.UseVisualStyleBackColor = false;
            btnRemoveItem.Click += new EventHandler(btnRemoveItem_Click);
            // 
            // btnSaveOrder
            // 
            btnSaveOrder.BackColor = Color.LightGreen;
            btnSaveOrder.Enabled = false;
            btnSaveOrder.Location = new Point(1157, 931);
            btnSaveOrder.Name = "btnSaveOrder";
            btnSaveOrder.Size = new Size(160, 60);
            btnSaveOrder.TabIndex = 13;
            btnSaveOrder.Text = "Save Order";
            btnSaveOrder.UseVisualStyleBackColor = false;
            btnSaveOrder.Click += new EventHandler(btnSaveOrder_Click);
            // 
            // btnPayment
            // 
            btnPayment.BackColor = Color.LightBlue;
            btnPayment.Location = new Point(1159, 842);
            btnPayment.Name = "btnPayment";
            btnPayment.Size = new Size(160, 60);
            btnPayment.TabIndex = 14;
            btnPayment.Text = "Payment";
            btnPayment.UseVisualStyleBackColor = false;
            btnPayment.Click += new EventHandler(btnPayment_Click);
            // 
            // btnCancel
            // 
            btnCancel.BackColor = Color.LightGray;
            btnCancel.Location = new Point(1167, 1013);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new Size(150, 60);
            btnCancel.TabIndex = 15;
            btnCancel.Text = "Close";
            btnCancel.UseVisualStyleBackColor = false;
            btnCancel.Click += new EventHandler(btnCancel_Click);
            // 
            // btnMarkServed
            // 
            btnMarkServed.BackColor = Color.LightCyan;
            btnMarkServed.Enabled = false;
            btnMarkServed.Location = new Point(606, 931);
            btnMarkServed.Name = "btnMarkServed";
            btnMarkServed.Size = new Size(213, 60);
            btnMarkServed.TabIndex = 16;
            btnMarkServed.Text = "Mark as Served";
            btnMarkServed.UseVisualStyleBackColor = false;
            btnMarkServed.Click += new EventHandler(btnMarkServed_Click);
            // 
            // orderedList
            // 
            orderedList.FullRowSelect = true;
            orderedList.Location = new Point(631, 92);
            orderedList.Name = "orderedList";
            orderedList.Size = new Size(686, 300);
            orderedList.TabIndex = 17;
            orderedList.UseCompatibleStateImageBehavior = false;
            orderedList.View = View.Details;
            orderedList.Columns.Add("Item");
            orderedList.Columns.Add("Qty");
            orderedList.Columns.Add("Price");
            orderedList .Columns.Add("Subtotal");
            orderedList.Columns.Add("Status");
            orderedList.Columns.Add("Comment");
            // 
            // label1
            // 
            label1.Font = new Font("Segoe UI", 14F, FontStyle.Bold);
            label1.Location = new Point(633, 445);
            label1.Name = "label1";
            label1.Size = new Size(284, 52);
            label1.TabIndex = 18;
            label1.Text = "New Order";
            // 
            // label2
            // 
            label2.Font = new Font("Segoe UI", 14F, FontStyle.Bold);
            label2.Location = new Point(631, 20);
            label2.Name = "label2";
            label2.Size = new Size(200, 52);
            label2.TabIndex = 19;
            label2.Text = "Ordered";
            // 
            // OrderView
            // 
            ClientSize = new Size(1329, 1119);
            Controls.Add(lblTable);
            Controls.Add(lblCategory);
            Controls.Add(cmbCategory);
            Controls.Add(lvMenuItems);
            Controls.Add(lblQuantity);
            Controls.Add(nudQuantity);
            Controls.Add(lblComment);
            Controls.Add(txtComment);
            Controls.Add(btnAddToOrder);
            Controls.Add(label2);
            Controls.Add(orderedList);
            Controls.Add(label1);
            Controls.Add(lvOrderItems);
            Controls.Add(lblOrderTotal);
            Controls.Add(btnEditItem);
            Controls.Add(btnRemoveItem);
            Controls.Add(btnSaveOrder);
            Controls.Add(btnPayment);
            Controls.Add(btnCancel);
            Controls.Add(btnMarkServed);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            Name = "OrderView";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Order Management";
            FormClosing += OrderView_FormClosing;
            Load += OrderView_Load;
            ((System.ComponentModel.ISupportInitialize)nudQuantity).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        private void OrderView_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                // Only ask for confirmation if there are unsaved changes and it's not from Cancel button
                if (newOrderItems.Count > 0 && !isExistingOrder && e.CloseReason == CloseReason.UserClosing)
                {
                    DialogResult result = MessageBox.Show(
                        "Are you sure you want to close this order view? All unsaved changes will be lost.",
                        "Confirm Close",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question);
                        
                    if (result == DialogResult.No)
                    {
                        // Cancel the form closing
                        e.Cancel = true;
                        return;
                    }
                }
                
                // The key fix: ONLY reset the table status if we opened a new table view
                // AND never saved an order for it during this session
                if (!isExistingOrder)
                {
                    // This is a table we just opened but never saved an order for
                    // Set it back to Available to prevent it from turning red
                    tableService.UpdateTableStatus(selectedTable.TableId, TableStatus.Free);
                    
                    // We don't need to check for active orders in the database here
                    // If isExistingOrder is false, we know we never saved an order in this session
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in form closing: {ex.Message}");
                MessageBox.Show($"Error updating table status: {ex.Message}", 
                    "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        
        private void btnPayment_Click(object sender, EventArgs e)
        {
            try
            {
                // Check if all items have been served
                bool allServed = true;
                foreach (OrderItem item in newOrderItems)
                {
                    if (item.Status != OrderItem.OrderStatus.Served)
                    {
                        allServed = false;
                        break;
                    }
                }
                
                if (!allServed)
                {
                    MessageBox.Show("All order items must be served before proceeding to payment.", 
                        "Items Not Served", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                
                // First check if there's a saved order
                if (!currentOrderId.HasValue || !isExistingOrder)
                {
                    // No saved order, ask if the user wants to save it first
                    DialogResult result = MessageBox.Show(
                        "You need to save the order before proceeding to payment.\nWould you like to save it now?",
                        "Save Order",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question);
                        
                    if (result == DialogResult.Yes)
                    {
                        // Save the order first
                        btnSaveOrder_Click(sender, e);
                        
                        // If still no valid order after attempted save, return
                        if (!currentOrderId.HasValue)
                        {
                            MessageBox.Show("Cannot proceed to payment without a saved order.",
                                "Payment Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                    }
                    else
                    {
                        // User chose not to save
                        return;
                    }
                }
                
                // Check if there are any items in the order
                if (orderedItems.Count == 0)
                {
                    MessageBox.Show("Cannot process payment for an empty order.", 
                        "Empty Order", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                
                PaymentForm paymentForm = new PaymentForm(currentOrderId.Value, loggedInEmployee);
                
                // Show the payment form as a dialog
                if (paymentForm.ShowDialog() == DialogResult.OK)
                {
                    // Payment was successful, refresh the table view
                    MessageBox.Show("Payment processed successfully!", 
                        "Payment Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    
                    // Close this form
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error processing payment: {ex.Message}", 
                    "Payment Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
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
        private Button btnEditItem;
        private Button btnRemoveItem;
        private Button btnSaveOrder;
        private Button btnPayment;
        private Button btnCancel;
        private Button btnMarkServed;
        private ListView orderedList;
        private Label label1;
        private Label label2;
    }
}