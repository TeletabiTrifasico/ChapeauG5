namespace ChapeauModel
{
    public class TableOrderStatus
    {
        public int TableId { get; set; }
        public int TableNumber { get; set; }
        public bool HasRunningOrder { get; set; }
        public string FoodOrderStatus { get; set; } // e.g. "Ordered", "BeingPrepared", "ReadyToBeServed", or null
        public string DrinkOrderStatus { get; set; } // e.g. "Ordered", "BeingPrepared", "ReadyToBeServed", or null
    }
}