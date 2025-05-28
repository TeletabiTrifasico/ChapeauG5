using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using ChapeauModel;
using ChapeauService;

namespace ChapeauG5
{
    public partial class PaymentForm : Form
    {
        private int orderId;
        private Employee loggedInEmployee;
        private PaymentService paymentService;
        private Invoice invoice;

        public PaymentForm(int orderId, Employee employee)
        {
            InitializeComponent();
            this.orderId = orderId;
            this.loggedInEmployee = employee;
            paymentService = new PaymentService();
            
            // Add the Load event handler
            this.Load += PaymentForm_Load;
        }

        private void PaymentForm_Load(object sender, EventArgs e)
        {
            try
            {
                Console.WriteLine($"Loading payment form for order ID: {orderId}");
                
                // Load the invoice information
                invoice = paymentService.GetInvoice(orderId);
                
                if (invoice == null)
                {
                    MessageBox.Show("No invoice could be loaded for this order. Make sure items are marked as served.", 
                        "Invoice Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    this.DialogResult = DialogResult.Cancel;
                    this.Close();
                    return;
                }
                
                // Display invoice items
                DisplayInvoice();
                
                // Initialize payment method options
                cmbPaymentMethod.Items.Clear();
                cmbPaymentMethod.Items.Add("Cash");
                cmbPaymentMethod.Items.Add("Debit Card");
                cmbPaymentMethod.Items.Add("Credit Card (VISA)");
                cmbPaymentMethod.Items.Add("Credit Card (AMEX)");
                cmbPaymentMethod.SelectedIndex = 0;
                
                // Make sure panels are visible
                pnlPayment.Visible = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading payment form: {ex.Message}", 
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        
        private void DisplayInvoice()
        {
            try
            {
                // Set order info
                lblOrderInfo.Text = $"Order #{orderId} - Invoice #{invoice.InvoiceId}";
                
                // Clear and populate invoice items
                lvInvoiceItems.Items.Clear();
                
                foreach (InvoiceItem item in invoice.Items)
                {
                    ListViewItem lvi = new ListViewItem(item.ItemName);
                    lvi.SubItems.Add(item.Quantity.ToString());
                    lvi.SubItems.Add($"€{item.UnitPrice:0.00}");
                    lvi.SubItems.Add($"€{item.Subtotal:0.00}");
                    lvi.SubItems.Add($"{item.VatPercentage}%");
                    lvInvoiceItems.Items.Add(lvi);
                }
                
                // Display totals
                lblSubtotal.Text = $"€{invoice.TotalAmount:0.00}";
                lblVatAmount.Text = $"€{invoice.TotalVat:0.00}";
                decimal finalTotal = invoice.TotalAmount + invoice.TotalVat;
                lblTotal.Text = $"€{finalTotal:0.00}";
            }
            catch (Exception ex)
            {
                throw new Exception($"Error displaying invoice: {ex.Message}", ex);
            }
        }
        
        // Implement your other methods (btnProcessPayment_Click, btnSplitBill_Click, etc.)
    }
}