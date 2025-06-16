using System;
using System.Data;
using Microsoft.Data.SqlClient;
using ChapeauModel;

namespace ChapeauDAL
{
    public class PaymentDao : BaseDao
    {
        // Process a payment
        public int CreatePayment(Payment payment)
        {
            // Get the actual tip amount from the Payment object
            decimal tipAmount = 0;
            
            if (payment.InvoiceId != null && payment.InvoiceId.TotalTipAmount > 0)
            {
                tipAmount = payment.InvoiceId.TotalTipAmount;
            }
            
            // Get the total VAT amount
            decimal totalVatAmount = 0;
            if (payment.InvoiceId != null)
            {
                totalVatAmount = payment.InvoiceId.TotalVat;
            }
            
            // Map the enum to the exact string values expected by the database
            string paymentMethodString = MapPaymentMethodToDbValue(payment.PaymentMethod);
            
            // Get the EXCLUSIVE price (total before VAT and tip)
            decimal totalExclusivePrice = 0;
            if (payment.InvoiceId != null)
            {
                // Calculate total amount excluding VAT based on the total amount and VAT amount
                totalExclusivePrice = payment.InvoiceId.TotalAmount - totalVatAmount;
                
                Console.WriteLine($"DEBUG - Total Amount: {payment.InvoiceId.TotalAmount}");
                Console.WriteLine($"DEBUG - Total VAT: {totalVatAmount}");
                Console.WriteLine($"DEBUG - Calculated Ex VAT: {totalExclusivePrice}");
            }
            
            string query = @"
                INSERT INTO Payments 
                (invoice_id, payment_method, total_price, feedback, final_amount, tip_amount, vat_amount) 
                VALUES 
                (@InvoiceId, @PaymentMethod, @TotalPrice, @Feedback, @FinalAmount, @TipAmount, @VatAmount);
                SELECT SCOPE_IDENTITY();";

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@InvoiceId", payment.InvoiceId.InvoiceId),
                new SqlParameter("@PaymentMethod", paymentMethodString),
                new SqlParameter("@TotalPrice", totalExclusivePrice), // Price excluding VAT and tip
                new SqlParameter("@Feedback", string.IsNullOrEmpty(payment.Feedback) ? (object)DBNull.Value : payment.Feedback),
                new SqlParameter("@FinalAmount", payment.Amount), // Total final amount with tip
                new SqlParameter("@TipAmount", tipAmount),
                new SqlParameter("@VatAmount", totalVatAmount) // Store VAT amount
            };

            return ExecuteInsertQuery(query, parameters);
        }

        // Map enum values to the exact string values expected by the database
        private string MapPaymentMethodToDbValue(PaymentMethod method)
        {
            switch(method)
            {
                case PaymentMethod.Cash:
                    return "Cash";
                case PaymentMethod.DebitCard:
                    return "Debit Card";
                case PaymentMethod.CreditCard:
                    return "Credit Card";
                default:
                    return "Cash";
            }
        }

        // Get payment by invoice ID
        public Payment GetPaymentByInvoiceId(int invoiceId)
        {
            string query = @"
                SELECT p.*, i.*
                FROM Payments p
                JOIN Invoice i ON p.invoice_id = i.invoice_id
                WHERE p.invoice_id = @InvoiceId";

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@InvoiceId", invoiceId)
            };

            DataTable dataTable = ExecuteSelectQuery(query, parameters);
            
            if (dataTable.Rows.Count == 0)
                return null;

            return ReadPayment(dataTable.Rows[0]);
        }

        // Helper method to read a payment from a DataRow
        private Payment ReadPayment(DataRow dr)
        {
            Payment payment = new Payment
            {
                PaymentId = (int)dr["payment_id"],
                InvoiceId = new Invoice { InvoiceId = (int)dr["invoice_id"] },
                PaymentMethod = ParsePaymentMethod((string)dr["payment_method"]),
                Amount = (decimal)dr["total_price"],
                Feedback = dr["feedback"] != DBNull.Value ? (string)dr["feedback"] : null,
                FeedbackType = ParseFeedbackType(dr["feedback_type"] != DBNull.Value ? (string)dr["feedback_type"] : null),
                CreatedAt = (DateTime)dr["created_at"]
            };

            return payment;
        }

        // Parse payment method from string
        private PaymentMethod ParsePaymentMethod(string method)
        {
            if (Enum.TryParse<PaymentMethod>(method, true, out PaymentMethod result))
                return result;
            
            return PaymentMethod.Cash;
        }

        // Parse feedback type from string
        private FeedbackType ParseFeedbackType(string feedbackType)
        {
            if (string.IsNullOrEmpty(feedbackType))
                return FeedbackType.None;
                
            if (Enum.TryParse<FeedbackType>(feedbackType, true, out FeedbackType result))
                return result;
            
            return FeedbackType.None;
        }
    }
}