using System;

namespace ChapeauModel
{
    public class Order
    {
        public int OrderId { get; set; }
        public Table TableId { get; set; }
        public Employee EmployeeId { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsDone { get; set; }
    }
} 