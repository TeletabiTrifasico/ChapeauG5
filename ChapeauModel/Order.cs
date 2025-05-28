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
    
    public class OrderItem //Separate file for order items
    {
        public int OrderItemId { get; set; }
        public int OrderId { get; set; }
        public int MenuItemId { get; set; } // Change to Object
        public int Quantity { get; set; }
        public string Comment { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Status { get; set; } // e.g., Ordered, BeingPrepared, ReadyToBeServed, Served.
        
        // For display purposes
        public string ItemName { get; set; }
        public decimal ItemPrice { get; set; }
    }
}