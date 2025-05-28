using ChapeauDAL;
using ChapeauG5.ChapeauModel;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChapeauG5.ChapeauDAL
{
    public class CategoryDao : BaseDao
    {
        public List<Category> GetAllCategories()
        {
            List<Category> categories = new List<Category>();
            using (SqlConnection conn = GetConnection())
            {
                conn.Open();
                string query = "SELECT category_id, name FROM Menu"; // assuming 'Menu' holds categories
                using (SqlCommand cmd = new SqlCommand(query, conn))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        categories.Add(new Category
                        {
                            CategoryID = reader.GetInt32(0),
                            Name = reader.GetString(1)
                        });
                    }
                }
            }
            return categories;
        }
    }
}
