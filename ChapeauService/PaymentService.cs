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
        
        public PaymentService()
        {
            paymentDao = new PaymentDao();
            invoiceDao = new InvoiceDao();
        }
        
        public Invoice GetInvoice(int orderId)
        {
            // Check if invoice already exists
            Invoice invoice = invoiceDao.GetInvoiceByOrderId(orderId);
            
            if (invoice == null)
            {
                // Generate a new invoice based on order items
                invoice = invoiceDao.GenerateInvoiceForOrder(orderId);
                
                // Save the invoice to the database
                int invoiceId = invoiceDao.CreateInvoice(invoice);
                invoice.InvoiceId = invoiceId;
            }
            
            // Load the invoice items
            if (invoice.Items.Count == 0)
            {
                Invoice detailedInvoice = invoiceDao.GenerateInvoiceForOrder(orderId);
                invoice.Items = detailedInvoice.Items;
            }
            
            // Load any existing payments
            invoice.Payments = paymentDao.GetPaymentsByInvoiceId(invoice.InvoiceId);
            
            return invoice;
        }
        
        public List<Invoice> SplitInvoice(Invoice invoice, int numberOfPeople)
        {
            return invoice.SplitInvoice(numberOfPeople);
        }
        
        public int ProcessPayment(Payment payment)
        {
            return paymentDao.RegisterPayment(payment);
        }
        
        public List<Payment> GetPaymentsByInvoiceId(int invoiceId)
        {
            return paymentDao.GetPaymentsByInvoiceId(invoiceId);
        }
        
        public bool IsOrderPaid(int orderId)
        {
            Invoice invoice = invoiceDao.GetInvoiceByOrderId(orderId);
            if (invoice == null)
                return false;
                
            List<Payment> payments = paymentDao.GetPaymentsByInvoiceId(invoice.InvoiceId);
            
            if (payments.Count == 0)
                return false;
                
            decimal totalPaid = 0;
            foreach (Payment payment in payments)
            {
                totalPaid += payment.FinalAmount;
            }
            
            // Order is considered paid if total payment is equal to or greater than invoice total
            return totalPaid >= (invoice.TotalAmount + invoice.TotalVat);
        }
    }
}