using System;

namespace ChapeauModel
{
    public class InvoiceItem
    {
        public int OrderItemId { get; set; }
        public int MenuItemId { get; set; }
        public string ItemName { get; set; }
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public decimal VatPercentage { get; set; }
        public decimal Subtotal { get; set; }
        public decimal VatAmount { get; set; }
        public bool IsAlcoholic { get; set; }
    }
}