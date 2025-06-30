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
        // Creates a new invoice record in the database.
        public int CreateInvoice(Invoice invoice)
        {
            // Query to insert a new invoice record and return its ID
            string query = @"INSERT INTO Invoice (order_id, total_amount, total_vat, total_tip_amount, created_at)
                            VALUES (@OrderId, @TotalAmount, @TotalVat, @TotalTipAmount, @CreatedAt);
                            SELECT SCOPE_IDENTITY();";

            // Parameters to prevent SQL injection and ensure type safety
            SqlParameter[] parameters = new SqlParameter[]
            {
                // Extract OrderId value from Order object
                new SqlParameter("@OrderId", invoice.OrderId != null ? invoice.OrderId.OrderId : (object)DBNull.Value),
                new SqlParameter("@TotalAmount", invoice.TotalAmount),
                new SqlParameter("@TotalVat", invoice.TotalVat),
                new SqlParameter("@TotalTipAmount", invoice.TotalTipAmount),
                new SqlParameter("@CreatedAt", invoice.CreatedAt)
            };

            // Execute the insert query and return the new invoice ID
            return ExecuteInsertQuery(query, parameters);
        }

        // Retrieves an invoice from the database by its associated order ID.
        public Invoice? GetInvoiceByOrderId(int orderId)
        {
            // Query to find invoice by order ID
            string query = "SELECT * FROM Invoice WHERE order_id = @OrderId";
            SqlParameter[] parameters = { new SqlParameter("@OrderId", orderId) };

            // Execute the query and get the result table
            DataTable dataTable = ExecuteSelectQuery(query, parameters);

            // Return null if no invoice found
            if (dataTable.Rows.Count == 0)
                return null;

            // Convert the data row to an Invoice object
            return ReadTable(dataTable.Rows[0]);
        }

        // Retrieves an invoice from the database by its invoice ID. (Same as above)
        public Invoice? GetInvoiceById(int invoiceId)
        {
            string query = "SELECT * FROM Invoice WHERE invoice_id = @InvoiceId";
            SqlParameter[] parameters = { new SqlParameter("@InvoiceId", invoiceId) };

            DataTable dataTable = ExecuteSelectQuery(query, parameters);

            if (dataTable.Rows.Count == 0)
                return null;

            return ReadTable(dataTable.Rows[0]);
        }

        // Helper method to convert a DataRow from the Invoice table into an Invoice object.
        private Invoice ReadTable(DataRow dr)
        {
            // Map database columns to Invoice properties
            Invoice invoice = new Invoice
            {
                InvoiceId = (int)dr["invoice_id"],
                // Create Order object with just the ID
                OrderId = new Order { OrderId = (int)dr["order_id"] },
                TotalAmount = (decimal)dr["total_amount"],
                TotalVat = (decimal)dr["total_vat"],
                TotalTipAmount = (decimal)dr["total_tip_amount"],
                CreatedAt = (DateTime)dr["created_at"]
            };

            return invoice;
        }

        // Generates a new invoice by querying order details and calculating totals.
        // This doesn't save the invoice to the database, just creates the object.
        public Invoice GenerateInvoiceForOrder(int orderId)
        {
            // SQL query that joins tables to get all order details:
            // - Orders table for basic order information
            // - Order_items for the items in the order
            // - Menu_items for item details like price and VAT percentage
            // - Drink_Item to determine if items are alcoholic (for VAT calculations)
            // Only includes items with status "Served"
            string query = @"
                SELECT o.order_id, o.employee_id, oi.order_item_id, oi.menu_item_id, oi.quantity, oi.comment,
                       mi.name as item_name, mi.price, mi.vat_percentage, di.is_alcoholic
                FROM [Order] o
                JOIN Order_Item oi ON o.order_id = oi.order_id
                JOIN Menu_Item mi ON oi.menu_item_id = mi.menu_item_id
                LEFT JOIN Drink_Item di ON mi.menu_item_id = di.menu_item_id
                WHERE o.order_id = @OrderId AND oi.status = 'Served'";

            SqlParameter[] parameters = { new SqlParameter("@OrderId", orderId) };

            // Execute the query and get all the order items
            DataTable dataTable = ExecuteSelectQuery(query, parameters);

            // Create a new invoice object
            Invoice invoice = new Invoice();
            // Create Order object with just the ID
            invoice.OrderId = new Order { OrderId = orderId };

            // Initialize running totals
            decimal totalAmount = 0;
            decimal totalVat = 0;

            // Process each order item row from the query result
            foreach (DataRow dr in dataTable.Rows)
            {
                // Extract values from the row
                int vatPercentage = (int)dr["vat_percentage"];
                decimal price = Convert.ToDecimal(dr["price"]);
                int quantity = (int)dr["quantity"];

                // Check if item is alcoholic (for special VAT rules)
                // is_alcoholic will be DBNull for non-drink items, so need to handle that case
                bool isAlcoholic = dr["is_alcoholic"] != DBNull.Value && (bool)dr["is_alcoholic"];

                // Calculate item totals
                decimal itemSubtotal = price * quantity;
                decimal itemVat = itemSubtotal * (vatPercentage / 100m);

                // Create an InvoiceItem object to represent this line item
                InvoiceItem item = new InvoiceItem()
                {
                    // Create OrderItem object with just the ID
                    OrderItemId = new OrderItem { OrderItemId = (int)dr["order_item_id"] },
                    // Create MenuItem object with just the ID
                    MenuItemId = new MenuItem { MenuItemId = (int)dr["menu_item_id"] },
                    ItemName = (string)dr["item_name"],
                    UnitPrice = price,
                    Quantity = quantity,
                    VatPercentage = vatPercentage,
                    Subtotal = itemSubtotal,
                    VatAmount = itemVat,
                    IsAlcoholic = isAlcoholic
                };

                // Add the item to the invoice's items collection
                invoice.Items.Add(item);

                // Add to running totals
                totalAmount += itemSubtotal;
                totalVat += itemVat;
            }

            // Set the calculated totals on the invoice
            invoice.TotalAmount = totalAmount;
            invoice.TotalVat = totalVat;

            return invoice;
        }




    


        // InvoiceDao method:

        public List<Invoice> GetInvoicesBetween(DateTime start, DateTime end)
        {
            string query = "SELECT * FROM Invoice WHERE created_at BETWEEN @StartDate AND @EndDate";
            SqlParameter[] parameters = new SqlParameter[]
            {
        new SqlParameter("@StartDate", start),
        new SqlParameter("@EndDate", end)
            };

            DataTable dt = ExecuteSelectQuery(query, parameters);
            List<Invoice> invoices = new List<Invoice>();

            foreach (DataRow row in dt.Rows)
            {
                invoices.Add(ReadTable(row));
            }

            return invoices;
        }
    }
}
