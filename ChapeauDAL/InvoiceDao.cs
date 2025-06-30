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

        // Get invoice by ID
        public Invoice GetInvoiceById(int invoiceId)
        {
            string query = BuildInvoiceSelectByIdQuery();
            SqlParameter[] parameters = CreateInvoiceSelectByIdParameters(invoiceId);

            DataTable dataTable = ExecuteSelectQuery(query, parameters);
            
            if (dataTable.Rows.Count == 0)
                return null;

            Invoice invoice = ReadInvoice(dataTable.Rows[0]);
            LoadInvoicePayments(invoice);
            return invoice;
        }

        private string BuildInvoiceSelectByIdQuery()
        {
            return @"
                SELECT i.*, o.* 
                FROM Invoice i
                JOIN [Order] o ON i.order_id = o.order_id
                WHERE i.invoice_id = @InvoiceId";
        }

        private SqlParameter[] CreateInvoiceSelectByIdParameters(int invoiceId)
        {
            return new SqlParameter[]
            {
                new SqlParameter("@InvoiceId", invoiceId)
            };
        }

        // Get invoice by order ID
        public Invoice GetInvoiceByOrderId(int orderId)
        {
            string query = BuildInvoiceSelectByOrderIdQuery();
            SqlParameter[] parameters = CreateInvoiceSelectByOrderIdParameters(orderId);

            DataTable dataTable = ExecuteSelectQuery(query, parameters);
            
            if (dataTable == null || dataTable.Rows.Count == 0)
                return null;
            
            Invoice invoice = ReadInvoice(dataTable.Rows[0]);
            LoadInvoicePayments(invoice);
            return invoice;
        }

        private string BuildInvoiceSelectByOrderIdQuery()
        {
            return @"
                SELECT i.* 
                FROM Invoice i
                WHERE i.order_id = @OrderId";
        }

        private SqlParameter[] CreateInvoiceSelectByOrderIdParameters(int orderId)
        {
            return new SqlParameter[]
            {
                new SqlParameter("@OrderId", orderId)
            };
        }

        private void LoadInvoicePayments(Invoice invoice)
        {
            if (invoice != null)
            {
                invoice.Payments = GetPaymentsByInvoiceId(invoice.InvoiceId);
            }
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

        // Get all payments for a specific invoice
        public List<Payment> GetPaymentsByInvoiceId(int invoiceId)
        {
            string query = BuildPaymentsSelectQuery();
            SqlParameter[] parameters = CreatePaymentsSelectParameters(invoiceId);

            DataTable dataTable = ExecuteSelectQuery(query, parameters);
            
            List<Payment> payments = new List<Payment>();
            foreach (DataRow dr in dataTable.Rows)
            {
                payments.Add(ReadPayment(dr));
            }
            
            return payments;
        }

        private string BuildPaymentsSelectQuery()
        {
            return @"
                SELECT p.*
                FROM Payments p
                WHERE p.invoice_id = @InvoiceId
                ORDER BY p.payment_id";
        }

        private SqlParameter[] CreatePaymentsSelectParameters(int invoiceId)
        {
            return new SqlParameter[]
            {
                new SqlParameter("@InvoiceId", invoiceId)
            };
        }

        #endregion

        #region Helper Methods

        // Helper method to read an invoice from a DataRow
        private Invoice ReadInvoice(DataRow dr)
        {
            Invoice invoice = new Invoice
            {
                InvoiceId = (int)dr["invoice_id"],
                OrderId = new Order { OrderId = (int)dr["order_id"] },
                TotalAmount = (decimal)dr["total_amount"],
                TotalVat = (decimal)dr["total_vat"],
                TotalTipAmount = dr["total_tip_amount"] != DBNull.Value ? (decimal)dr["total_tip_amount"] : 0,
                CreatedAt = (DateTime)dr["created_at"]
            };

            return invoice;
        }

        // Helper method to read a payment from a DataRow
        private Payment ReadPayment(DataRow dr)
        {
            Payment payment = new Payment
            {
                PaymentId = (int)dr["payment_id"],
                Invoice = new Invoice { InvoiceId = (int)dr["invoice_id"] },
                PaymentMethod = ParsePaymentMethod((string)dr["payment_method"]),
                Amount = (decimal)dr["final_amount"],
                Feedback = GetFeedbackFromDataRow(dr),
                FeedbackType = ParseFeedbackType(GetFeedbackTypeFromDataRow(dr)),
                CreatedAt = (DateTime)dr["created_at"]
            };

            return payment;
        }

        private string GetFeedbackFromDataRow(DataRow dr)
        {
            return dr["feedback"] != DBNull.Value ? (string)dr["feedback"] : null;
        }

        private string GetFeedbackTypeFromDataRow(DataRow dr)
        {
            return dr["feedback_type"] != DBNull.Value ? (string)dr["feedback_type"] : null;
        }

        // Map enum values to database string values
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

        // Parse payment method from string
        private PaymentMethod ParsePaymentMethod(string method)
        {
            if (Enum.TryParse<PaymentMethod>(method.Replace(" ", ""), true, out PaymentMethod result))
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

        #endregion
    }
}