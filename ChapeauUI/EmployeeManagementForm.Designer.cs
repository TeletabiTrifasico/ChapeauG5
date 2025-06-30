namespace ChapeauUI
{
    partial class EmployeeManagementForm
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
            employeeDataGridView = new DataGridView();
            nameTextBox = new TextBox();
            usernameTextBox = new TextBox();
            passwordTextBox = new TextBox();
            button1 = new Button();
            button2 = new Button();
            button3 = new Button();
            roleComboBox = new ComboBox();
            lastNameTextBox = new TextBox();
            emailTextBox = new TextBox();
            label1 = new Label();
            label2 = new Label();
            label3 = new Label();
            label4 = new Label();
            label5 = new Label();
            label6 = new Label();
            label7 = new Label();
            deleteButton = new Button();
            ((System.ComponentModel.ISupportInitialize)employeeDataGridView).BeginInit();
            SuspendLayout();
            // 
            // employeeDataGridView
            // 
            employeeDataGridView.AllowUserToAddRows = false;
            employeeDataGridView.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            employeeDataGridView.Location = new Point(425, 100);
            employeeDataGridView.Name = "employeeDataGridView";
            employeeDataGridView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            employeeDataGridView.Size = new Size(662, 371);
            employeeDataGridView.TabIndex = 0;
            // 
            // nameTextBox
            // 
            nameTextBox.Font = new Font("Segoe UI", 9.75F);
            nameTextBox.Location = new Point(12, 100);
            nameTextBox.Name = "nameTextBox";
            nameTextBox.Size = new Size(162, 25);
            nameTextBox.TabIndex = 1;
            // 
            // usernameTextBox
            // 
            usernameTextBox.Font = new Font("Segoe UI", 9.75F);
            usernameTextBox.Location = new Point(12, 200);
            usernameTextBox.Name = "usernameTextBox";
            usernameTextBox.Size = new Size(162, 25);
            usernameTextBox.TabIndex = 2;
            // 
            // passwordTextBox
            // 
            passwordTextBox.Font = new Font("Segoe UI", 9.75F);
            passwordTextBox.Location = new Point(12, 246);
            passwordTextBox.Name = "passwordTextBox";
            passwordTextBox.Size = new Size(162, 25);
            passwordTextBox.TabIndex = 3;
            // 
            // button1
            // 
            button1.Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            button1.Location = new Point(12, 363);
            button1.Name = "button1";
            button1.Size = new Size(348, 30);
            button1.TabIndex = 4;
            button1.Text = "Add";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // button2
            // 
            button2.Font = new Font("Segoe UI", 9.75F);
            button2.Location = new Point(726, 477);
            button2.Name = "button2";
            button2.Size = new Size(75, 30);
            button2.TabIndex = 5;
            button2.Text = "Update";
            button2.UseVisualStyleBackColor = true;
            button2.Click += button2_Click;
            // 
            // button3
            // 
            button3.Font = new Font("Segoe UI", 9.75F);
            button3.Location = new Point(807, 477);
            button3.Name = "button3";
            button3.Size = new Size(75, 30);
            button3.TabIndex = 6;
            button3.Text = "Activate";
            button3.UseVisualStyleBackColor = true;
            button3.Click += button3_Click;
            // 
            // roleComboBox
            // 
            roleComboBox.Font = new Font("Segoe UI", 9.75F);
            roleComboBox.FormattingEnabled = true;
            roleComboBox.Items.AddRange(new object[] { "Manager", "Waiter", "Kitchen", "Bar" });
            roleComboBox.Location = new Point(12, 296);
            roleComboBox.Name = "roleComboBox";
            roleComboBox.Size = new Size(121, 25);
            roleComboBox.TabIndex = 7;
            // 
            // lastNameTextBox
            // 
            lastNameTextBox.Font = new Font("Segoe UI", 9.75F);
            lastNameTextBox.Location = new Point(180, 100);
            lastNameTextBox.Name = "lastNameTextBox";
            lastNameTextBox.Size = new Size(180, 25);
            lastNameTextBox.TabIndex = 8;
            // 
            // emailTextBox
            // 
            emailTextBox.Font = new Font("Segoe UI", 9.75F);
            emailTextBox.Location = new Point(12, 148);
            emailTextBox.Name = "emailTextBox";
            emailTextBox.Size = new Size(348, 25);
            emailTextBox.TabIndex = 9;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 20.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label1.Location = new Point(24, 18);
            label1.Name = "label1";
            label1.Size = new Size(321, 37);
            label1.TabIndex = 10;
            label1.Text = "Employee Management";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold);
            label2.Location = new Point(12, 82);
            label2.Name = "label2";
            label2.Size = new Size(75, 17);
            label2.TabIndex = 11;
            label2.Text = "First Name";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold);
            label3.Location = new Point(180, 82);
            label3.Name = "label3";
            label3.Size = new Size(73, 17);
            label3.TabIndex = 12;
            label3.Text = "Last Name";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold);
            label4.Location = new Point(12, 130);
            label4.Name = "label4";
            label4.Size = new Size(42, 17);
            label4.TabIndex = 13;
            label4.Text = "Email";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold);
            label5.Location = new Point(12, 182);
            label5.Name = "label5";
            label5.Size = new Size(69, 17);
            label5.TabIndex = 14;
            label5.Text = "Username";
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold);
            label6.Location = new Point(12, 228);
            label6.Name = "label6";
            label6.Size = new Size(66, 17);
            label6.TabIndex = 15;
            label6.Text = "Password";
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold);
            label7.Location = new Point(12, 274);
            label7.Name = "label7";
            label7.Size = new Size(35, 17);
            label7.TabIndex = 16;
            label7.Text = "Role";
            // 
            // deleteButton
            // 
            deleteButton.Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            deleteButton.Location = new Point(645, 477);
            deleteButton.Name = "deleteButton";
            deleteButton.Size = new Size(75, 30);
            deleteButton.TabIndex = 17;
            deleteButton.Text = "Delete";
            deleteButton.UseVisualStyleBackColor = true;
            deleteButton.Click += deleteButton_Click_1;
            // 
            // EmployeeManagementForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1099, 567);
            Controls.Add(deleteButton);
            Controls.Add(label7);
            Controls.Add(label6);
            Controls.Add(label5);
            Controls.Add(label4);
            Controls.Add(label3);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(emailTextBox);
            Controls.Add(lastNameTextBox);
            Controls.Add(roleComboBox);
            Controls.Add(button3);
            Controls.Add(button2);
            Controls.Add(button1);
            Controls.Add(passwordTextBox);
            Controls.Add(usernameTextBox);
            Controls.Add(nameTextBox);
            Controls.Add(employeeDataGridView);
            Name = "EmployeeManagementForm";
            Text = "EmployeeManagementForm";
            ((System.ComponentModel.ISupportInitialize)employeeDataGridView).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private DataGridView employeeDataGridView;
        private TextBox nameTextBox;
        private TextBox usernameTextBox;
        private TextBox passwordTextBox;
        private Button button1;
        private Button button2;
        private Button button3;
        private ComboBox roleComboBox;
        private TextBox lastNameTextBox;
        private TextBox emailTextBox;
        private Label label1;
        private Label label2;
        private Label label3;
        private Label label4;
        private Label label5;
        private Label label6;
        private Label label7;
        private Button deleteButton;
    }
}