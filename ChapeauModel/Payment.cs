using System;

namespace ChapeauModel
{
    public enum PaymentMethod
    {
        Cash,
        DebitCard,
        CreditCard
    }
    
    public enum FeedbackType
    {
        None = 0,
        Comment = 1,
        Complaint = 2,
        Recommendation = 3
    }

    public class Payment
    {
        public int PaymentId { get; set; }
        public Invoice Invoice { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public decimal Amount { get; set; } // Final amount paid (including tip)
        public string Feedback { get; set; }
        public FeedbackType FeedbackType { get; set; }
        public DateTime CreatedAt { get; set; }
        
        public Payment()
        {
            CreatedAt = DateTime.Now;
            FeedbackType = FeedbackType.None;
        }
    }
}