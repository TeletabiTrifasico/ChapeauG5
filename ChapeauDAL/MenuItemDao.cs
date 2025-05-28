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
        public List<MenuItem> GetAllMenuItems()
        {
            List<MenuItem> items = new List<MenuItem>();
            string query = "SELECT * FROM Menu_Item";
            using (SqlConnection conn = GetConnection())
            {
                conn.Open(); //  ADD THIS
                using (SqlCommand cmd = new SqlCommand(query, conn))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        items.Add(new MenuItem
                        {
                            MenuItemID = (int)reader["menu_item_id"],
                            Name = reader["name"].ToString(),
                            Price = (decimal)reader["price"],
                            Stock = (int)reader["stock"],
                            CategoryID = (int)reader["category_id"], // Assuming CategoryID is used for
                           
                            Category = reader["course_type"].ToString(),
                            IsActive = (bool)reader["is_active"]
                        });
                    }
                }
            }
            return items;
        }

        public void AddMenuItem(MenuItem item)
        {
            string query = "INSERT INTO Menu_Item (name, price, stock, category_id, course_type, is_active) " +
                           "VALUES (@name, @price, @stock, @category_id, @course_type, 1)";
            using (SqlConnection conn = GetConnection())
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    int categoryId = GetCategoryIdByCourseType(item.Category);  // category from combobox

                    cmd.Parameters.AddWithValue("@name", item.Name);
                    cmd.Parameters.AddWithValue("@price", item.Price);
                    cmd.Parameters.AddWithValue("@stock", item.Stock);
                    cmd.Parameters.AddWithValue("@category_id", categoryId);     // correct ID
                    cmd.Parameters.AddWithValue("@course_type", item.Category);  // readable label for UI

                    cmd.ExecuteNonQuery();
                }
            }
        }


        // TEST TEST TEST

       

        private int GetCategoryIdByCourseType(string courseType)
        {
            // This mapping is based on your screenshot
            return courseType switch
            {
                "Starters" => 1,
                "Mains" => 2,
                "Desserts" => 3,
                "Entremets" => 5,
                _ => throw new Exception($"No category_id mapped for course type '{courseType}'")
            };
        }





        public void UpdateMenuItem(MenuItem item)
        {
            string query = "UPDATE Menu_Item SET name = @name, price = @price, stock = @stock, category_id = @category_id, course_type = @course_type WHERE menu_item_id = @menu_item_id";
            using (SqlConnection conn = GetConnection())
            {
                conn.Open(); // ADD THIS
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@name", item.Name);
                    cmd.Parameters.AddWithValue("@price", item.Price);
                    cmd.Parameters.AddWithValue("@stock", item.Stock);
                   

                    cmd.Parameters.AddWithValue("@category_id", item.CategoryID);

                    cmd.Parameters.AddWithValue("@course_type", item.Category);
                    cmd.Parameters.AddWithValue("@menu_item_id", item.MenuItemID);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void SetMenuItemActiveStatus(int menuItemID, bool isActive)
        {
            string query = "UPDATE Menu_Item SET is_active = @is_active WHERE menu_item_id = @menu_item_id";
            using (SqlConnection conn = GetConnection())
            {
                conn.Open(); //  ADD THIS
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@is_active", isActive);
                    cmd.Parameters.AddWithValue("@menu_item_id", menuItemID);
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
