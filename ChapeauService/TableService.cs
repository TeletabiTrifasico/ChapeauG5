using System.Collections.Generic;
using ChapeauDAL;
using ChapeauModel;
using System.Linq;

namespace ChapeauService
{
    public class TableService
    {
        private TableDao tableDao;
        
        public TableService()
        {
            tableDao = new TableDao();
        }
        
        public List<Table> GetAllTables()
        {
            return tableDao.GetAllTables();
        }
        
        public void UpdateTableStatus(int tableId, TableStatus status)
        {
            tableDao.UpdateTableStatus(tableId, status);
        }

        public Table GetTableById(int tableId)
        {
            // Get all tables and find the one with matching ID
            List<Table> tables = tableDao.GetAllTables();
            return tables.FirstOrDefault(t => t.TableId == tableId);
        }
    }
}