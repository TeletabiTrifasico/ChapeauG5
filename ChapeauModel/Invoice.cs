using System;
using System.Collections.Generic;

namespace ChapeauModel
{
    public class Invoice
    {
        public int InvoiceId { get; set; }
        public int OrderId { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal TotalVat { get; set; }
        public decimal TotalTipAmount { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<Payment> Payments { get; set; }
        public List<InvoiceItem> Items { get; set; }
        
        // Initializes a new invoice
        public Invoice()
        {
            Payments = new List<Payment>();
            Items = new List<InvoiceItem>();
            CreatedAt = DateTime.Now;
        }
        
        // Splits the current invoice into multiple invoices for bill sharing.
        // Items are distributed evenly among the specified number of people.
        public List<Invoice> SplitInvoice(int numberOfPeople)
        {
            // List to hold all the split invoices
            List<Invoice> splitInvoices = new List<Invoice>();
            
            // Create one invoice per person, inheriting the original order ID
            for (int i = 0; i < numberOfPeople; i++)
            {
                splitInvoices.Add(new Invoice
                {
                    OrderId = this.OrderId,
                    TotalAmount = 0,
                    TotalVat = 0,
                    CreatedAt = DateTime.Now
                });
            }
            
            // Distribute items evenly
            int itemIndex = 0;
            foreach (var item in this.Items)
            {
                int remainingQuantity = item.Quantity;
                
                // Continue until we've distributed all of this item
                while (remainingQuantity > 0)
                {
                    int invoiceIndex = itemIndex % numberOfPeople;
                    int quantityForInvoice = (remainingQuantity + numberOfPeople - 1) / numberOfPeople;
                    
                    // Safety check to prevent overallocation
                    if (quantityForInvoice > remainingQuantity)
                        quantityForInvoice = remainingQuantity;
                    
                    // Create a new invoice item with appropriate quantities and calculated amounts
                    InvoiceItem splitItem = new InvoiceItem
                    {
                        OrderItemId = item.OrderItemId,
                        MenuItemId = item.MenuItemId,
                        ItemName = item.ItemName,
                        UnitPrice = item.UnitPrice,
                        Quantity = quantityForInvoice,
                        VatPercentage = item.VatPercentage,
                        
                        // Calculate subtotal (price * quantity)
                        Subtotal = Math.Round(item.UnitPrice * quantityForInvoice, 2),
                        
                        // Calculate VAT amount based on subtotal and VAT percentage
                        VatAmount = Math.Round(item.UnitPrice * quantityForInvoice * (item.VatPercentage / 100), 2),
                        
                        // Preserve the alcoholic status (important for different VAT rates)
                        IsAlcoholic = item.IsAlcoholic
                    };
                    
                    // Add this item portion to the appropriate split invoice
                    splitInvoices[invoiceIndex].Items.Add(splitItem);
                    
                    // Reduce the remaining quantity and move to next allocation position
                    remainingQuantity -= quantityForInvoice;
                    itemIndex++;
                }
            }
            
            // Calculate totals for each split invoice by summing their items
            foreach (var invoice in splitInvoices)
            {
                decimal totalAmount = 0;
                decimal totalVat = 0;
                
                // Sum up the subtotals and VAT amounts for all items in this split invoice
                foreach (var item in invoice.Items)
                {
                    totalAmount += item.Subtotal;
                    totalVat += item.VatAmount;
                }
                
                // Set the calculated totals on the invoice
                invoice.TotalAmount = totalAmount;
                invoice.TotalVat = totalVat;
            }
            
            // Return the collection of split invoices
            return splitInvoices;
        }
    }
}