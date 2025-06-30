using System;
using System.Collections.Generic;
using System.Linq;

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
        
        public List<Payment> Payments { get; set; }
        
        public Invoice()
        {
            CreatedAt = DateTime.Now;
            Payments = new List<Payment>();
        }
        
        public decimal GetTotalPaidAmount()
        {
            return Payments?.Sum(p => p.Amount) ?? 0;
        }
        
        public bool IsFullyPaid()
        {
            return Math.Abs(GetTotalPaidAmount() - (TotalAmount + TotalTipAmount)) < 0.01m;
        }
        
        public void AddPayment(Payment payment)
        {
            if (Payments == null)
                Payments = new List<Payment>();
            
            payment.Invoice = this;
            Payments.Add(payment);
        }
    }
}