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
            // First check if an invoice already exists
            Invoice existingInvoice = invoiceDao.GetInvoiceByOrderId(orderId);
            
            if (existingInvoice != null)
            {
                // Load payments for this invoice
                existingInvoice.Payments = paymentDao.GetPaymentsByInvoiceId(existingInvoice.InvoiceId);
                return existingInvoice;
            }
            
            // Generate a new invoice from order items
            Invoice newInvoice = invoiceDao.GenerateInvoiceForOrder(orderId);
            
            // If there are no items or all items are zero, there's an issue
            if (newInvoice.Items.Count == 0 || newInvoice.TotalAmount <= 0)
            {
                return null;
            }
            
            // Save the new invoice to the database
            newInvoice.InvoiceId = invoiceDao.CreateInvoice(newInvoice);
            newInvoice.Payments = new List<Payment>();
            
            return newInvoice;
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