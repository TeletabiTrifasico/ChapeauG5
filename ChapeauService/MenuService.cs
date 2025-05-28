using System.Collections.Generic;
using ChapeauDAL;
using ChapeauModel;

namespace ChapeauService
{
    public class MenuService
    {
        private MenuDao menuDao;
        
        public MenuService()
        {
            menuDao = new MenuDao();
        }
        
        public List<MenuCategory> GetAllCategories()
        {
            return menuDao.GetAllCategories();
        }
        
        public List<MenuItem> GetMenuItemsByCategory(int categoryId)
        {
            return menuDao.GetMenuItemsByCategory(categoryId);
        }
    }
}