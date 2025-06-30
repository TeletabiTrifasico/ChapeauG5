namespace ChapeauG5
{
    partial class MenuView
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
            categoryList = new ComboBox();
            category = new Label();
            menuList = new ListView();
            SuspendLayout();
            // 
            // categoryList
            // 
            categoryList.DisplayMember = "Name";
            categoryList.DropDownStyle = ComboBoxStyle.DropDownList;
            categoryList.FormattingEnabled = true;
            categoryList.Location = new Point(200, 59);
            categoryList.Name = "categoryList";
            categoryList.Size = new Size(242, 40);
            categoryList.TabIndex = 1;
            categoryList.ValueMember = "CategoryId";
            categoryList.SelectedIndexChanged += categoryList_SelectedIndexChanged;
            // 
            // category
            // 
            category.AutoSize = true;
            category.Location = new Point(60, 59);
            category.Name = "category";
            category.Size = new Size(115, 32);
            category.TabIndex = 3;
            category.Text = "Category:";
            // 
            // menuList
            // 
            menuList.FullRowSelect = true;
            menuList.Location = new Point(51, 136);
            menuList.MultiSelect = false;
            menuList.Name = "menuList";
            menuList.Size = new Size(1100, 542);
            menuList.TabIndex = 4;
            menuList.UseCompatibleStateImageBehavior = false;
            menuList.View = View.Details;
            menuList.FullRowSelect = true;
            menuList.MultiSelect = false;
            menuList.Columns.Add("Item");
            menuList.Columns.Add("Price");
            menuList.Columns.Add("Stock");
            menuList.Columns.Add("Description");
            // 
            // MenuView
            // 
            AutoScaleDimensions = new SizeF(13F, 32F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1211, 742);
            Controls.Add(category);
            Controls.Add(categoryList);
            Controls.Add(menuList);
            Name = "MenuView";
            Text = "MenuView";
            Load += MenuView_Load;
            ResumeLayout(false);
            PerformLayout();
        }
        private ComboBox categoryList;
        private Label category;
        private ListView menuList;

        #endregion
    }
}