using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Linq;
using ChapeauModel;
using ChapeauService;
using Microsoft.Data.SqlClient;
using System.Text;

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
        private bool isSplittingBill = false;
        private int numberOfSplits = 2;
        private int currentSplitIndex = 0;
        private List<PaymentInfo> splitPayments;
        private bool isCustomSplit = false;
        private List<decimal> customSplitAmounts = new List<decimal>();

        private class PaymentInfo
        {
            public PaymentMethod Method { get; set; }
            public decimal Amount { get; set; }
            public string Feedback { get; set; }
            public FeedbackType FeedbackType { get; set; }
        }

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
            
            // If custom split is enabled, update custom amounts when tip changes
            if (isSplittingBill && isCustomSplit)
            {
                // Reinitialize custom amounts with the new total
                customSplitAmounts.Clear();
                decimal evenSplit = decimal.Round((totalWithVat + tipAmount) / numberOfSplits, 2);
                for (int i = 0; i < numberOfSplits; i++)
                {
                    customSplitAmounts.Add(evenSplit);
                }
                // Adjust last amount to account for rounding
                if (customSplitAmounts.Count > 0)
                {
                    decimal totalCustom = customSplitAmounts.Sum();
                    decimal totalActual = totalWithVat + tipAmount;
                    decimal difference = totalActual - totalCustom;
                    customSplitAmounts[customSplitAmounts.Count - 1] += difference;
                }
                UpdateCustomSplitDisplay();
            }
        }
        
        private void cmbFeedbackType_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Enable the feedback textbox only if a feedback type other than None is selected
            dynamic selectedItem = cmbFeedbackType.SelectedItem;
            FeedbackType feedbackType = (FeedbackType)selectedItem.Value;
            
            txtFeedback.Enabled = feedbackType != FeedbackType.None;
            txtFeedback.Text = feedbackType == FeedbackType.None ? "" : txtFeedback.Text;
        }

        private void chkSplitBill_CheckedChanged(object sender, EventArgs e)
        {
            isSplittingBill = chkSplitBill.Checked;
            nudNumberOfSplits.Visible = isSplittingBill;
            lblNumberOfSplits.Visible = isSplittingBill;
            chkCustomSplit.Visible = isSplittingBill;
            
            // Show the split amount if splitting
            if (isSplittingBill)
            {
                UpdateSplitAmount();
            }
            else
            {
                // Hide custom split controls when not splitting
                chkCustomSplit.Checked = false;
                isCustomSplit = false;
                btnConfigureCustomSplit.Visible = false;
                lblCustomSplitStatus.Visible = false;
                lblSplitAmount.Visible = false;
            }
        }

        private void nudNumberOfSplits_ValueChanged(object sender, EventArgs e)
        {
            numberOfSplits = (int)nudNumberOfSplits.Value;
            
            // If custom split is enabled, reinitialize custom amounts
            if (isCustomSplit)
            {
                customSplitAmounts.Clear();
                decimal evenSplit = decimal.Round((totalWithVat + tipAmount) / numberOfSplits, 2);
                for (int i = 0; i < numberOfSplits; i++)
                {
                    customSplitAmounts.Add(evenSplit);
                }
                // Adjust last amount to account for rounding
                if (customSplitAmounts.Count > 0)
                {
                    decimal totalCustom = customSplitAmounts.Sum();
                    decimal totalActual = totalWithVat + tipAmount;
                    decimal difference = totalActual - totalCustom;
                    customSplitAmounts[customSplitAmounts.Count - 1] += difference;
                }
                UpdateCustomSplitDisplay();
            }
            else
            {
                UpdateSplitAmount();
            }
        }

        private void UpdateSplitAmount()
        {
            if (isCustomSplit)
            {
                UpdateCustomSplitDisplay();
            }
            else
            {
                decimal finalAmount = totalWithVat + tipAmount;
                decimal splitAmount = decimal.Round(finalAmount / numberOfSplits, 2);
                lblSplitAmount.Text = $"Each person pays: €{splitAmount:0.00}";
                lblSplitAmount.Visible = true;
            }
        }

        private void btnProcessPayment_Click(object sender, EventArgs e)
        {
            try
            {
                if (isSplittingBill)
                {
                    ProcessSplitPayment();
                }
                else
                {
                    ProcessSinglePayment();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error processing payment: {ex.Message}", 
                    "Payment Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ProcessSinglePayment()
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

        private void ProcessSplitPayment()
        {
            // Get selected feedback type and text
            dynamic selectedFeedbackType = cmbFeedbackType.SelectedItem;
            FeedbackType feedbackType = (FeedbackType)selectedFeedbackType.Value;
            string feedback = txtFeedback.Text;

            // Validate custom split amounts if using custom split
            if (isCustomSplit)
            {
                if (customSplitAmounts.Count != numberOfSplits)
                {
                    MessageBox.Show(
                        "Please configure the custom split amounts before proceeding.",
                        "Custom Split Not Configured",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    return;
                }
                
                decimal totalCustom = customSplitAmounts.Sum();
                decimal expectedTotal = totalWithVat + tipAmount;
                if (Math.Abs(totalCustom - expectedTotal) >= 0.01m)
                {
                    MessageBox.Show(
                        $"The sum of custom amounts (€{totalCustom:0.00}) does not match the total bill (€{expectedTotal:0.00}).\n\nPlease reconfigure the amounts.",
                        "Amount Mismatch",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    return;
                }
            }
            
            // Create invoice
            int invoiceId = paymentService.CreateInvoice(
                orderId, 
                totalWithVat, 
                lowVatAmount + highVatAmount, 
                lowVatAmount, 
                highVatAmount, 
                totalExVat, 
                tipAmount);
            
            // Initialize list to track payments
            splitPayments = new List<PaymentInfo>();
            currentSplitIndex = 0;
            
            // Show split payment dialog for each split
            if (isCustomSplit)
            {
                ProcessNextCustomSplitPayment(invoiceId, feedback, feedbackType);
            }
            else
            {
                // Calculate the total amount and split amount
                decimal finalAmount = totalWithVat + tipAmount;
                decimal splitAmount = decimal.Round(finalAmount / numberOfSplits, 2);
                
                // Calculate the last split amount
                decimal lastSplitAmount = finalAmount - (splitAmount * (numberOfSplits - 1));
                
                ProcessNextSplitPayment(invoiceId, splitAmount, lastSplitAmount, feedback, feedbackType);
            }
        }

        private void ProcessNextSplitPayment(int invoiceId, decimal splitAmount, decimal lastSplitAmount, string feedback, FeedbackType feedbackType)
        {
            currentSplitIndex++;
            
            if (currentSplitIndex > numberOfSplits)
            {
                // All splits have been processed
                FinalizeSplitPayment();
                return;
            }
            
            // Create a payment dialog for this split
            using (Form paymentDialog = new Form())
            {
                paymentDialog.Text = $"Payment {currentSplitIndex} of {numberOfSplits}";
                paymentDialog.Size = new Size(400, 350);
                paymentDialog.StartPosition = FormStartPosition.CenterParent;
                paymentDialog.FormBorderStyle = FormBorderStyle.FixedDialog;
                paymentDialog.MaximizeBox = false;
                paymentDialog.MinimizeBox = false;
                
                // Amount to pay
                decimal amountToPay = (currentSplitIndex == numberOfSplits) ? lastSplitAmount : splitAmount;
                
                Label lblAmount = new Label();
                lblAmount.Text = $"Amount: €{amountToPay:0.00}";
                lblAmount.Location = new Point(20, 20);
                lblAmount.Size = new Size(200, 25);
                lblAmount.Font = new Font("Segoe UI", 11, FontStyle.Bold);
                
                // Payment method
                Label lblMethod = new Label();
                lblMethod.Text = "Payment Method:";
                lblMethod.Location = new Point(20, 60);
                lblMethod.Size = new Size(150, 25);
                
                ComboBox cmbMethod = new ComboBox();
                cmbMethod.Location = new Point(170, 60);
                cmbMethod.Size = new Size(200, 25);
                cmbMethod.DropDownStyle = ComboBoxStyle.DropDownList;
                cmbMethod.DisplayMember = "Text";
                cmbMethod.ValueMember = "Value";
                
                cmbMethod.Items.Add(new { Text = "Cash", Value = PaymentMethod.Cash });
                cmbMethod.Items.Add(new { Text = "Debit Card", Value = PaymentMethod.DebitCard });
                cmbMethod.Items.Add(new { Text = "Credit Card", Value = PaymentMethod.CreditCard });
                cmbMethod.SelectedIndex = 0;

                // Add feedback type selector
                Label lblFeedbackType = new Label();
                lblFeedbackType.Text = "Feedback Type:";
                lblFeedbackType.Location = new Point(20, 100);
                lblFeedbackType.Size = new Size(150, 25);
                
                ComboBox cmbFeedbackType = new ComboBox();
                cmbFeedbackType.Location = new Point(170, 100);
                cmbFeedbackType.Size = new Size(200, 25);
                cmbFeedbackType.DropDownStyle = ComboBoxStyle.DropDownList;
                cmbFeedbackType.DisplayMember = "Text";
                cmbFeedbackType.ValueMember = "Value";
                
                // Add feedback type options
                cmbFeedbackType.Items.Add(new { Text = "None", Value = FeedbackType.None });
                cmbFeedbackType.Items.Add(new { Text = "Comment", Value = FeedbackType.Comment });
                cmbFeedbackType.Items.Add(new { Text = "Complaint", Value = FeedbackType.Complaint });
                cmbFeedbackType.Items.Add(new { Text = "Recommendation", Value = FeedbackType.Recommendation });
                cmbFeedbackType.SelectedIndex = 0;
                
                // Add feedback text field
                Label lblSplitFeedback = new Label();
                lblSplitFeedback.Text = "Feedback:";
                lblSplitFeedback.Location = new Point(20, 140);
                lblSplitFeedback.Size = new Size(150, 25);
                
                TextBox txtSplitFeedback = new TextBox();
                txtSplitFeedback.Location = new Point(20, 170);
                txtSplitFeedback.Size = new Size(350, 80);
                txtSplitFeedback.Multiline = true;
                txtSplitFeedback.Enabled = false; // Initially disabled until a feedback type is selected
                
                // Enable/disable feedback text field based on selected feedback type
                cmbFeedbackType.SelectedIndexChanged += (s, e) => {
                    dynamic selectedItem = cmbFeedbackType.SelectedItem;
                    FeedbackType ft = (FeedbackType)selectedItem.Value;
                    txtSplitFeedback.Enabled = ft != FeedbackType.None;
                    if (ft == FeedbackType.None)
                        txtSplitFeedback.Text = "";
                };
                
                // Pay Button
                Button btnPay = new Button();
                btnPay.Text = "Pay";
                btnPay.DialogResult = DialogResult.OK;
                btnPay.Location = new Point(200, 270);
                btnPay.Size = new Size(80, 30);
                
                Button btnCancel = new Button();
                btnCancel.Text = "Cancel";
                btnCancel.DialogResult = DialogResult.Cancel;
                btnCancel.Location = new Point(290, 270);
                btnCancel.Size = new Size(80, 30);
                
                paymentDialog.Controls.Add(lblAmount);
                paymentDialog.Controls.Add(lblMethod);
                paymentDialog.Controls.Add(cmbMethod);
                paymentDialog.Controls.Add(lblFeedbackType);
                paymentDialog.Controls.Add(cmbFeedbackType);
                paymentDialog.Controls.Add(lblSplitFeedback);
                paymentDialog.Controls.Add(txtSplitFeedback);
                paymentDialog.Controls.Add(btnPay);
                paymentDialog.Controls.Add(btnCancel);
                
                paymentDialog.AcceptButton = btnPay;
                paymentDialog.CancelButton = btnCancel;
                
                if (paymentDialog.ShowDialog() == DialogResult.OK)
                {
                    // Record this payment
                    dynamic selectedMethod = cmbMethod.SelectedItem;
                    PaymentMethod paymentMethod = (PaymentMethod)selectedMethod.Value;
                    
                    // Get feedback information
                    dynamic selectedFeedbackType = cmbFeedbackType.SelectedItem;
                    FeedbackType splitFeedbackType = (FeedbackType)selectedFeedbackType.Value;
                    string splitFeedback = txtSplitFeedback.Text;
                    int paymentId;
                    
                    // Use the feedback from this split payment dialog
                    if (currentSplitIndex == numberOfSplits)
                    {
                        // Last payment marks the order as done
                        paymentId = paymentService.ProcessPayment(
                            invoiceId, 
                            paymentMethod, 
                            amountToPay, 
                            splitFeedback, 
                            splitFeedbackType,
                            true); // Mark order as done
                    }
                    else
                    {
                        // All other payments use their own feedback but don't mark as done
                        paymentId = paymentService.ProcessPayment(
                            invoiceId, 
                            paymentMethod, 
                            amountToPay, 
                            splitFeedback, 
                            splitFeedbackType,
                            false); // Don't mark order as done yet
                    }
                    
                    // Save the payment info
                    splitPayments.Add(new PaymentInfo
                    {
                        Method = paymentMethod,
                        Amount = amountToPay,
                        Feedback = splitFeedback,
                        FeedbackType = splitFeedbackType
                    });
                    
                    // Process the next split
                    ProcessNextSplitPayment(invoiceId, splitAmount, lastSplitAmount, feedback, feedbackType);
                }
                else
                {
                    // User canceled - need to clean up partially processed payments
                    MessageBox.Show("Payment process was canceled.", 
                        "Payment Canceled", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.DialogResult = DialogResult.Cancel;
                    this.Close();
                }
            }
        }

        private void ProcessNextCustomSplitPayment(int invoiceId, string feedback, FeedbackType feedbackType)
        {
            currentSplitIndex++;
            
            if (currentSplitIndex > numberOfSplits)
            {
                // All splits have been processed
                FinalizeSplitPayment();
                return;
            }
            
            // Get the custom amount for this split
            decimal amountToPay = customSplitAmounts[currentSplitIndex - 1];
            
            // Create a payment dialog for this split
            using (Form paymentDialog = new Form())
            {
                paymentDialog.Text = $"Payment {currentSplitIndex} of {numberOfSplits} (Custom Amount)";
                paymentDialog.Size = new Size(400, 350);
                paymentDialog.StartPosition = FormStartPosition.CenterParent;
                paymentDialog.FormBorderStyle = FormBorderStyle.FixedDialog;
                paymentDialog.MaximizeBox = false;
                paymentDialog.MinimizeBox = false;
                
                // Amount to pay
                Label lblAmount = new Label();
                lblAmount.Text = $"Amount: €{amountToPay:0.00}";
                lblAmount.Location = new Point(20, 20);
                lblAmount.Size = new Size(200, 25);
                lblAmount.Font = new Font("Segoe UI", 11, FontStyle.Bold);
                
                // Payment method
                Label lblMethod = new Label();
                lblMethod.Text = "Payment Method:";
                lblMethod.Location = new Point(20, 60);
                lblMethod.Size = new Size(150, 25);
                
                ComboBox cmbMethod = new ComboBox();
                cmbMethod.Location = new Point(170, 60);
                cmbMethod.Size = new Size(200, 25);
                cmbMethod.DropDownStyle = ComboBoxStyle.DropDownList;
                cmbMethod.DisplayMember = "Text";
                cmbMethod.ValueMember = "Value";
                
                cmbMethod.Items.Add(new { Text = "Cash", Value = PaymentMethod.Cash });
                cmbMethod.Items.Add(new { Text = "Debit Card", Value = PaymentMethod.DebitCard });
                cmbMethod.Items.Add(new { Text = "Credit Card", Value = PaymentMethod.CreditCard });
                cmbMethod.SelectedIndex = 0;

                // Add feedback type selector
                Label lblFeedbackType = new Label();
                lblFeedbackType.Text = "Feedback Type:";
                lblFeedbackType.Location = new Point(20, 100);
                lblFeedbackType.Size = new Size(150, 25);
                
                ComboBox cmbFeedbackType = new ComboBox();
                cmbFeedbackType.Location = new Point(170, 100);
                cmbFeedbackType.Size = new Size(200, 25);
                cmbFeedbackType.DropDownStyle = ComboBoxStyle.DropDownList;
                cmbFeedbackType.DisplayMember = "Text";
                cmbFeedbackType.ValueMember = "Value";
                
                // Add feedback type options
                cmbFeedbackType.Items.Add(new { Text = "None", Value = FeedbackType.None });
                cmbFeedbackType.Items.Add(new { Text = "Comment", Value = FeedbackType.Comment });
                cmbFeedbackType.Items.Add(new { Text = "Complaint", Value = FeedbackType.Complaint });
                cmbFeedbackType.Items.Add(new { Text = "Recommendation", Value = FeedbackType.Recommendation });
                cmbFeedbackType.SelectedIndex = 0;
                
                // Add feedback text field
                Label lblSplitFeedback = new Label();
                lblSplitFeedback.Text = "Feedback:";
                lblSplitFeedback.Location = new Point(20, 140);
                lblSplitFeedback.Size = new Size(150, 25);
                
                TextBox txtSplitFeedback = new TextBox();
                txtSplitFeedback.Location = new Point(20, 170);
                txtSplitFeedback.Size = new Size(350, 80);
                txtSplitFeedback.Multiline = true;
                txtSplitFeedback.Enabled = false; // Initially disabled until a feedback type is selected
                
                // Enable/disable feedback text field based on selected feedback type
                cmbFeedbackType.SelectedIndexChanged += (s, e) => {
                    dynamic selectedItem = cmbFeedbackType.SelectedItem;
                    FeedbackType ft = (FeedbackType)selectedItem.Value;
                    txtSplitFeedback.Enabled = ft != FeedbackType.None;
                    if (ft == FeedbackType.None)
                        txtSplitFeedback.Text = "";
                };
                
                // Pay Button
                Button btnPay = new Button();
                btnPay.Text = "Pay";
                btnPay.DialogResult = DialogResult.OK;
                btnPay.Location = new Point(200, 270);
                btnPay.Size = new Size(80, 30);
                
                Button btnCancel = new Button();
                btnCancel.Text = "Cancel";
                btnCancel.DialogResult = DialogResult.Cancel;
                btnCancel.Location = new Point(290, 270);
                btnCancel.Size = new Size(80, 30);
                
                paymentDialog.Controls.Add(lblAmount);
                paymentDialog.Controls.Add(lblMethod);
                paymentDialog.Controls.Add(cmbMethod);
                paymentDialog.Controls.Add(lblFeedbackType);
                paymentDialog.Controls.Add(cmbFeedbackType);
                paymentDialog.Controls.Add(lblSplitFeedback);
                paymentDialog.Controls.Add(txtSplitFeedback);
                paymentDialog.Controls.Add(btnPay);
                paymentDialog.Controls.Add(btnCancel);
                
                paymentDialog.AcceptButton = btnPay;
                paymentDialog.CancelButton = btnCancel;
                
                if (paymentDialog.ShowDialog() == DialogResult.OK)
                {
                    // Record this payment
                    dynamic selectedMethod = cmbMethod.SelectedItem;
                    PaymentMethod paymentMethod = (PaymentMethod)selectedMethod.Value;
                    
                    // Get feedback information
                    dynamic selectedFeedbackType = cmbFeedbackType.SelectedItem;
                    FeedbackType splitFeedbackType = (FeedbackType)selectedFeedbackType.Value;
                    string splitFeedback = txtSplitFeedback.Text;
                    int paymentId;
                    
                    // Use the feedback from this split payment dialog
                    if (currentSplitIndex == numberOfSplits)
                    {
                        // Last payment marks the order as done
                        paymentId = paymentService.ProcessPayment(
                            invoiceId, 
                            paymentMethod, 
                            amountToPay, 
                            splitFeedback, 
                            splitFeedbackType,
                            true); // Mark order as done
                    }
                    else
                    {
                        // All other payments use their own feedback but don't mark as done
                        paymentId = paymentService.ProcessPayment(
                            invoiceId, 
                            paymentMethod, 
                            amountToPay, 
                            splitFeedback, 
                            splitFeedbackType,
                            false); // Don't mark order as done yet
                    }
                    
                    // Save the payment info
                    splitPayments.Add(new PaymentInfo
                    {
                        Method = paymentMethod,
                        Amount = amountToPay,
                        Feedback = splitFeedback,
                        FeedbackType = splitFeedbackType
                    });
                    
                    // Process the next split
                    ProcessNextCustomSplitPayment(invoiceId, feedback, feedbackType);
                }
                else
                {
                    // User canceled - need to clean up partially processed payments
                    MessageBox.Show("Payment process was canceled.", 
                        "Payment Canceled", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.DialogResult = DialogResult.Cancel;
                    this.Close();
                }
            }
        }

        private void FinalizeSplitPayment()
        {
            // Show a summary of the payments
            StringBuilder summary = new StringBuilder();
            summary.AppendLine("Split Bill Payment Summary:");
            summary.AppendLine("---------------------------");
            
            for (int i = 0; i < splitPayments.Count; i++)
            {
                summary.AppendLine($"Payment {i+1}: {splitPayments[i].Method} - €{splitPayments[i].Amount:0.00}");
                
                // Add feedback information
                if (splitPayments[i].FeedbackType != FeedbackType.None && !string.IsNullOrEmpty(splitPayments[i].Feedback))
                {
                    summary.AppendLine($"  Feedback ({splitPayments[i].FeedbackType}): {splitPayments[i].Feedback}");
                }
            }
            
            MessageBox.Show(summary.ToString(), "Payment Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void chkCustomSplit_CheckedChanged(object sender, EventArgs e)
        {
            isCustomSplit = chkCustomSplit.Checked;
            
            if (isCustomSplit)
            {
                // Show custom split configuration controls
                btnConfigureCustomSplit.Visible = true;
                lblCustomSplitStatus.Visible = true;
                
                // Initialize custom amounts if not already set
                if (customSplitAmounts.Count != numberOfSplits)
                {
                    customSplitAmounts.Clear();
                    decimal evenSplit = decimal.Round((totalWithVat + tipAmount) / numberOfSplits, 2);
                    for (int i = 0; i < numberOfSplits; i++)
                    {
                        customSplitAmounts.Add(evenSplit);
                    }
                    // Adjust last amount to account for rounding
                    if (customSplitAmounts.Count > 0)
                    {
                        decimal totalCustom = customSplitAmounts.Sum();
                        decimal totalActual = totalWithVat + tipAmount;
                        decimal difference = totalActual - totalCustom;
                        customSplitAmounts[customSplitAmounts.Count - 1] += difference;
                    }
                }
                
                UpdateCustomSplitDisplay();
                lblSplitAmount.Visible = false; // Hide equal split display
            }
            else
            {
                // Hide custom split configuration controls
                btnConfigureCustomSplit.Visible = false;
                lblCustomSplitStatus.Visible = false;
                UpdateSplitAmount(); // Show equal split display
            }
        }

        private void btnConfigureCustomSplit_Click(object sender, EventArgs e)
        {
            ShowCustomSplitDialog();
        }

        private void UpdateCustomSplitDisplay()
        {
            if (customSplitAmounts.Count > 0)
            {
                decimal totalCustom = customSplitAmounts.Sum();
                decimal totalActual = totalWithVat + tipAmount;
                
                if (Math.Abs(totalCustom - totalActual) < 0.01m) // Account for rounding differences
                {
                    lblCustomSplitStatus.Text = $"Custom amounts configured (Total: €{totalCustom:0.00})";
                    lblCustomSplitStatus.ForeColor = Color.DarkGreen;
                }
                else
                {
                    lblCustomSplitStatus.Text = $"Custom amounts mismatch! Expected: €{totalActual:0.00}, Current: €{totalCustom:0.00}";
                    lblCustomSplitStatus.ForeColor = Color.Red;
                }
            }
            else
            {
                lblCustomSplitStatus.Text = "Custom amounts not configured";
                lblCustomSplitStatus.ForeColor = Color.DarkBlue;
            }
        }

        private void ShowCustomSplitDialog()
        {
            using (Form customSplitDialog = new Form())
            {
                customSplitDialog.Text = "Configure Custom Split Amounts";
                customSplitDialog.Size = new Size(500, 400);
                customSplitDialog.StartPosition = FormStartPosition.CenterParent;
                customSplitDialog.FormBorderStyle = FormBorderStyle.FixedDialog;
                customSplitDialog.MaximizeBox = false;
                customSplitDialog.MinimizeBox = false;
                
                // Header label
                Label lblHeader = new Label();
                lblHeader.Text = $"Total Amount to Split: €{(totalWithVat + tipAmount):0.00}";
                lblHeader.Location = new Point(20, 20);
                lblHeader.Size = new Size(450, 25);
                lblHeader.Font = new Font("Segoe UI", 11, FontStyle.Bold);
                lblHeader.TextAlign = ContentAlignment.MiddleCenter;
                
                // Create text boxes for each split amount
                List<TextBox> amountTextBoxes = new List<TextBox>();
                int yPosition = 60;
                
                for (int i = 0; i < numberOfSplits; i++)
                {
                    Label lblPerson = new Label();
                    lblPerson.Text = $"Person {i + 1}:";
                    lblPerson.Location = new Point(20, yPosition);
                    lblPerson.Size = new Size(80, 25);
                    lblPerson.TextAlign = ContentAlignment.MiddleLeft;
                    
                    TextBox txtAmount = new TextBox();
                    txtAmount.Location = new Point(110, yPosition);
                    txtAmount.Size = new Size(100, 25);
                    txtAmount.TextAlign = HorizontalAlignment.Right;
                    txtAmount.Text = customSplitAmounts.Count > i ? customSplitAmounts[i].ToString("0.00") : "0.00";
                    
                    Label lblEuro = new Label();
                    lblEuro.Text = "€";
                    lblEuro.Location = new Point(220, yPosition);
                    lblEuro.Size = new Size(20, 25);
                    lblEuro.TextAlign = ContentAlignment.MiddleLeft;
                    
                    customSplitDialog.Controls.Add(lblPerson);
                    customSplitDialog.Controls.Add(txtAmount);
                    customSplitDialog.Controls.Add(lblEuro);
                    amountTextBoxes.Add(txtAmount);
                    
                    yPosition += 35;
                }
                
                // Total display
                Label lblTotal = new Label();
                lblTotal.Location = new Point(20, yPosition + 10);
                lblTotal.Size = new Size(220, 25);
                lblTotal.Font = new Font("Segoe UI", 10, FontStyle.Bold);
                lblTotal.Text = "Total of amounts above: €0.00";
                
                // Update total when amounts change
                EventHandler updateTotal = (s, e) =>
                {
                    decimal total = 0;
                    foreach (TextBox tb in amountTextBoxes)
                    {
                        if (decimal.TryParse(tb.Text, out decimal amount))
                        {
                            total += amount;
                        }
                    }
                    lblTotal.Text = $"Total of amounts above: €{total:0.00}";
                    
                    decimal expected = totalWithVat + tipAmount;
                    if (Math.Abs(total - expected) < 0.01m)
                    {
                        lblTotal.ForeColor = Color.DarkGreen;
                    }
                    else
                    {
                        lblTotal.ForeColor = Color.Red;
                    }
                };
                
                // Attach event handlers
                foreach (TextBox tb in amountTextBoxes)
                {
                    tb.TextChanged += updateTotal;
                }
                
                customSplitDialog.Controls.Add(lblHeader);
                customSplitDialog.Controls.Add(lblTotal);
                
                // Buttons
                Button btnOK = new Button();
                btnOK.Text = "OK";
                btnOK.DialogResult = DialogResult.OK;
                btnOK.Location = new Point(250, yPosition + 40);
                btnOK.Size = new Size(80, 30);
                
                Button btnCancel = new Button();
                btnCancel.Text = "Cancel";
                btnCancel.DialogResult = DialogResult.Cancel;
                btnCancel.Location = new Point(340, yPosition + 40);
                btnCancel.Size = new Size(80, 30);
                
                Button btnEqualSplit = new Button();
                btnEqualSplit.Text = "Equal Split";
                btnEqualSplit.Location = new Point(250, yPosition + 10);
                btnEqualSplit.Size = new Size(100, 25);
                btnEqualSplit.BackColor = Color.LightBlue;
                btnEqualSplit.Click += (s, e) =>
                {
                    decimal evenAmount = decimal.Round((totalWithVat + tipAmount) / numberOfSplits, 2);
                    for (int i = 0; i < amountTextBoxes.Count; i++)
                    {
                        amountTextBoxes[i].Text = evenAmount.ToString("0.00");
                    }
                    // Adjust last amount for rounding
                    if (amountTextBoxes.Count > 0)
                    {
                        decimal totalEven = evenAmount * numberOfSplits;
                        decimal actualTotal = totalWithVat + tipAmount;
                        decimal difference = actualTotal - totalEven;
                        decimal lastAmount = evenAmount + difference;
                        amountTextBoxes[amountTextBoxes.Count - 1].Text = lastAmount.ToString("0.00");
                    }
                };
                
                customSplitDialog.Controls.Add(btnEqualSplit);
                customSplitDialog.Controls.Add(btnOK);
                customSplitDialog.Controls.Add(btnCancel);
                
                customSplitDialog.AcceptButton = btnOK;
                customSplitDialog.CancelButton = btnCancel;
                
                // Trigger initial total calculation
                updateTotal(this, EventArgs.Empty);
                
                if (customSplitDialog.ShowDialog() == DialogResult.OK)
                {
                    // Save the custom amounts
                    customSplitAmounts.Clear();
                    decimal totalEntered = 0;
                    
                    foreach (TextBox tb in amountTextBoxes)
                    {
                        if (decimal.TryParse(tb.Text, out decimal amount))
                        {
                            customSplitAmounts.Add(amount);
                            totalEntered += amount;
                        }
                        else
                        {
                            customSplitAmounts.Add(0);
                        }
                    }
                    
                    // Validate total
                    decimal expectedTotal = totalWithVat + tipAmount;
                    if (Math.Abs(totalEntered - expectedTotal) >= 0.01m)
                    {
                        MessageBox.Show(
                            $"The sum of individual amounts (€{totalEntered:0.00}) does not match the total bill amount (€{expectedTotal:0.00}).\n\nPlease adjust the amounts so they add up correctly.",
                            "Amount Mismatch",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning);
                        
                        // Don't save invalid amounts, show dialog again
                        ShowCustomSplitDialog();
                        return;
                    }
                    
                    UpdateCustomSplitDisplay();
                }
            }
        }
    }
}