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

        // Get order with all items for payment
        public Order GetOrderForPayment(int orderId)
        {
            return orderDao.GetOrderWithItemsById(orderId);
        }

        // Calculate VAT amounts for an order - corrected for prices that already include VAT
        public (decimal totalExVat, decimal lowVat, decimal highVat, decimal totalWithVat) CalculateOrderTotals(List<OrderItem> orderItems)
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
                    // Low VAT rate (9%)
                    itemVat = itemTotal - (itemTotal / 1.09m);
                    lowVatAmount += itemVat;
                }
                
                // Calculate price excluding VAT (price without the VAT component)
                decimal itemExVat = itemTotal - itemVat;
                totalExVat += itemExVat;
            }
            
            return (totalExVat, lowVatAmount, highVatAmount, totalWithVat);
        }

        // Create an invoice for an order
        public int CreateInvoice(int orderId, decimal totalAmount, decimal totalVat, 
                               decimal lowVatAmount, decimal highVatAmount, 
                               decimal totalExcludingVat, decimal tipAmount)
        {
            Invoice invoice = new Invoice
            {
                OrderId = new Order { OrderId = orderId },
                TotalAmount = totalAmount,
                TotalVat = totalVat,
                // Store the calculated VAT values for reference
                LowVatAmount = lowVatAmount,
                HighVatAmount = highVatAmount,
                TotalExcludingVat = totalExcludingVat,
                TotalTipAmount = tipAmount,
                CreatedAt = DateTime.Now
            };
            
            return invoiceDao.CreateInvoice(invoice);
        }

        // Process payment for an invoice
        public int ProcessPayment(int invoiceId, PaymentMethod paymentMethod, 
                                decimal amount, string feedback, FeedbackType feedbackType)
        {
            // Get the full invoice object from the database instead of creating a new one
            Invoice invoice = invoiceDao.GetInvoiceById(invoiceId);
            
            Payment payment = new Payment
            {
                InvoiceId = invoice, // Use the full invoice object with tip amount
                PaymentMethod = paymentMethod,
                Amount = amount,
                Feedback = feedback,
                FeedbackType = feedbackType,
                CreatedAt = DateTime.Now
            };
            
            int paymentId = paymentDao.CreatePayment(payment);
            
            // Mark the order as done
            if (invoice != null)
            {
                orderDao.MarkOrderAsDone(invoice.OrderId.OrderId);
            }
            
            return paymentId;
        }

        // Check if an order has an existing invoice
        public Invoice GetInvoiceForOrder(int orderId)
        {
            return invoiceDao.GetInvoiceByOrderId(orderId);
        }
    }
}