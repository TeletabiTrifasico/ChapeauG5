using ChapeauModel;

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
            courseFilterComboBox = new ComboBox();
            vatTextBox = new TextBox();
            descriptionTextBox = new TextBox();
            deleteButton = new Button();
            courseTypeComboBox = new ComboBox();
            label1 = new Label();
            label2 = new Label();
            label3 = new Label();
            label4 = new Label();
            label5 = new Label();
            label6 = new Label();
            label7 = new Label();
            label8 = new Label();
            ((System.ComponentModel.ISupportInitialize)menuDataGridView).BeginInit();
            SuspendLayout();
            // 
            // menuDataGridView
            // 
            menuDataGridView.AllowUserToAddRows = false;
            menuDataGridView.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            menuDataGridView.EditMode = DataGridViewEditMode.EditOnEnter;
            menuDataGridView.Location = new Point(425, 100);
            menuDataGridView.Name = "menuDataGridView";
            menuDataGridView.Size = new Size(662, 371);
            menuDataGridView.TabIndex = 0;
            // 
            // button1
            // 
            button1.Location = new Point(12, 448);
            button1.Name = "button1";
            button1.Size = new Size(312, 23);
            button1.TabIndex = 1;
            button1.Text = "Add";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // button2
            // 
            button2.Font = new Font("Segoe UI", 9.75F);
            button2.Location = new Point(645, 477);
            button2.Name = "button2";
            button2.Size = new Size(75, 29);
            button2.TabIndex = 2;
            button2.Text = "Update";
            button2.UseVisualStyleBackColor = true;
            button2.Click += button2_Click;
            // 
            // button3
            // 
            button3.Font = new Font("Segoe UI", 9.75F);
            button3.Location = new Point(726, 477);
            button3.Name = "button3";
            button3.Size = new Size(75, 29);
            button3.TabIndex = 3;
            button3.Text = "Activate";
            button3.UseVisualStyleBackColor = true;
            button3.Click += button3_Click;
            // 
            // nameTextBox
            // 
            nameTextBox.Location = new Point(12, 101);
            nameTextBox.Name = "nameTextBox";
            nameTextBox.Size = new Size(100, 23);
            nameTextBox.TabIndex = 4;
            // 
            // priceTextBox
            // 
            priceTextBox.Location = new Point(118, 101);
            priceTextBox.Name = "priceTextBox";
            priceTextBox.Size = new Size(100, 23);
            priceTextBox.TabIndex = 5;
            // 
            // stockTextBox
            // 
            stockTextBox.Location = new Point(224, 101);
            stockTextBox.Name = "stockTextBox";
            stockTextBox.Size = new Size(100, 23);
            stockTextBox.TabIndex = 6;
            // 
            // courseFilterComboBox
            // 
            courseFilterComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            courseFilterComboBox.FormattingEnabled = true;
            courseFilterComboBox.Items.AddRange(new object[] { "All", "Starter", "Main", "Dessert", "Drink" });
            courseFilterComboBox.Location = new Point(532, 71);
            courseFilterComboBox.Name = "courseFilterComboBox";
            courseFilterComboBox.Size = new Size(121, 23);
            courseFilterComboBox.TabIndex = 10;
            courseFilterComboBox.SelectedIndexChanged += CourseFilterComboBox_SelectedIndexChanged;
            // 
            // vatTextBox
            // 
            vatTextBox.Location = new Point(15, 162);
            vatTextBox.Name = "vatTextBox";
            vatTextBox.Size = new Size(100, 23);
            vatTextBox.TabIndex = 11;
            // 
            // descriptionTextBox
            // 
            descriptionTextBox.Location = new Point(12, 240);
            descriptionTextBox.Multiline = true;
            descriptionTextBox.Name = "descriptionTextBox";
            descriptionTextBox.Size = new Size(312, 162);
            descriptionTextBox.TabIndex = 12;
            // 
            // deleteButton
            // 
            deleteButton.Font = new Font("Segoe UI", 9.75F);
            deleteButton.Location = new Point(807, 477);
            deleteButton.Name = "deleteButton";
            deleteButton.Size = new Size(75, 29);
            deleteButton.TabIndex = 13;
            deleteButton.Text = "Delete";
            deleteButton.UseVisualStyleBackColor = true;
            deleteButton.Click += deleteButton_Click;
            // 
            // courseTypeComboBox
            // 
            courseTypeComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            courseTypeComboBox.FormattingEnabled = true;
            courseTypeComboBox.Items.AddRange(new object[] { CourseType.Starter, CourseType.Main, CourseType.Dessert, CourseType.Drink });
            courseTypeComboBox.Location = new Point(151, 162);
            courseTypeComboBox.Name = "courseTypeComboBox";
            courseTypeComboBox.Size = new Size(121, 23);
            courseTypeComboBox.TabIndex = 14;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label1.Location = new Point(431, 74);
            label1.Name = "label1";
            label1.Size = new Size(101, 17);
            label1.TabIndex = 15;
            label1.Text = "Filter By Course:";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold);
            label2.Location = new Point(12, 83);
            label2.Name = "label2";
            label2.Size = new Size(44, 17);
            label2.TabIndex = 16;
            label2.Text = "Name";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold);
            label3.Location = new Point(118, 83);
            label3.Name = "label3";
            label3.Size = new Size(38, 17);
            label3.TabIndex = 17;
            label3.Text = "Price";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold);
            label4.Location = new Point(224, 83);
            label4.Name = "label4";
            label4.Size = new Size(41, 17);
            label4.TabIndex = 18;
            label4.Text = "Stock";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold);
            label5.Location = new Point(15, 144);
            label5.Name = "label5";
            label5.Size = new Size(32, 17);
            label5.TabIndex = 19;
            label5.Text = "VAT";
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold);
            label6.Location = new Point(151, 144);
            label6.Name = "label6";
            label6.Size = new Size(83, 17);
            label6.TabIndex = 20;
            label6.Text = "Course Type";
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold);
            label7.Location = new Point(15, 222);
            label7.Name = "label7";
            label7.Size = new Size(79, 17);
            label7.TabIndex = 21;
            label7.Text = "Description";
            // 
            // label8
            // 
            label8.AutoSize = true;
            label8.Font = new Font("Segoe UI", 20.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label8.Location = new Point(24, 18);
            label8.Name = "label8";
            label8.Size = new Size(268, 37);
            label8.TabIndex = 22;
            label8.Text = "Menu Management";
            // 
            // MenuManagementForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1099, 567);
            Controls.Add(label8);
            Controls.Add(label7);
            Controls.Add(label6);
            Controls.Add(label5);
            Controls.Add(label4);
            Controls.Add(label3);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(courseTypeComboBox);
            Controls.Add(deleteButton);
            Controls.Add(descriptionTextBox);
            Controls.Add(vatTextBox);
            Controls.Add(courseFilterComboBox);
            Controls.Add(stockTextBox);
            Controls.Add(priceTextBox);
            Controls.Add(nameTextBox);
            Controls.Add(button3);
            Controls.Add(button2);
            Controls.Add(button1);
            Controls.Add(menuDataGridView);
            Name = "MenuManagementForm";
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
        private ComboBox courseFilterComboBox;
        private TextBox vatTextBox;
        private TextBox descriptionTextBox;
        private Button deleteButton;
        private ComboBox courseTypeComboBox;
        private Label label1;
        private Label label2;
        private Label label3;
        private Label label4;
        private Label label5;
        private Label label6;
        private Label label7;
        private Label label8;
    }
}