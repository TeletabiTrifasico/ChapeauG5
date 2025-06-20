using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using ChapeauModel;

namespace ChapeauDAL
{
    public class TableDao : BaseDao
    {
        public List<Table> GetAllTables()
        {
            string query = "SELECT * FROM [Table] ORDER BY table_number";
            DataTable dataTable = ExecuteSelectQuery(query, null);
            
            List<Table> tables = new List<Table>();
            
            foreach (DataRow dr in dataTable.Rows)
            {
                Table table = new Table
                {
                    TableId = (int)dr["table_id"],
                    TableNumber = (int)dr["table_number"],
                    Capacity = (int)dr["capacity"],
                    Status = ParseTableStatus((string)dr["status"])
                };
                
                tables.Add(table);
            }
            
            return tables;
        }
        
        public void UpdateTableStatus(int tableId, TableStatus status)
        {
            try
            {
                // Convert enum to the exact string value your database expects
                string newStatus;
                switch (status)
                {
                    case TableStatus.Free:
                        // Change this to match what your database expects
                        newStatus = "Free"; // Try "Free" instead of "Available"
                        break;
                    case TableStatus.Occupied:
                        newStatus = "Occupied"; 
                        break;
                    default:
                        newStatus = "Free"; // Default to Free
                        break;
                }
                
                // Update with known value
                string updateQuery = "UPDATE [Table] SET status = @Status WHERE table_id = @TableId";
                
                SqlParameter[] parameters = new SqlParameter[]
                {
                    new SqlParameter("@Status", newStatus),
                    new SqlParameter("@TableId", tableId)
                };
                
                ExecuteEditQuery(updateQuery, parameters);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error updating table {tableId} status: {ex.Message} - Check constraint violation.", ex);
            }
        }
        
        // Add this method to get current table status
        public string GetCurrentTableStatus(int tableId)
        {
            string query = "SELECT status FROM [Table] WHERE table_id = @TableId";
            
            SqlParameter[] parameters = { new SqlParameter("@TableId", tableId) };
            
            DataTable dataTable = ExecuteSelectQuery(query, parameters);
            
            if (dataTable.Rows.Count > 0)
            {
                return dataTable.Rows[0]["status"].ToString();
            }
            
            return "Unknown";
        }
        
        private TableStatus ParseTableStatus(string status)
        {
            if (Enum.TryParse<TableStatus>(status, true, out TableStatus result))
                return result;
            
            return TableStatus.Free; // Default
        }
    }
}