using System;
using System.Collections.Generic;

namespace ChapeauModel
{
    public class Order
    {
        public int OrderId { get; set; }
        public Table TableId { get; set; }
        public Employee EmployeeId { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsDone { get; set; }
        public List<OrderItem> OrderItems { get; set; }
        
        public Order()
        {
            OrderItems = new List<OrderItem>();
        }
    }
}