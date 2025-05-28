using ChapeauModel;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChapeauDAL
{
    public class OrderDao : BaseDao
    {
        public List<BZOrder> GetOrdersByDateRange(DateTime start, DateTime end)
        {
            List<BZOrder> orders = new List<BZOrder>();
            string query = "SELECT * FROM BZOrders WHERE OrderDate BETWEEN @Start AND @End";
            using (SqlConnection conn = GetConnection())
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@Start", start);
                cmd.Parameters.AddWithValue("@End", end);
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        orders.Add(new BZOrder
                        {
                            OrderID = (int)reader["OrderID"],
                            OrderDate = (DateTime)reader["OrderDate"],
                            TotalAmount = (decimal)reader["TotalAmount"],
                            TipAmount = (decimal)reader["TipAmount"],
                            CardType = reader["CardType"].ToString()
                        });
                    }
                }
            }
            return orders;
        }

        public decimal GetTotalSales(string cardType, DateTime start, DateTime end)
        {
            string query = "SELECT SUM(TotalAmount) FROM BZOrders WHERE CardType = @CardType AND OrderDate BETWEEN @Start AND @End";
            using (SqlConnection conn = GetConnection())
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@CardType", cardType);
                cmd.Parameters.AddWithValue("@Start", start);
                cmd.Parameters.AddWithValue("@End", end);
                return (decimal)(cmd.ExecuteScalar() ?? 0);
            }
        }

        public decimal GetTotalTips(DateTime start, DateTime end)
        {
            string query = "SELECT SUM(TipAmount) FROM BZOrders WHERE OrderDate BETWEEN @Start AND @End";
            using (SqlConnection conn = GetConnection())
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@Start", start);
                cmd.Parameters.AddWithValue("@End", end);
                return (decimal)(cmd.ExecuteScalar() ?? 0);
            }
        }
    }
}
