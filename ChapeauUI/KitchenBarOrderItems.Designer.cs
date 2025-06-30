
namespace ChapeauG5.ChapeauUI
{
    partial class KitchenBarOrderItems
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            orderItemName = new Label();
            orderItemStatus = new Label();
            btnStartOrderItem = new Button();
            btnReadyOrderItem = new Button();
            orderItemComment = new Label();
            orderItemQuantity = new Label();
            SuspendLayout();
            // 
            // orderItemName
            // 
            orderItemName.AutoSize = true;
            orderItemName.Location = new Point(22, 36);
            orderItemName.Name = "orderItemName";
            orderItemName.Size = new Size(138, 25);
            orderItemName.TabIndex = 0;
            orderItemName.Text = "orderItemName";
            // 
            // orderItemStatus
            // 
            orderItemStatus.AutoSize = true;
            orderItemStatus.Location = new Point(515, 36);
            orderItemStatus.Name = "orderItemStatus";
            orderItemStatus.Size = new Size(111, 25);
            orderItemStatus.TabIndex = 0;
            orderItemStatus.Text = "Order Status";
            // 
            // btnStartOrderItem
            // 
            btnStartOrderItem.Location = new Point(672, 23);
            btnStartOrderItem.Name = "btnStartOrderItem";
            btnStartOrderItem.Size = new Size(100, 50);
            btnStartOrderItem.TabIndex = 1;
            btnStartOrderItem.Text = "Start 🔥  ";
            btnStartOrderItem.UseVisualStyleBackColor = true;
            btnStartOrderItem.Click += btnStartOrderItem_Click;
            // 
            // btnReadyOrderItem
            // 
            btnReadyOrderItem.Location = new Point(778, 23);
            btnReadyOrderItem.Name = "btnReadyOrderItem";
            btnReadyOrderItem.Size = new Size(94, 50);
            btnReadyOrderItem.TabIndex = 1;
            btnReadyOrderItem.Text = "Ready ✓";
            btnReadyOrderItem.UseVisualStyleBackColor = true;
            btnReadyOrderItem.Click += btnReadyOrderItem_Click;
            // 
            // orderItemComment
            // 
            orderItemComment.AutoSize = true;
            orderItemComment.Location = new Point(353, 36);
            orderItemComment.Name = "orderItemComment";
            orderItemComment.Size = new Size(91, 25);
            orderItemComment.TabIndex = 0;
            orderItemComment.Text = "Comment";
            // 
            // orderItemQuantity
            // 
            orderItemQuantity.AutoSize = true;
            orderItemQuantity.Location = new Point(267, 36);
            orderItemQuantity.Name = "orderItemQuantity";
            orderItemQuantity.Size = new Size(80, 25);
            orderItemQuantity.TabIndex = 0;
            orderItemQuantity.Text = "Quantity";
            // 
            // KitchenBarOrderItems
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.ControlLight;
            Controls.Add(btnReadyOrderItem);
            Controls.Add(btnStartOrderItem);
            Controls.Add(orderItemQuantity);
            Controls.Add(orderItemComment);
            Controls.Add(orderItemStatus);
            Controls.Add(orderItemName);
            Name = "KitchenBarOrderItems";
            Size = new Size(894, 88);
            Load += KitchenBarOrderItems_Load;
            ResumeLayout(false);
            PerformLayout();
        }



        #endregion

        private Label orderItemName;
        private Label orderItemStatus;
        private Button btnStartOrderItem;
        private Button btnReadyOrderItem;
        private Label orderItemComment;
        private Label orderItemQuantity;
    }
}
