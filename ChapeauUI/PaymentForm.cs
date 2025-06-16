using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Linq;
using ChapeauModel;
using ChapeauService;
using Microsoft.Data.SqlClient;

namespace ChapeauG5
{
    public partial class PaymentForm : Form
    {
        private int orderId;
        private Employee loggedInEmployee;
        private Order currentOrder;
        private PaymentService paymentService;
        private decimal totalExVat = 0;
        private decimal lowVatAmount = 0;
        private decimal highVatAmount = 0;
        private decimal totalWithVat = 0;
        private decimal tipAmount = 0;
        
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
                // Load the order with all items
                currentOrder = paymentService.GetOrderForPayment(orderId);
                
                if (currentOrder == null)
                {
                    Console.WriteLine($"Error: Order #{orderId} not found in database");
                    MessageBox.Show($"Order #{orderId} not found in the database.", "Order Not Found", 
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    this.Close();
                    return;
                }
                
                if (currentOrder.OrderItems == null || currentOrder.OrderItems.Count == 0)
                {
                    Console.WriteLine($"Error: Order #{orderId} has no items");
                    MessageBox.Show("No items found in this order.", "Empty Order", 
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    this.Close();
                    return;
                }
                
                // Set the table number in the header
                lblHeader.Text = $"Payment for Table {currentOrder.TableId.TableNumber}";
                // Load order items
                LoadOrderItems();
                // Calculate totals
                CalculateTotals();
                // Setup payment method combo box
                SetupPaymentMethodComboBox();
                // Setup feedback type combo box
                SetupFeedbackTypeComboBox();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Critical error in PaymentForm_Load: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                MessageBox.Show($"Error loading payment information: {ex.Message}", 
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
            }
        }

        private void LoadOrderItems()
        {
            lvOrderItems.Items.Clear();
            
            foreach (OrderItem item in currentOrder.OrderItems)
            {
                // Get VAT rate directly from the menu item
                string vatRate = item.MenuItemId.VatPercentage == 21 ? "21%" : "9%";
                decimal subtotal = item.Quantity * item.MenuItemId.Price;
                
                ListViewItem lvi = new ListViewItem(item.MenuItemId.Name);
                lvi.SubItems.Add(item.Quantity.ToString());
                lvi.SubItems.Add($"€{item.MenuItemId.Price:0.00}");
                lvi.SubItems.Add(vatRate);
                lvi.SubItems.Add($"€{subtotal:0.00}");
                lvi.Tag = item;
                
                lvOrderItems.Items.Add(lvi);
            }
        }

        private void CalculateTotals()
        {
            // Calculate VAT amounts
            var totals = paymentService.CalculateOrderTotals(currentOrder.OrderItems);
            
            totalExVat = totals.totalExVat;
            lowVatAmount = totals.lowVat;
            highVatAmount = totals.highVat;
            totalWithVat = totals.totalWithVat;
            
            // Update display
            lblSubtotalValue.Text = $"€{totalExVat:0.00}";
            lblLowVatValue.Text = $"€{lowVatAmount:0.00}";
            lblHighVatValue.Text = $"€{highVatAmount:0.00}";
            lblTotalValue.Text = $"€{totalWithVat:0.00}";
            
            // Calculate final amount with tip
            CalculateFinalAmount();
        }
        
        private void CalculateFinalAmount()
        {
            // Get tip amount from input
            if (decimal.TryParse(txtTip.Text, out decimal tip))
            {
                tipAmount = tip;
            }
            else
            {
                tipAmount = 0;
            }
            // Calculate final amount
            decimal finalAmount = totalWithVat + tipAmount;
            // Update display
            lblFinalAmountValue.Text = $"€{finalAmount:0.00}";
        }

        private void SetupPaymentMethodComboBox()
        {
            cmbPaymentMethod.Items.Clear();
            cmbPaymentMethod.DisplayMember = "Text";
            cmbPaymentMethod.ValueMember = "Value";
            cmbPaymentMethod.Items.Add(new { Text = "Cash", Value = PaymentMethod.Cash });
            cmbPaymentMethod.Items.Add(new { Text = "Debit Card", Value = PaymentMethod.DebitCard });
            cmbPaymentMethod.Items.Add(new { Text = "Credit Card", Value = PaymentMethod.CreditCard });
            
            cmbPaymentMethod.SelectedIndex = 0;
        }
        
        private void SetupFeedbackTypeComboBox()
        {
            cmbFeedbackType.Items.Clear();
            cmbFeedbackType.DisplayMember = "Text";
            cmbFeedbackType.ValueMember = "Value";
            
            cmbFeedbackType.Items.Add(new { Text = "None", Value = FeedbackType.None });
            cmbFeedbackType.Items.Add(new { Text = "Comment", Value = FeedbackType.Comment });
            cmbFeedbackType.Items.Add(new { Text = "Complaint", Value = FeedbackType.Complaint });
            cmbFeedbackType.Items.Add(new { Text = "Recommendation", Value = FeedbackType.Recommendation });
            
            cmbFeedbackType.SelectedIndex = 0;
        }

        private void txtTip_TextChanged(object sender, EventArgs e)
        {
            CalculateFinalAmount();
        }
        
        private void cmbFeedbackType_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Enable the feedback textbox only if a feedback type other than None is selected
            dynamic selectedItem = cmbFeedbackType.SelectedItem;
            FeedbackType feedbackType = (FeedbackType)selectedItem.Value;
            
            txtFeedback.Enabled = feedbackType != FeedbackType.None;
            txtFeedback.Text = feedbackType == FeedbackType.None ? "" : txtFeedback.Text;
        }

        private void btnProcessPayment_Click(object sender, EventArgs e)
        {
            try
            {
                // Get selected payment method
                dynamic selectedPaymentMethod = cmbPaymentMethod.SelectedItem;
                PaymentMethod paymentMethod = (PaymentMethod)selectedPaymentMethod.Value;
                // Get selected feedback type
                dynamic selectedFeedbackType = cmbFeedbackType.SelectedItem;
                FeedbackType feedbackType = (FeedbackType)selectedFeedbackType.Value;
                // Get feedback text
                string feedback = txtFeedback.Text;
                // Confirm payment
                DialogResult result = MessageBox.Show(
                    $"Process payment of {lblFinalAmountValue.Text} using {selectedPaymentMethod.Text}?",
                    "Confirm Payment",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);
                
                if (result == DialogResult.Yes)
                {
                    // Create invoice
                    int invoiceId = paymentService.CreateInvoice(
                        orderId, 
                        totalWithVat, 
                        lowVatAmount + highVatAmount, 
                        lowVatAmount, 
                        highVatAmount, 
                        totalExVat, 
                        tipAmount);
                    // Process payment
                    int paymentId = paymentService.ProcessPayment(
                        invoiceId, 
                        paymentMethod, 
                        totalWithVat + tipAmount, 
                        feedback, 
                        feedbackType);
                    MessageBox.Show(
                        "Payment processed successfully!",
                        "Payment Complete",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error processing payment: {ex.Message}", 
                    "Payment Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}