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
        private Invoice invoice;
        private Employee loggedInEmployee;
        private PaymentService paymentService;
        
        public PaymentForm(int orderId, Employee employee)
        {
            InitializeComponent();
            this.orderId = orderId;
            this.loggedInEmployee = employee;
            paymentService = new PaymentService();
        }
        
        private void PaymentForm_Load(object sender, EventArgs e)
        {
            try
            {
                // Load the invoice information
                invoice = paymentService.GetInvoice(orderId);
                
                if (invoice == null)
                {
                    MessageBox.Show("Error: Could not load invoice for this order. The order might not have any served items.", 
                        "Invoice Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    this.DialogResult = DialogResult.Cancel;
                    this.Close();
                    return;
                }
                
                DisplayInvoice();
                
                // Check if invoice already has payments
                if (invoice.Payments.Count > 0)
                {
                    ShowExistingPayments();
                }
                
                // Initialize payment method options
                cmbPaymentMethod.Items.Add("Cash");
                cmbPaymentMethod.Items.Add("Debit Card");
                cmbPaymentMethod.Items.Add("Credit Card (VISA)");
                cmbPaymentMethod.Items.Add("Credit Card (AMEX)");
                cmbPaymentMethod.SelectedIndex = 0;
                
                // Make sure the payment panel is visible
                pnlPayment.Visible = true;
                
                // Log for debugging
                Console.WriteLine($"Invoice loaded: {invoice.InvoiceId}, Items: {invoice.Items.Count}, Total: {invoice.TotalAmount}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading payment form: {ex.Message}\n\n{ex.StackTrace}", 
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        
        private void DisplayInvoice()
        {
            // Set order info
            lblOrderInfo.Text = $"Order #{orderId} - Invoice #{invoice.InvoiceId}";
            
            // Display invoice items in a list view
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
            lblTotal.Text = $"€{(invoice.TotalAmount + invoice.TotalVat):0.00}";
        }
        
        private void ShowExistingPayments()
        {
            // Show existing payments
            pnlExistingPayments.Visible = true;
            
            lvExistingPayments.Items.Clear();
            decimal totalPaid = 0;
            
            foreach (Payment payment in invoice.Payments)
            {
                ListViewItem lvi = new ListViewItem(payment.PaymentId.ToString());
                lvi.SubItems.Add(GetPaymentMethodName(payment.PaymentMethod));
                lvi.SubItems.Add($"€{payment.TotalPrice:0.00}");
                lvi.SubItems.Add($"€{payment.TipAmount:0.00}");
                lvi.SubItems.Add($"€{payment.FinalAmount:0.00}");
                
                totalPaid += payment.FinalAmount;
                lvExistingPayments.Items.Add(lvi);
            }
            
            lblTotalPaid.Text = $"Total Paid: €{totalPaid:0.00}";
            decimal remaining = (invoice.TotalAmount + invoice.TotalVat) - totalPaid;
            
            if (remaining <= 0)
            {
                lblRemaining.Text = "Fully Paid";
                lblRemaining.ForeColor = System.Drawing.Color.Green;
                pnlPayment.Enabled = false;
                btnSplitBill.Enabled = false;
            }
            else
            {
                lblRemaining.Text = $"Remaining: €{remaining:0.00}";
                lblRemaining.ForeColor = System.Drawing.Color.Red;
            }
        }
        
        private string GetPaymentMethodName(PaymentMethod method)
        {
            switch (method)
            {
                case PaymentMethod.Cash: return "Cash";
                case PaymentMethod.DebitCard: return "Debit Card";
                case PaymentMethod.CreditCardVisa: return "Credit Card (VISA)";
                case PaymentMethod.CreditCardAmex: return "Credit Card (AMEX)";
                default: return "Unknown";
            }
        }
        
        private void btnSplitBill_Click(object sender, EventArgs e)
        {
            using (var form = new SplitBillDialog())
            {
                if (form.ShowDialog() == DialogResult.OK)
                {
                    int numberOfPeople = form.NumberOfPeople;
                    List<Invoice> splitInvoices = paymentService.SplitInvoice(invoice, numberOfPeople);
                    
                    // Show split invoices
                    using (var splitForm = new SplitInvoicesForm(splitInvoices, invoice.InvoiceId, loggedInEmployee))
                    {
                        splitForm.ShowDialog();
                        if (splitForm.DialogResult == DialogResult.OK)
                        {
                            // Refresh the invoice view
                            invoice = paymentService.GetInvoice(orderId);
                            ShowExistingPayments();
                        }
                    }
                }
            }
        }
        
        private void btnProcessPayment_Click(object sender, EventArgs e)
        {
            try
            {
                // Get the payment method
                PaymentMethod paymentMethod = PaymentMethod.Cash;
                switch (cmbPaymentMethod.SelectedIndex)
                {
                    case 0: paymentMethod = PaymentMethod.Cash; break;
                    case 1: paymentMethod = PaymentMethod.DebitCard; break;
                    case 2: paymentMethod = PaymentMethod.CreditCardVisa; break;
                    case 3: paymentMethod = PaymentMethod.CreditCardAmex; break;
                }
                
                // Get tip amount
                decimal tipAmount = 0;
                if (decimal.TryParse(txtTipAmount.Text, out tipAmount))
                {
                    // Valid decimal input
                }
                else
                {
                    MessageBox.Show("Please enter a valid tip amount.", "Invalid Input", 
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                
                // Get feedback
                string feedback = string.Empty;
                if (!string.IsNullOrEmpty(txtFeedback.Text))
                {
                    if (rdoComplaint.Checked)
                        feedback = "Complaint: " + txtFeedback.Text;
                    else if (rdoCommendation.Checked)
                        feedback = "Commendation: " + txtFeedback.Text;
                    else
                        feedback = txtFeedback.Text;
                }
                
                // Calculate remaining amount to pay
                decimal totalToPay = invoice.TotalAmount + invoice.TotalVat;
                decimal alreadyPaid = 0;
                
                foreach (Payment existingPayment in invoice.Payments)
                {
                    alreadyPaid += existingPayment.FinalAmount;
                }
                
                decimal remainingAmount = totalToPay - alreadyPaid;
                
                if (remainingAmount <= 0)
                {
                    MessageBox.Show("This invoice is already fully paid.", "Information", 
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                
                // Create payment
                Payment payment = new Payment
                {
                    InvoiceId = invoice.InvoiceId,
                    Feedback = feedback,
                    PaymentMethod = paymentMethod,
                    TotalPrice = remainingAmount,
                    VatPercentage = (int)Math.Round(invoice.TotalVat / invoice.TotalAmount * 100, 0), // Average VAT percentage
                    TipAmount = tipAmount,
                    FinalAmount = remainingAmount + tipAmount,
                    EmployeeId = loggedInEmployee.EmployeeId
                };
                
                // Process payment
                int paymentId = paymentService.ProcessPayment(payment);
                
                // Update invoice tip amount
                invoice.TotalTipAmount += tipAmount;
                
                MessageBox.Show("Payment processed successfully!", "Success", 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error processing payment: {ex.Message}", 
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}