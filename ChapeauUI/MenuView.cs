using ChapeauModel;
using ChapeauService;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

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
            // Loading the menu categories when the form loads
            LoadMenuCategories();
        }

        private void LoadMenuCategories()
        {
            categoryList.Items.Clear();

            List<MenuCategory> categories = menuService.GetAllCategories();

            foreach (MenuCategory category in categories)
            {
                categoryList.Items.Add(category);
            }

            if (categoryList.Items.Count > 0)
                categoryList.SelectedIndex = 0;
        }
        private void categoryList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (categoryList.SelectedItem is MenuCategory selectedCategory)
            {
                // Load menu items for the selected category
                List<MenuItem> menuItems = menuService.GetMenuItemsByCategory(selectedCategory.CategoryId);

                menuList.Items.Clear();

                foreach (MenuItem item in menuItems)
                {
                    ListViewItem lvi = new ListViewItem(item.Name);
                    lvi.SubItems.Add($"€{item.Price:0.00}");
                    lvi.SubItems.Add(GetStockStatus(item.Stock));
                    lvi.SubItems.Add(item.Description);
                    lvi.Tag = item;

                    menuList.Items.Add(lvi);
                }
                menuList.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
            }
        }

        private string GetStockStatus(int stock)
        {
            if (stock > 10)
                return "In Stock";
            else if (stock > 0 && stock <= 10)
                return "Low Stock";
            else
                return "Out of Stock";
        }
    }
}
