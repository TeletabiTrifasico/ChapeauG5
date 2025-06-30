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
            salesLabel.Location = new Point(7, 181);
            salesLabel.Name = "salesLabel";
            salesLabel.Size = new Size(38, 15);
            salesLabel.TabIndex = 2;
            salesLabel.Text = "label1";
            // 
            // incomeLabel
            // 
            incomeLabel.AutoSize = true;
            incomeLabel.Location = new Point(7, 246);
            incomeLabel.Name = "incomeLabel";
            incomeLabel.Size = new Size(38, 15);
            incomeLabel.TabIndex = 3;
            incomeLabel.Text = "label2";
            // 
            // tipsLabel
            // 
            tipsLabel.AutoSize = true;
            tipsLabel.Location = new Point(7, 316);
            tipsLabel.Name = "tipsLabel";
            tipsLabel.Size = new Size(38, 15);
            tipsLabel.TabIndex = 5;
            tipsLabel.Text = "label4";
            // 
            // periodComboBox
            // 
            periodComboBox.FormattingEnabled = true;
            periodComboBox.Location = new Point(224, 79);
            periodComboBox.Name = "periodComboBox";
            periodComboBox.Size = new Size(121, 23);
            periodComboBox.TabIndex = 6;
            // 
            // viewButton
            // 
            viewButton.Location = new Point(243, 129);
            viewButton.Name = "viewButton";
            viewButton.Size = new Size(75, 23);
            viewButton.TabIndex = 7;
            viewButton.Text = "View";
            viewButton.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(7, 156);
            label1.Name = "label1";
            label1.Size = new Size(33, 15);
            label1.TabIndex = 8;
            label1.Text = "Sales";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(7, 221);
            label2.Name = "label2";
            label2.Size = new Size(147, 15);
            label2.TabIndex = 9;
            label2.Text = "Total Income (Sales + VAT)";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(7, 290);
            label3.Name = "label3";
            label3.Size = new Size(24, 15);
            label3.TabIndex = 10;
            label3.Text = "Tip";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(259, 29);
            label4.Name = "label4";
            label4.Size = new Size(38, 15);
            label4.TabIndex = 11;
            label4.Text = "label4";
            // 
            // FinancialOverviewForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(576, 340);
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
    }
}