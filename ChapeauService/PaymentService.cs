using System;
using System.Collections.Generic;
using ChapeauDAL;
using ChapeauModel;

namespace ChapeauService
{
    public class PaymentService
    {
        private PaymentDao paymentDao;
        private InvoiceDao invoiceDao;
        private OrderDao orderDao;

        public PaymentService()
        {
            paymentDao = new PaymentDao();
            invoiceDao = new InvoiceDao();
            orderDao = new OrderDao();
        }

        // Calculate VAT amounts for an order and populate the invoice directly
        public void CalculateOrderTotals(List<OrderItem> orderItems, Invoice invoice)
        {
            decimal totalExVat = 0;
            decimal lowVatAmount = 0;  // 9% VAT
            decimal highVatAmount = 0; // 21% VAT
            decimal totalWithVat = 0;
            
            foreach (OrderItem item in orderItems)
            {
                // This is the total including VAT
                decimal itemTotal = item.Quantity * item.MenuItemId.Price;
                totalWithVat += itemTotal;
                
                // Use the VAT percentage
                int vatPercentage = item.MenuItemId.VatPercentage;
                
                // Extract the VAT that's already included in the price
                decimal itemVat;
                if (vatPercentage == 21)
                {
                    // High VAT rate (21%)
                    // VAT amount = Total - (Total / (1 + VAT rate))
                    itemVat = itemTotal - (itemTotal / 1.21m);
                    highVatAmount += itemVat;
                }
                else
                {
                    // Low VAT rate (9%) same formula
                    itemVat = itemTotal - (itemTotal / 1.09m);
                    lowVatAmount += itemVat;
                }
                
                // Calculate price excluding VAT (price without the VAT component)
                decimal itemExVat = itemTotal - itemVat;
                totalExVat += itemExVat;
            }
            
            // Populate the invoice with calculated totals
            invoice.TotalAmount = totalWithVat;
            invoice.TotalVat = lowVatAmount + highVatAmount;
            invoice.LowVatAmount = lowVatAmount;
            invoice.HighVatAmount = highVatAmount;
            invoice.TotalExcludingVat = totalExVat;
        }

        // Process complete invoice with all payments at once
        public void ProcessCompleteInvoice(Invoice invoice)
        {
            // First create the invoice in the database
            int invoiceId = invoiceDao.CreateInvoice(invoice);
            
            // Update the invoice ID in our object
            invoice.InvoiceId = invoiceId;
            
            // Process all payments for this invoice
            foreach (Payment payment in invoice.Payments)
            {
                // Ensure payment is linked to the correct invoice
                payment.Invoice = invoice;
                
                // Create each payment in the database
                int paymentId = paymentDao.CreatePayment(payment);
                
                // Update the payment ID in our object
                payment.PaymentId = paymentId;
            }
            
            // Finally, mark the order as done using the orderId from the invoice
            orderDao.MarkOrderAsDone(invoice.OrderId.OrderId);
        }
    }
}