using ChapeauModel;
using ChapeauService;

namespace ChapeauG5
{
    public partial class MenuView : Form
    {
        private MenuService menuService;
        private Employee loggedInEmployee;

        public MenuView(Employee employee)
        {
            InitializeComponent();
            menuService = new MenuService();
            loggedInEmployee = employee;
        }

        private void MenuView_Load(object sender, EventArgs e)
        {
            lblWelcome.Text = $"Welcome, {loggedInEmployee.FirstName}!";
            LoadMenuItems();
        }

        private void LoadMenuItems()
        {
            List<MenuItem> menuItems = menuService.GetAllMenuItems();
            flpMenuItems.Controls.Clear();

            foreach (MenuItem item in menuItems)
            {
                Button btnItem = new Button
                {
                    Text = $"{item.Name}\n{item.Price:C}",
                    Size = new Size(150, 100),
                    Tag = item,
                    Font = new Font("Segoe UI", 10, FontStyle.Bold),
                    FlatStyle = FlatStyle.Flat,
                    BackColor = Color.LightBlue
                };
                btnItem.Click += BtnItem_Click;
                flpMenuItems.Controls.Add(btnItem);
            }
        }

        private void BtnItem_Click(object sender, EventArgs e)
        {
            Button btn = sender as Button;
            if (btn != null && btn.Tag is MenuItem item)
            {
                MessageBox.Show($"You selected: {item.Name}\nPrice: {item.Price:C}", "Menu Item Selected");
            }
        }
        private ComboBox cardList;
        private ComboBox categoryList;
        private Label label1;
        private Label label2;
        private ListView menuList;

        private void InitializeComponent()
        {
            cardList = new ComboBox();
            categoryList = new ComboBox();
            label1 = new Label();
            label2 = new Label();
            menuList = new ListView();
            SuspendLayout();
            // 
            // cardList
            // 
            cardList.FormattingEnabled = true;
            cardList.Location = new Point(125, 56);
            cardList.Name = "cardList";
            cardList.Size = new Size(242, 40);
            cardList.TabIndex = 0;
            // 
            // categoryList
            // 
            categoryList.FormattingEnabled = true;
            categoryList.Location = new Point(609, 56);
            categoryList.Name = "categoryList";
            categoryList.Size = new Size(242, 40);
            categoryList.TabIndex = 1;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(51, 59);
            label1.Name = "label1";
            label1.Size = new Size(68, 32);
            label1.TabIndex = 2;
            label1.Text = "Card:";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(488, 59);
            label2.Name = "label2";
            label2.Size = new Size(115, 32);
            label2.TabIndex = 3;
            label2.Text = "Category:";
            // 
            // menuList
            // 
            menuList.Location = new Point(51, 136);
            menuList.Name = "menuList";
            menuList.Size = new Size(1100, 542);
            menuList.TabIndex = 4;
            menuList.UseCompatibleStateImageBehavior = false;
            // 
            // MenuView
            // 
            ClientSize = new Size(1211, 742);
            Controls.Add(menuList);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(categoryList);
            Controls.Add(cardList);
            Name = "MenuView";
            ResumeLayout(false);
            PerformLayout();

        }
    }
}