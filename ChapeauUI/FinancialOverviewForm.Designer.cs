namespace ChapeauUI
{
    partial class FinancialOverviewForm
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
            startDatePicker = new DateTimePicker();
            endDatePicker = new DateTimePicker();
            drinksLabel = new Label();
            lunchLabel = new Label();
            dinnerLabel = new Label();
            tipsLabel = new Label();
            SuspendLayout();
            // 
            // startDatePicker
            // 
            startDatePicker.Location = new Point(60, 92);
            startDatePicker.Name = "startDatePicker";
            startDatePicker.Size = new Size(200, 23);
            startDatePicker.TabIndex = 0;
            // 
            // endDatePicker
            // 
            endDatePicker.Location = new Point(326, 92);
            endDatePicker.Name = "endDatePicker";
            endDatePicker.Size = new Size(200, 23);
            endDatePicker.TabIndex = 1;
            // 
            // drinksLabel
            // 
            drinksLabel.AutoSize = true;
            drinksLabel.Location = new Point(144, 275);
            drinksLabel.Name = "drinksLabel";
            drinksLabel.Size = new Size(38, 15);
            drinksLabel.TabIndex = 2;
            drinksLabel.Text = "label1";
            // 
            // lunchLabel
            // 
            lunchLabel.AutoSize = true;
            lunchLabel.Location = new Point(266, 275);
            lunchLabel.Name = "lunchLabel";
            lunchLabel.Size = new Size(38, 15);
            lunchLabel.TabIndex = 3;
            lunchLabel.Text = "label2";
            // 
            // dinnerLabel
            // 
            dinnerLabel.AutoSize = true;
            dinnerLabel.Location = new Point(346, 283);
            dinnerLabel.Name = "dinnerLabel";
            dinnerLabel.Size = new Size(38, 15);
            dinnerLabel.TabIndex = 4;
            dinnerLabel.Text = "label3";
            // 
            // tipsLabel
            // 
            tipsLabel.AutoSize = true;
            tipsLabel.Location = new Point(468, 275);
            tipsLabel.Name = "tipsLabel";
            tipsLabel.Size = new Size(38, 15);
            tipsLabel.TabIndex = 5;
            tipsLabel.Text = "label4";
            // 
            // FinancialOverviewForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(tipsLabel);
            Controls.Add(dinnerLabel);
            Controls.Add(lunchLabel);
            Controls.Add(drinksLabel);
            Controls.Add(endDatePicker);
            Controls.Add(startDatePicker);
            Name = "FinancialOverviewForm";
            Text = "FinancialOverviewForm";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private DateTimePicker startDatePicker;
        private DateTimePicker endDatePicker;
        private Label drinksLabel;
        private Label lunchLabel;
        private Label dinnerLabel;
        private Label tipsLabel;
    }
}