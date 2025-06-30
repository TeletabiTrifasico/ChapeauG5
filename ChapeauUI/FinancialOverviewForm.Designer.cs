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
            salesLabel = new Label();
            incomeLabel = new Label();
            tipsLabel = new Label();
            periodComboBox = new ComboBox();
            viewButton = new Button();
            label1 = new Label();
            label2 = new Label();
            label3 = new Label();
            label4 = new Label();
            label5 = new Label();
            label6 = new Label();
            SuspendLayout();
            // 
            // startDatePicker
            // 
            startDatePicker.Location = new Point(12, 79);
            startDatePicker.Name = "startDatePicker";
            startDatePicker.Size = new Size(200, 23);
            startDatePicker.TabIndex = 0;
            // 
            // endDatePicker
            // 
            endDatePicker.Location = new Point(364, 79);
            endDatePicker.Name = "endDatePicker";
            endDatePicker.Size = new Size(200, 23);
            endDatePicker.TabIndex = 1;
            // 
            // salesLabel
            // 
            salesLabel.AutoSize = true;
            salesLabel.Font = new Font("Segoe UI", 9.75F);
            salesLabel.Location = new Point(7, 181);
            salesLabel.Name = "salesLabel";
            salesLabel.Size = new Size(31, 17);
            salesLabel.TabIndex = 2;
            salesLabel.Text = "N/A";
            // 
            // incomeLabel
            // 
            incomeLabel.AutoSize = true;
            incomeLabel.Font = new Font("Segoe UI", 9.75F);
            incomeLabel.Location = new Point(7, 246);
            incomeLabel.Name = "incomeLabel";
            incomeLabel.Size = new Size(31, 17);
            incomeLabel.TabIndex = 3;
            incomeLabel.Text = "N/A";
            // 
            // tipsLabel
            // 
            tipsLabel.AutoSize = true;
            tipsLabel.Font = new Font("Segoe UI", 9.75F);
            tipsLabel.Location = new Point(7, 316);
            tipsLabel.Name = "tipsLabel";
            tipsLabel.Size = new Size(31, 17);
            tipsLabel.TabIndex = 5;
            tipsLabel.Text = "N/A";
            // 
            // periodComboBox
            // 
            periodComboBox.Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            periodComboBox.FormattingEnabled = true;
            periodComboBox.Location = new Point(224, 79);
            periodComboBox.Name = "periodComboBox";
            periodComboBox.Size = new Size(121, 25);
            periodComboBox.TabIndex = 6;
            // 
            // viewButton
            // 
            viewButton.Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            viewButton.Location = new Point(224, 123);
            viewButton.Name = "viewButton";
            viewButton.Size = new Size(121, 29);
            viewButton.TabIndex = 7;
            viewButton.Text = "View";
            viewButton.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 11.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label1.Location = new Point(7, 156);
            label1.Name = "label1";
            label1.Size = new Size(44, 20);
            label1.TabIndex = 8;
            label1.Text = "Sales";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Segoe UI", 11.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label2.Location = new Point(7, 221);
            label2.Name = "label2";
            label2.Size = new Size(198, 20);
            label2.TabIndex = 9;
            label2.Text = "Total Income (Sales + VAT)";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new Font("Segoe UI", 11.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label3.Location = new Point(7, 290);
            label3.Name = "label3";
            label3.Size = new Size(31, 20);
            label3.TabIndex = 10;
            label3.Text = "Tip";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Dock = DockStyle.Top;
            label4.Font = new Font("Segoe UI", 20.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label4.Location = new Point(0, 0);
            label4.Name = "label4";
            label4.Size = new Size(259, 37);
            label4.TabIndex = 11;
            label4.Text = "Financial Overview";
            label4.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label5.Location = new Point(69, 59);
            label5.Name = "label5";
            label5.Size = new Size(70, 17);
            label5.TabIndex = 12;
            label5.Text = "Start Date";
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label6.Location = new Point(422, 59);
            label6.Name = "label6";
            label6.Size = new Size(64, 17);
            label6.TabIndex = 13;
            label6.Text = "End Date";
            // 
            // FinancialOverviewForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(576, 366);
            Controls.Add(label6);
            Controls.Add(label5);
            Controls.Add(label4);
            Controls.Add(label3);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(viewButton);
            Controls.Add(periodComboBox);
            Controls.Add(tipsLabel);
            Controls.Add(incomeLabel);
            Controls.Add(salesLabel);
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
        private Label salesLabel;
        private Label incomeLabel;
        private Label tipsLabel;
        private ComboBox periodComboBox;
        private Button viewButton;
        private Label label1;
        private Label label2;
        private Label label3;
        private Label label4;
        private Label label5;
        private Label label6;
    }
}