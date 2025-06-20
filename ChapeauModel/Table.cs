using System;

namespace ChapeauModel
{
    public enum TableStatus
    {
        Free,     
        Occupied,
    }
    
    public class Table
    {
        public int TableId { get; set; }
        public int TableNumber { get; set; }
        public int Capacity { get; set; }
        public TableStatus Status { get; set; }
        
        public bool IsAvailable => Status == TableStatus.Free;
    }
}