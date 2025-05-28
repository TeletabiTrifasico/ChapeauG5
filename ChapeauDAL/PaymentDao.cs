using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using ChapeauModel;

namespace ChapeauDAL
{
    // Handles creating and retrieving payment records.
    public class PaymentDao : BaseDao
    {
        // Registers a new payment in the database.
        public int RegisterPayment(Payment payment)
        {
            // Query to insert a new payment record and return its ID
            string query = @"INSERT INTO Payments 
                          (invoice_id, feedback, payment_method, total_price, vat_percentage, tip_amount, final_amount) 
                          VALUES (@InvoiceId, @Feedback, @PaymentMethod, @TotalPrice, @VatPercentage, @TipAmount, @FinalAmount);
                          SELECT SCOPE_IDENTITY();";
            
            // This prevents SQL injection and ensures type safety
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@InvoiceId", payment.InvoiceId),
                new SqlParameter("@Feedback", payment.Feedback ?? (object)DBNull.Value),
                new SqlParameter("@PaymentMethod", (int)payment.PaymentMethod),
                new SqlParameter("@TotalPrice", payment.TotalPrice),
                new SqlParameter("@VatPercentage", payment.VatPercentage),
                new SqlParameter("@TipAmount", payment.TipAmount),
                new SqlParameter("@FinalAmount", payment.FinalAmount)
            };
            
            // Execute the query and return the ID of the newly created payment
            return ExecuteInsertQuery(query, parameters);
        }
        
        // Retrieves all payments associated with a specific invoice.
        public List<Payment> GetPaymentsByInvoiceId(int invoiceId)
        {
            string query = "SELECT * FROM Payments WHERE invoice_id = @InvoiceId";
            SqlParameter[] parameters = { new SqlParameter("@InvoiceId", invoiceId) };
            
            // Execute the query and convert the result to a list of Payment objects
            return ReadTables(ExecuteSelectQuery(query, parameters));
        }
        
        // Helper method to convert a DataTable containing payment records into a list of Payment objects.
        private List<Payment> ReadTables(DataTable dataTable)
        {
            // Create a list to hold the payment objects
            List<Payment> payments = new List<Payment>();
            
            // Process each row in the result set
            foreach (DataRow dr in dataTable.Rows)
            {
                // Create a new Payment object and map each database column to its properties
                Payment payment = new Payment()
                {
                    PaymentId = (int)dr["payment_id"],
                    InvoiceId = (int)dr["invoice_id"],
                    Feedback = dr["feedback"] != DBNull.Value ? (string)dr["feedback"] : string.Empty,
                    PaymentMethod = (PaymentMethod)(int)dr["payment_method"],
                    TotalPrice = (decimal)dr["total_price"],
                    VatPercentage = (int)dr["vat_percentage"],
                    TipAmount = (decimal)dr["tip_amount"],
                    FinalAmount = (decimal)dr["final_amount"]
                };
                
                // Add the payment to our collection
                payments.Add(payment);
            }
            
            // Return the complete list of payments
            return payments;
        }
    }
}