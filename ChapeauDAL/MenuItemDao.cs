using ChapeauModel;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChapeauDAL
{
   public class MenuItemDao : BaseDao
{
    public async Task<List<MenuItem>> GetAllMenuItemsAsync()
    {
        string query = "SELECT * FROM Menu_Item";

        return (await ExecuteQueryAsync(query, reader => new MenuItem
        {
            MenuItemId = (int)reader["menu_item_id"],
            Name = reader["name"].ToString(),
            Description = reader["description"].ToString(),
            Price = (decimal)reader["price"],
            Stock = (int)reader["stock"],
            CategoryId = new Menu
            {
                CategoryId = (int)reader["category_id"]
                // Will be matched to name and menu_card later in LoadMenuItems()
            },
            VatPercentage = (int)reader["vat_percentage"],
            IsActive = (bool)reader["is_active"],
            CourseType = ParseCourseTypeFromDb(reader["course_type"].ToString()),
            IsAlcoholic = false // Optional: update if needed
        })).ToList();
    }

    public void UpdateMenuItem(MenuItem menuItem)
    {
        string query = @"
            UPDATE Menu_Item SET 
                name = @name,
                description = @description,
                price = @price,
                stock = @stock,
                category_id = @category_id,
                vat_percentage = @vat_percentage,
                is_active = @is_active,
                course_type = @course_type
            WHERE menu_item_id = @menu_item_id";

        SqlParameter[] parameters = new SqlParameter[]
        {
            new SqlParameter("@name", menuItem.Name),
            new SqlParameter("@description", menuItem.Description),
            new SqlParameter("@price", menuItem.Price),
            new SqlParameter("@stock", menuItem.Stock),

            
            new SqlParameter("@category_id", menuItem.CategoryId.CategoryId),

            new SqlParameter("@vat_percentage", menuItem.VatPercentage),
            new SqlParameter("@is_active", menuItem.IsActive),
            new SqlParameter("@course_type", MapCourseTypeToDb(menuItem.CourseType)),
            new SqlParameter("@menu_item_id", menuItem.MenuItemId)
        };

        ExecuteEditQuery(query, parameters);
    }

    public void AddMenuItem(MenuItem menuItem)
    {
        string query = @"
            INSERT INTO Menu_Item 
                (name, description, price, stock, category_id, vat_percentage, is_active, course_type)
            VALUES 
                (@name, @description, @price, @stock, @category_id, @vat_percentage, @is_active, @course_type)";

        SqlParameter[] parameters = new SqlParameter[]
        {
            new SqlParameter("@name", menuItem.Name),
            new SqlParameter("@description", menuItem.Description),
            new SqlParameter("@price", menuItem.Price),
            new SqlParameter("@stock", menuItem.Stock),
            new SqlParameter("@category_id", menuItem.CategoryId.CategoryId),
            new SqlParameter("@vat_percentage", menuItem.VatPercentage),
            new SqlParameter("@is_active", menuItem.IsActive),
            new SqlParameter("@course_type", MapCourseTypeToDb(menuItem.CourseType))
        };

        ExecuteEditQuery(query, parameters);
    }

    // Maps enum to string for DB
    private string MapCourseTypeToDb(CourseType type)
    {
        return type switch
        {
            CourseType.Starter => "Starters",
            CourseType.Main => "Mains",
            CourseType.Dessert => "Desserts",
            CourseType.Drink => "Drinks",
            _ => "Mains"
        };
    }

    // Maps string from DB to enum
    private CourseType ParseCourseTypeFromDb(string dbValue)
    {
        return dbValue switch
        {
            "Starters" => CourseType.Starter,
            "Mains" => CourseType.Main,
            "Desserts" => CourseType.Dessert,
            "Drinks" => CourseType.Drink,
            _ => CourseType.Main
        };
    }


        public void SetMenuItemActiveStatus(int menuItemId, bool isActive)
        {
            string query = "UPDATE Menu_Item SET is_active = @isActive WHERE menu_item_id = @menuItemId";

            SqlParameter[] parameters = new SqlParameter[]
            {
        new SqlParameter("@isActive", isActive),
        new SqlParameter("@menuItemId", menuItemId)
            };

            ExecuteEditQuery(query, parameters);
        }
    }
}

