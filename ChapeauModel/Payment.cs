using System;

namespace ChapeauModel
{
    public enum PaymentMethod
    {
        Cash = 0,
        DebitCard = 1,
        CreditCardVisa = 2,
        CreditCardAmex = 3
    }

    public class Payment
    {
        public int PaymentId { get; set; }
        public int InvoiceId { get; set; }
        public string Feedback { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public decimal TotalPrice { get; set; }
        public decimal VatPercentage { get; set; }
        public decimal TipAmount { get; set; }
        public decimal FinalAmount { get; set; }
        public int EmployeeId { get; set; }
        
        public Payment()
        {
            Feedback = string.Empty;
        }
    }
}