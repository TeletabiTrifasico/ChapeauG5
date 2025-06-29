using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using ChapeauModel;

namespace ChapeauDAL
{
    // Handles creating, retrieving, and managing invoices with their payments
    public class InvoiceDao : BaseDao
    {
        #region Invoice Operations

        // Create a new invoice for an order
        public int CreateInvoice(Invoice invoice)
        {
            string query = BuildInvoiceInsertQuery();
            SqlParameter[] parameters = CreateInvoiceInsertParameters(invoice);
            
            return ExecuteInsertQuery(query, parameters);
        }

        private string BuildInvoiceInsertQuery()
        {
            return @"
                INSERT INTO Invoice 
                (order_id, total_amount, total_vat, total_tip_amount, created_at) 
                VALUES 
                (@OrderId, @TotalAmount, @TotalVat, @TotalTipAmount, @CreatedAt);
                SELECT SCOPE_IDENTITY();";
        }

        private SqlParameter[] CreateInvoiceInsertParameters(Invoice invoice)
        {
            return new SqlParameter[]
            {
                new SqlParameter("@OrderId", invoice.OrderId.OrderId),
                new SqlParameter("@TotalAmount", invoice.TotalAmount),
                new SqlParameter("@TotalVat", invoice.TotalVat),
                new SqlParameter("@TotalTipAmount", invoice.TotalTipAmount),
                new SqlParameter("@CreatedAt", invoice.CreatedAt)
            };
        }

        #endregion

        #region Payment Operations

        // Create a new payment for an invoice
        public int CreatePayment(Payment payment)
        {
            decimal tipAmount = ExtractTipAmount(payment);
            decimal totalVatAmount = ExtractTotalVatAmount(payment);
            string paymentMethodString = GetPaymentMethodString(payment);
            decimal totalExclusivePrice = CalculateExclusivePrice(payment, totalVatAmount);
            
            return ExecutePaymentInsert(payment, paymentMethodString, totalExclusivePrice, tipAmount, totalVatAmount);
        }

        private decimal ExtractTipAmount(Payment payment)
        {
            if (payment.Invoice != null && payment.Invoice.TotalTipAmount > 0)
            {
                return payment.Invoice.TotalTipAmount;
            }
            return 0;
        }

        private decimal ExtractTotalVatAmount(Payment payment)
        {
            if (payment.Invoice != null)
            {
                return payment.Invoice.TotalVat;
            }
            return 0;
        }

        private string GetPaymentMethodString(Payment payment)
        {
            return MapPaymentMethodToDbValue(payment.PaymentMethod);
        }

        private decimal CalculateExclusivePrice(Payment payment, decimal totalVatAmount)
        {
            if (payment.Invoice != null)
            {
                return payment.Invoice.TotalAmount - totalVatAmount;
            }
            return 0;
        }

        private int ExecutePaymentInsert(Payment payment, string paymentMethodString, decimal totalExclusivePrice, decimal tipAmount, decimal totalVatAmount)
        {
            string query = BuildPaymentInsertQuery();
            SqlParameter[] parameters = CreatePaymentInsertParameters(payment, paymentMethodString, totalExclusivePrice, tipAmount, totalVatAmount);
            
            return ExecuteInsertQuery(query, parameters);
        }

        private string BuildPaymentInsertQuery()
        {
            return @"
                INSERT INTO Payments 
                (invoice_id, payment_method, total_price, feedback, final_amount, tip_amount, vat_amount) 
                VALUES 
                (@InvoiceId, @PaymentMethod, @TotalPrice, @Feedback, @FinalAmount, @TipAmount, @VatAmount);
                SELECT SCOPE_IDENTITY();";
        }

        private SqlParameter[] CreatePaymentInsertParameters(Payment payment, string paymentMethodString, decimal totalExclusivePrice, decimal tipAmount, decimal totalVatAmount)
        {
            return new SqlParameter[]
            {
                new SqlParameter("@InvoiceId", payment.Invoice.InvoiceId),
                new SqlParameter("@PaymentMethod", paymentMethodString),
                new SqlParameter("@TotalPrice", totalExclusivePrice),
                new SqlParameter("@Feedback", GetFeedbackValue(payment)),
                new SqlParameter("@FinalAmount", payment.Amount),
                new SqlParameter("@TipAmount", tipAmount),
                new SqlParameter("@VatAmount", totalVatAmount)
            };
        }

        private object GetFeedbackValue(Payment payment)
        {
            return string.IsNullOrEmpty(payment.Feedback) ? (object)DBNull.Value : payment.Feedback;
        }

        #endregion

        #region Helper Methods
        
        private string MapPaymentMethodToDbValue(PaymentMethod method)
        {
            switch (method)
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

        #endregion
    }
}