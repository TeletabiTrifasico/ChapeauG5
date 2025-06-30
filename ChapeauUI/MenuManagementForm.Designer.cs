namespace ChapeauUI
{
    partial class MenuManagementForm
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
            menuDataGridView = new DataGridView();
            button1 = new Button();
            button2 = new Button();
            button3 = new Button();
            nameTextBox = new TextBox();
            priceTextBox = new TextBox();
            stockTextBox = new TextBox();
            categoryComboBox = new ComboBox();
            categoryIDComboBox = new ComboBox();
            ((System.ComponentModel.ISupportInitialize)menuDataGridView).BeginInit();
            SuspendLayout();
            // 
            // menuDataGridView
            // 
            menuDataGridView.ReadOnly = false;
            menuDataGridView.AllowUserToAddRows = false;
            menuDataGridView.AutoGenerateColumns = true;
            menuDataGridView.EditMode = DataGridViewEditMode.EditOnEnter;

            menuDataGridView.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            menuDataGridView.Location = new Point(12, 12);
            menuDataGridView.Name = "menuDataGridView";
            menuDataGridView.Size = new Size(458, 371);
            menuDataGridView.TabIndex = 0;
            // 
            // button1
            // 
            button1.Location = new Point(12, 415);
            button1.Name = "button1";
            button1.Size = new Size(75, 23);
            button1.TabIndex = 1;
            button1.Text = "Add";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // button2
            // 
            button2.Location = new Point(93, 415);
            button2.Name = "button2";
            button2.Size = new Size(75, 23);
            button2.TabIndex = 2;
            button2.Text = "Update";
            button2.UseVisualStyleBackColor = true;
            button2.Click += button2_Click;
            // 
            // button3
            // 
            button3.Location = new Point(174, 415);
            button3.Name = "button3";
            button3.Size = new Size(75, 23);
            button3.TabIndex = 3;
            button3.Text = "Activate";
            button3.UseVisualStyleBackColor = true;
            button3.Click += button3_Click;
            // 
            // nameTextBox
            // 
            nameTextBox.Location = new Point(476, 42);
            nameTextBox.Name = "nameTextBox";
            nameTextBox.Size = new Size(100, 23);
            nameTextBox.TabIndex = 4;
            nameTextBox.Text = "Name";
            // 
            // priceTextBox
            // 
            priceTextBox.Location = new Point(582, 42);
            priceTextBox.Name = "priceTextBox";
            priceTextBox.Size = new Size(100, 23);
            priceTextBox.TabIndex = 5;
            priceTextBox.Text = "Price";
            // 
            // stockTextBox
            // 
            stockTextBox.Location = new Point(688, 42);
            stockTextBox.Name = "stockTextBox";
            stockTextBox.Size = new Size(100, 23);
            stockTextBox.TabIndex = 6;
            stockTextBox.Text = "Stock";
            // 
            // categoryComboBox
            // 
            categoryComboBox.FormattingEnabled = true;
            categoryComboBox.Items.AddRange(new object[] { "Starters", "Mains", "Desserts", "Snacks" });
            categoryComboBox.Location = new Point(570, 88);
            categoryComboBox.Name = "categoryComboBox";
            categoryComboBox.Size = new Size(121, 23);
            categoryComboBox.TabIndex = 8;
            categoryComboBox.Text = "Category";
            // 
            // categoryIDComboBox
            // 
            categoryIDComboBox.FormattingEnabled = true;
            categoryIDComboBox.Location = new Point(570, 146);
            categoryIDComboBox.Name = "categoryIDComboBox";
            categoryIDComboBox.Size = new Size(121, 23);
            categoryIDComboBox.TabIndex = 9;
            // 
            // ManagerDashboardForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(categoryIDComboBox);
            Controls.Add(categoryComboBox);
            Controls.Add(stockTextBox);
            Controls.Add(priceTextBox);
            Controls.Add(nameTextBox);
            Controls.Add(button3);
            Controls.Add(button2);
            Controls.Add(button1);
            Controls.Add(menuDataGridView);
            Name = "ManagerDashboardForm";
            Text = "ManagerDashboardForm";
            ((System.ComponentModel.ISupportInitialize)menuDataGridView).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private DataGridView menuDataGridView;
        private Button button1;
        private Button button2;
        private Button button3;
        private TextBox nameTextBox;
        private TextBox priceTextBox;
        private TextBox stockTextBox;
        private ComboBox categoryComboBox;
        private ComboBox categoryIDComboBox;
    }
}