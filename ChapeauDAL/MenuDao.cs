using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using ChapeauModel;

namespace ChapeauDAL
{
    public class MenuDao : BaseDao
    {
        public List<MenuCategory> GetAllCategories()
        {
            string query = "SELECT * FROM Menu ORDER BY name";
            DataTable dataTable = ExecuteSelectQuery(query, null);
            
            List<MenuCategory> categories = new List<MenuCategory>();
            
            foreach (DataRow dr in dataTable.Rows)
            {
                MenuCategory category = new MenuCategory
                {
                    CategoryId = (int)dr["category_id"],
                    Name = (string)dr["name"],
                    MenuCard = (string)dr["menu_card"]
                };
                
                categories.Add(category);
            }
            
            return categories;
        }
        
        public List<MenuItem> GetMenuItemsByCategory(int categoryId)
        {
            string query = @"
                SELECT mi.*, ISNULL(di.is_alcoholic, 0) as is_alcoholic 
                FROM Menu_Item mi
                LEFT JOIN Drink_Item di ON mi.menu_item_id = di.menu_item_id
                WHERE mi.category_id = @CategoryId AND mi.is_active = 1
                ORDER BY mi.name";
            
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@CategoryId", categoryId)
            };
            
            DataTable dataTable = ExecuteSelectQuery(query, parameters);
            
            List<MenuItem> menuItems = new List<MenuItem>();
            
            foreach (DataRow dr in dataTable.Rows)
            {
                MenuItem item = new MenuItem
                {
                    MenuItemId = (int)dr["menu_item_id"],
                    Name = (string)dr["name"],
                    Description = dr["description"] != DBNull.Value ? (string)dr["description"] : string.Empty,
                    Price = (decimal)dr["price"],
                    Stock = (int)dr["stock"],
                    CategoryId = (int)dr["category_id"],
                    VatPercentage = (int)dr["vat_percentage"],
                    IsActive = (bool)dr["is_active"],
                    CourseType = dr["course_type"] != DBNull.Value ? 
                                ParseCourseType((string)dr["course_type"]) : CourseType.Main,
                    IsAlcoholic = (bool)dr["is_alcoholic"]
                };
                
                menuItems.Add(item);
            }
            
            return menuItems;
        }
        
        private CourseType ParseCourseType(string courseType)
        {
            if (string.IsNullOrEmpty(courseType) || 
                !Enum.TryParse<CourseType>(courseType, true, out CourseType result))
            {
                return CourseType.Main; // Default
            }
            
            return result;
        }

        private string GetStringOrEmpty(DataRow dr, string columnName)
        {
            return dr[columnName] != DBNull.Value ? (string)dr[columnName] : string.Empty;
        }
    }
}