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
            decimal tipAmount = ExtractTipAmount(payment);
            decimal totalVatAmount = ExtractTotalVatAmount(payment);
            string paymentMethodString = GetPaymentMethodString(payment);
            decimal totalExclusivePrice = CalculateExclusivePrice(payment, totalVatAmount);
                        
            return ExecutePaymentInsert(payment, paymentMethodString, totalExclusivePrice, tipAmount, totalVatAmount);
        }

        // Gets the tip amount from the payment
        private decimal ExtractTipAmount(Payment payment)
        {
            if (payment.Invoice != null && payment.Invoice.TotalTipAmount > 0)
            {
                return payment.Invoice.TotalTipAmount;
            }
            return 0;
        }

        // Gets the total VAT amount from the payment
        private decimal ExtractTotalVatAmount(Payment payment)
        {
            if (payment.Invoice != null)
            {
                return payment.Invoice.TotalVat;
            }
            return 0;
        }

        // Converts the payment method enum to a string for database storage
        private string GetPaymentMethodString(Payment payment)
        {
            return MapPaymentMethodToDbValue(payment.PaymentMethod);
        }

        // Calculate the exclusive price without the VAT and tip
        private decimal CalculateExclusivePrice(Payment payment, decimal totalVatAmount)
        {
            if (payment.Invoice != null)
            {
                return payment.Invoice.TotalAmount - totalVatAmount;
            }
            return 0;
        }

        // Execute the insert query to create a new payment
        private int ExecutePaymentInsert(Payment payment, string paymentMethodString, decimal totalExclusivePrice, decimal tipAmount, decimal totalVatAmount)
        {
            string query = BuildInsertQuery();
            SqlParameter[] parameters = CreateInsertParameters(payment, paymentMethodString, totalExclusivePrice, tipAmount, totalVatAmount);

            return ExecuteInsertQuery(query, parameters);
        }

        // Build the SQL insert query for payments
        private string BuildInsertQuery()
        {
            return @"
                INSERT INTO Payments 
                (invoice_id, payment_method, total_price, feedback, final_amount, tip_amount, vat_amount) 
                VALUES 
                (@InvoiceId, @PaymentMethod, @TotalPrice, @Feedback, @FinalAmount, @TipAmount, @VatAmount);
                SELECT SCOPE_IDENTITY();";
        }

        // Create the parameters for the insert query
        private SqlParameter[] CreateInsertParameters(Payment payment, string paymentMethodString, decimal totalExclusivePrice, decimal tipAmount, decimal totalVatAmount)
        {
            return new SqlParameter[]
            {
                new SqlParameter("@InvoiceId", payment.Invoice.InvoiceId),
                new SqlParameter("@PaymentMethod", paymentMethodString),
                new SqlParameter("@TotalPrice", totalExclusivePrice), // Price excluding VAT and tip
                new SqlParameter("@Feedback", GetFeedbackValue(payment)),
                new SqlParameter("@FinalAmount", payment.Amount), // Total final amount with tip
                new SqlParameter("@TipAmount", tipAmount),
                new SqlParameter("@VatAmount", totalVatAmount) // Store VAT amount
            };
        }

        // Get feedback value, handling null or empty strings
        private object GetFeedbackValue(Payment payment)
        {
            return string.IsNullOrEmpty(payment.Feedback) ? (object)DBNull.Value : payment.Feedback;
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
    }
}