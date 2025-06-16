using System;
using System.Collections.Generic;

namespace ChapeauModel
{
    public class Invoice
    {
        public int InvoiceId { get; set; }
        public Order OrderId { get; set; }
        public decimal TotalAmount { get; set; } // Total including VAT
        public decimal TotalVat { get; set; } // Total VAT amount
        public decimal TotalTipAmount { get; set; } // Optional tip
        public DateTime CreatedAt { get; set; }
        public decimal LowVatAmount { get; set; }
        public decimal HighVatAmount { get; set; }
        public decimal TotalExcludingVat { get; set; } // Total excluding VAT
        
        public Invoice()
        {
            CreatedAt = DateTime.Now;
        }
    }
}