namespace ChapeauModel
{
    public class OrderItem
    {
        public int OrderItemId { get; set; }
        public Order OrderId { get; set; }
        public MenuItem MenuItemId { get; set; }
        public int Quantity { get; set; }
        public string Comment { get; set; }
        public DateTime CreatedAt { get; set; }
        public enum Status
        {
            Ordered,
            BeingPrepared,
            ReadyToBeServed,
            Served,
        }

    }
}