using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using ChapeauModel;

namespace ChapeauDAL
{
    // Handles creating, retrieving, and generating invoices from order data.
    public class InvoiceDao : BaseDao
    {
        // Create a new invoice for an order
        public int CreateInvoice(Invoice invoice)
        {
            string query = @"
                INSERT INTO Invoice 
                (order_id, total_amount, total_vat, total_tip_amount, created_at) 
                VALUES 
                (@OrderId, @TotalAmount, @TotalVat, @TotalTipAmount, @CreatedAt);
                SELECT SCOPE_IDENTITY();";

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@OrderId", invoice.OrderId.OrderId),
                new SqlParameter("@TotalAmount", invoice.TotalAmount),
                new SqlParameter("@TotalVat", invoice.TotalVat),
                new SqlParameter("@TotalTipAmount", invoice.TotalTipAmount),
                new SqlParameter("@CreatedAt", invoice.CreatedAt)
            };

            return ExecuteInsertQuery(query, parameters);
        }

        // Get invoice by ID
        public Invoice GetInvoiceById(int invoiceId)
        {
            string query = @"
                SELECT i.*, o.* 
                FROM Invoice i
                JOIN [Order] o ON i.order_id = o.order_id
                WHERE i.invoice_id = @InvoiceId";

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@InvoiceId", invoiceId)
            };

            DataTable dataTable = ExecuteSelectQuery(query, parameters);
            
            if (dataTable.Rows.Count == 0)
                return null;

            return ReadInvoice(dataTable.Rows[0]);
        }

        // Get invoice by order ID
        public Invoice GetInvoiceByOrderId(int orderId)
        {
            string query = @"
                SELECT i.* 
                FROM Invoice i
                WHERE i.order_id = @OrderId";

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@OrderId", orderId)
            };

            DataTable dataTable = ExecuteSelectQuery(query, parameters);
            
            if (dataTable == null || dataTable.Rows.Count == 0)
                return null;
            
            return ReadInvoice(dataTable.Rows[0]);
        }

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
    }
}