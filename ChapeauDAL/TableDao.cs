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
                // Get the current status first for debugging
                string currentStatus = GetCurrentTableStatus(tableId);
                
                // Get an existing table with a status that works
                string query = "SELECT TOP 1 status FROM [Table] WHERE table_id <> @TableId";
                DataTable dt = ExecuteSelectQuery(query, new SqlParameter[] { new SqlParameter("@TableId", tableId) });
                
                string validStatus = "Available"; // Default fallback
                
                if (dt.Rows.Count > 0)
                {
                    // Use a status value that exists in the database
                    validStatus = dt.Rows[0]["status"].ToString();
                }
                
                // Determine which valid status to use
                string newStatus;
                switch (status)
                {
                    case TableStatus.Available:
                        newStatus = "Available"; // Exact match 
                        break;
                    case TableStatus.Occupied:
                        newStatus = "Occupied"; // Exact match 
                        break;
                    case TableStatus.Reserved:
                        newStatus = "Reserved"; // Exact match 
                        break;
                    default:
                        newStatus = validStatus; // Fallback to a known value
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
                
                Console.WriteLine($"Successfully updated table {tableId} from '{currentStatus}' to '{newStatus}'");
            }
            catch (Exception ex)
            {
                //detailed information to the exception
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
            
            return TableStatus.Available; // Default
        }
    }
}