using System;

namespace ChapeauModel
{
    public class Order
    {
        public int OrderId { get; set; }
        public int TableId { get; set; } // Change to Object
        public int EmployeeId { get; set; } // Change to Object
        public DateTime CreatedAt { get; set; }
        public bool IsDone { get; set; }
    }
}