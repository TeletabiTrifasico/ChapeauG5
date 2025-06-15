namespace ChapeauG5.ChapeauUI
{
    partial class KitchenOrderTables
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
            OrderTableNumberLabel = new Label();
            totalOrderItemsCountLabel = new Label();
            OrderedItemStatusStatsLabel = new Label();
            SuspendLayout();
            // 
            // OrderTableNumberLabel
            // 
            OrderTableNumberLabel.AutoSize = true;
            OrderTableNumberLabel.Location = new Point(26, 22);
            OrderTableNumberLabel.Name = "OrderTableNumberLabel";
            OrderTableNumberLabel.Size = new Size(68, 25);
            OrderTableNumberLabel.TabIndex = 0;
            OrderTableNumberLabel.Text = "Table #";
            // 
            // totalOrderItemsCountLabel
            // 
            totalOrderItemsCountLabel.AutoSize = true;
            totalOrderItemsCountLabel.Location = new Point(383, 31);
            totalOrderItemsCountLabel.Name = "totalOrderItemsCountLabel";
            totalOrderItemsCountLabel.Size = new Size(22, 25);
            totalOrderItemsCountLabel.TabIndex = 0;
            totalOrderItemsCountLabel.Text = "0";
            // 
            // OrderedItemStatusStatsLabel
            // 
            OrderedItemStatusStatsLabel.AutoSize = true;
            OrderedItemStatusStatsLabel.Location = new Point(46, 84);
            OrderedItemStatusStatsLabel.Name = "OrderedItemStatusStatsLabel";
            OrderedItemStatusStatsLabel.Size = new Size(284, 25);
            OrderedItemStatusStatsLabel.TabIndex = 0;
            OrderedItemStatusStatsLabel.Text = "Ordered : 0 Preparing : 0 Ready : 0";
            // 
            // KitchenOrderTables
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(OrderedItemStatusStatsLabel);
            Controls.Add(totalOrderItemsCountLabel);
            Controls.Add(OrderTableNumberLabel);
            Name = "KitchenOrderTables";
            Size = new Size(438, 169);
            Load += KitchenOrderTables_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label OrderTableNumberLabel;
        private Label totalOrderItemsCountLabel;
        private Label OrderedItemStatusStatsLabel;
    }
}
