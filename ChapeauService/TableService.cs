using System.Collections.Generic;
using ChapeauDAL;
using ChapeauModel;

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
    }
}