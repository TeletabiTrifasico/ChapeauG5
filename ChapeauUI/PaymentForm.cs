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
        // Reduced global variables - only essential ones
        private readonly int orderId;
        private readonly Employee loggedInEmployee;
        private readonly PaymentService paymentService;
        private readonly OrderService orderService;
        
        // Main invoice object that contains all payment-related data including payments list
        private Invoice currentInvoice;
        private Order currentOrder;
        
        // Split payment tracking - now using the invoice's payment collection
        private int currentSplitIndex = 0;

        public PaymentForm(int orderId, Employee employee)
        {
            InitializeComponent();
            this.orderId = orderId;
            this.loggedInEmployee = employee;
            this.paymentService = new PaymentService();
            this.orderService = new OrderService();
            
            // Initialize the main invoice object with empty payments list
            this.currentInvoice = new Invoice();
        }

        private void PaymentForm_Load(object sender, EventArgs e)
        {
            try
            {                
                // Load the order with all items using OrderService
                currentOrder = orderService.GetOrderWithItemsById(orderId);
                
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
                
                // Initialize invoice with order totals
                InitializeInvoiceData();
                
                // Load order items
                LoadOrderItems();
                // Calculate and display totals
                UpdateTotalsDisplay();
                // Setup combo boxes
                SetupPaymentMethodComboBox();
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

        private void InitializeInvoiceData()
        {
            // Calculate order totals and store in the invoice
            var totals = paymentService.CalculateOrderTotals(currentOrder.OrderItems);
            
            // Set invoice properties with calculated totals
            currentInvoice.OrderId = currentOrder;
            currentInvoice.TotalAmount = totals.totalWithVat;
            currentInvoice.TotalVat = totals.lowVat + totals.highVat;
            currentInvoice.LowVatAmount = totals.lowVat;
            currentInvoice.HighVatAmount = totals.highVat;
            currentInvoice.TotalExcludingVat = totals.totalExVat;
            currentInvoice.TotalTipAmount = 0; // Will be updated when tip is entered
            currentInvoice.CreatedAt = DateTime.Now;
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

        private void UpdateTotalsDisplay()
        {
            // Update display labels using invoice data
            lblSubtotalValue.Text = $"€{currentInvoice.TotalExcludingVat:0.00}";
            lblLowVatValue.Text = $"€{currentInvoice.LowVatAmount:0.00}";
            lblHighVatValue.Text = $"€{currentInvoice.HighVatAmount:0.00}";
            lblTotalValue.Text = $"€{currentInvoice.TotalAmount:0.00}";
            
            // Calculate and display final amount with tip
            UpdateFinalAmount();
        }
        
        private void UpdateFinalAmount()
        {
            // Get tip amount from input
            if (decimal.TryParse(txtTip.Text, out decimal tip))
            {
                currentInvoice.TotalTipAmount = tip;
            }
            else
            {
                currentInvoice.TotalTipAmount = 0;
            }
            
            // Calculate total amount including tip
            decimal finalAmount = currentInvoice.TotalAmount + currentInvoice.TotalTipAmount;
            
            // Update display
            lblFinalAmountValue.Text = $"€{finalAmount:0.00}";
        }

        private decimal GetFinalAmount()
        {
            return currentInvoice.TotalAmount + currentInvoice.TotalTipAmount;
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
            UpdateFinalAmount();
            
            // If custom split is enabled, update custom amounts when tip changes
            if (IsSplittingBill() && IsCustomSplit())
            {
                ReinitializeCustomSplitAmounts();
            }
        }

        private bool IsSplittingBill()
        {
            return chkSplitBill.Checked;
        }

        private bool IsCustomSplit()
        {
            return chkCustomSplit.Checked;
        }

        private int GetNumberOfSplits()
        {
            return (int)nudNumberOfSplits.Value;
        }

        private void ReinitializeCustomSplitAmounts()
        {
            // Clear existing payments in the invoice and reinitialize with even amounts
            currentInvoice.Payments.Clear();
            decimal finalAmount = GetFinalAmount();
            decimal evenSplit = decimal.Round(finalAmount / GetNumberOfSplits(), 2);
            
            for (int i = 0; i < GetNumberOfSplits(); i++)
            {
                Payment splitPayment = new Payment
                {
                    Amount = evenSplit,
                    InvoiceId = currentInvoice
                };
                currentInvoice.Payments.Add(splitPayment);
            }
            
            // Adjust last amount to account for rounding
            if (currentInvoice.Payments.Count > 0)
            {
                decimal totalCustom = currentInvoice.Payments.Sum(p => p.Amount);
                decimal difference = finalAmount - totalCustom;
                currentInvoice.Payments[currentInvoice.Payments.Count - 1].Amount += difference;
            }
            
            UpdateCustomSplitDisplay();
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
            bool isSplitting = IsSplittingBill();
            
            nudNumberOfSplits.Visible = isSplitting;
            lblNumberOfSplits.Visible = isSplitting;
            chkCustomSplit.Visible = isSplitting;
            
            // Show the split amount if splitting
            if (isSplitting)
            {
                UpdateSplitAmount();
            }
            else
            {
                // Hide custom split controls when not splitting
                chkCustomSplit.Checked = false;
                btnConfigureCustomSplit.Visible = false;
                lblCustomSplitStatus.Visible = false;
                lblSplitAmount.Visible = false;
                currentInvoice.Payments.Clear();
            }
        }

        private void nudNumberOfSplits_ValueChanged(object sender, EventArgs e)
        {
            // If custom split is enabled, reinitialize custom amounts
            if (IsCustomSplit())
            {
                ReinitializeCustomSplitAmounts();
            }
            else
            {
                UpdateSplitAmount();
            }
        }

        private void UpdateSplitAmount()
        {
            if (IsCustomSplit())
            {
                UpdateCustomSplitDisplay();
            }
            else
            {
                decimal finalAmount = GetFinalAmount();
                decimal splitAmount = decimal.Round(finalAmount / GetNumberOfSplits(), 2);
                lblSplitAmount.Text = $"Each person pays: €{splitAmount:0.00}";
                lblSplitAmount.Visible = true;
            }
        }

        private void btnProcessPayment_Click(object sender, EventArgs e)
        {
            try
            {
                if (IsSplittingBill())
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
            // Get payment details from form
            dynamic selectedPaymentMethod = cmbPaymentMethod.SelectedItem;
            PaymentMethod paymentMethod = (PaymentMethod)selectedPaymentMethod.Value;
            
            dynamic selectedFeedbackType = cmbFeedbackType.SelectedItem;
            FeedbackType feedbackType = (FeedbackType)selectedFeedbackType.Value;
            string feedback = txtFeedback.Text;
            
            decimal finalAmount = GetFinalAmount();
            
            // Confirm payment
            DialogResult result = MessageBox.Show(
                $"Process payment of €{finalAmount:0.00} using {paymentMethod}?",

                "Confirm Payment",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);
            
            if (result == DialogResult.Yes)
            {
                // Create the payment and add it to the invoice
                Payment payment = new Payment
                {
                    PaymentMethod = paymentMethod,
                    Amount = finalAmount,
                    Feedback = feedback,
                    FeedbackType = feedbackType
                };
                
                currentInvoice.AddPayment(payment);
                
                // Create invoice in database
                int invoiceId = paymentService.CreateInvoice(
                    orderId, 
                    currentInvoice.TotalAmount, 
                    currentInvoice.TotalVat, 
                    currentInvoice.LowVatAmount, 
                    currentInvoice.HighVatAmount, 
                    currentInvoice.TotalExcludingVat, 
                    currentInvoice.TotalTipAmount);
                
                // Update the invoice ID
                currentInvoice.InvoiceId = invoiceId;
                payment.InvoiceId = currentInvoice;
                
                // Process the payment
                int paymentId = paymentService.ProcessPayment(
                    invoiceId, 
                    payment.PaymentMethod, 
                    payment.Amount, 
                    payment.Feedback, 
                    payment.FeedbackType);
                
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
            // Validate custom split amounts if using custom split
            if (IsCustomSplit())
            {
                if (currentInvoice.Payments.Count != GetNumberOfSplits())
                {
                    MessageBox.Show(
                        "Please configure the custom split amounts before proceeding.",
                        "Custom Split Not Configured",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    return;
                }
                
                decimal totalCustom = currentInvoice.Payments.Sum(p => p.Amount);
                decimal finalAmount = GetFinalAmount();
                if (Math.Abs(totalCustom - finalAmount) >= 0.01m)
                {
                    MessageBox.Show(
                        $"The sum of custom amounts (€{totalCustom:0.00}) does not match the total bill (€{finalAmount:0.00}).\n\nPlease reconfigure the amounts.",
                        "Amount Mismatch",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    return;
                }
            }
            
            // Create invoice in database first
            int invoiceId = paymentService.CreateInvoice(
                orderId, 
                currentInvoice.TotalAmount, 
                currentInvoice.TotalVat, 
                currentInvoice.LowVatAmount, 
                currentInvoice.HighVatAmount, 
                currentInvoice.TotalExcludingVat, 
                currentInvoice.TotalTipAmount);
            
            // Update the invoice ID
            currentInvoice.InvoiceId = invoiceId;
            
            // Reset split tracking
            currentSplitIndex = 0;
            
            // Show split payment dialog for each split
            if (IsCustomSplit())
            {
                ProcessNextCustomSplitPayment(invoiceId);
            }
            else
            {
                ProcessNextEqualSplitPayment(invoiceId);
            }
        }

        private void ProcessNextEqualSplitPayment(int invoiceId)
        {
            currentSplitIndex++;
            
            if (currentSplitIndex > GetNumberOfSplits())
            {
                FinalizeSplitPayment();
                return;
            }
            
            // Calculate split amounts
            decimal finalAmount = GetFinalAmount();
            decimal splitAmount = decimal.Round(finalAmount / GetNumberOfSplits(), 2);
            decimal lastSplitAmount = finalAmount - (splitAmount * (GetNumberOfSplits() - 1));
            decimal amountToPay = (currentSplitIndex == GetNumberOfSplits()) ? lastSplitAmount : splitAmount;

            ShowSplitPaymentDialog(invoiceId, amountToPay, () => ProcessNextEqualSplitPayment(invoiceId));
        }

        private void ProcessNextCustomSplitPayment(int invoiceId)
        {
            currentSplitIndex++;
            
            if (currentSplitIndex > GetNumberOfSplits())
            {
                FinalizeSplitPayment();
                return;
            }
            
            decimal amountToPay = currentInvoice.Payments[currentSplitIndex - 1].Amount;
            ShowSplitPaymentDialog(invoiceId, amountToPay, () => ProcessNextCustomSplitPayment(invoiceId));
        }

        private void ShowSplitPaymentDialog(int invoiceId, decimal amountToPay, Action nextAction)
        {
            using (Form paymentDialog = new Form())
            {
                paymentDialog.Text = $"Payment {currentSplitIndex} of {GetNumberOfSplits()}";
                paymentDialog.Size = new Size(400, 350);
                paymentDialog.StartPosition = FormStartPosition.CenterParent;
                paymentDialog.FormBorderStyle = FormBorderStyle.FixedDialog;
                paymentDialog.MaximizeBox = false;
                paymentDialog.MinimizeBox = false;
                
                // Create dialog controls
                var controls = CreateSplitPaymentDialogControls(amountToPay);
                
                foreach (Control control in controls)
                {
                    paymentDialog.Controls.Add(control);
                }
                
                if (paymentDialog.ShowDialog() == DialogResult.OK)
                {
                    // Get values from dialog
                    var cmbMethod = paymentDialog.Controls.OfType<ComboBox>().First(c => c.Name == "cmbMethod");
                    var cmbFeedbackType = paymentDialog.Controls.OfType<ComboBox>().First(c => c.Name == "cmbFeedbackType");
                    var txtSplitFeedback = paymentDialog.Controls.OfType<TextBox>().First(c => c.Name == "txtSplitFeedback");
                    
                    dynamic selectedMethod = cmbMethod.SelectedItem;
                    PaymentMethod paymentMethod = (PaymentMethod)selectedMethod.Value;
                    
                    dynamic selectedFeedbackType = cmbFeedbackType.SelectedItem;
                    FeedbackType splitFeedbackType = (FeedbackType)selectedFeedbackType.Value;
                    string splitFeedback = txtSplitFeedback.Text;
                    
                    // Process the payment
                    bool isLastPayment = currentSplitIndex == GetNumberOfSplits();
                    int paymentId = paymentService.ProcessPayment(
                        invoiceId, 
                        paymentMethod, 
                        amountToPay, 
                        splitFeedback, 
                        splitFeedbackType,
                        isLastPayment); // Mark order as done only on last payment
                    
                    // Update the payment in the invoice's payment collection
                    Payment splitPayment = new Payment
                    {
                        PaymentId = paymentId,
                        PaymentMethod = paymentMethod,
                        Amount = amountToPay,
                        Feedback = splitFeedback,
                        FeedbackType = splitFeedbackType,
                        InvoiceId = currentInvoice
                    };
                    
                    // Update the payment in the collection
                    if (IsCustomSplit() && currentSplitIndex <= currentInvoice.Payments.Count)
                    {
                        // Update existing payment in custom split
                        currentInvoice.Payments[currentSplitIndex - 1] = splitPayment;
                    }
                    else
                    {
                        // Add new payment for equal split
                        currentInvoice.AddPayment(splitPayment);
                    }
                    
                    // Process the next split
                    nextAction();
                }
                else
                {
                    // User canceled
                    MessageBox.Show("Payment process was canceled.", 
                        "Payment Canceled", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.DialogResult = DialogResult.Cancel;
                    this.Close();
                }
            }
        }

        private List<Control> CreateSplitPaymentDialogControls(decimal amountToPay)
        {
            var controls = new List<Control>();
            
            // Amount label
            var lblAmount = new Label
            {
                Text = $"Amount: €{amountToPay:0.00}",
                Location = new Point(20, 20),
                Size = new Size(200, 25),
                Font = new Font("Segoe UI", 11, FontStyle.Bold)
            };
            controls.Add(lblAmount);
            
            // Payment method
            var lblMethod = new Label
            {
                Text = "Payment Method:",
                Location = new Point(20, 60),
                Size = new Size(150, 25)
            };
            controls.Add(lblMethod);
            
            var cmbMethod = new ComboBox
            {
                Name = "cmbMethod",
                Location = new Point(170, 60),
                Size = new Size(200, 25),
                DropDownStyle = ComboBoxStyle.DropDownList,
                DisplayMember = "Text",
                ValueMember = "Value"
            };
            cmbMethod.Items.Add(new { Text = "Cash", Value = PaymentMethod.Cash });
            cmbMethod.Items.Add(new { Text = "Debit Card", Value = PaymentMethod.DebitCard });
            cmbMethod.Items.Add(new { Text = "Credit Card", Value = PaymentMethod.CreditCard });
            cmbMethod.SelectedIndex = 0;
            controls.Add(cmbMethod);
            
            // Feedback type
            var lblFeedbackType = new Label
            {
                Text = "Feedback Type:",
                Location = new Point(20, 100),
                Size = new Size(150, 25)
            };
            controls.Add(lblFeedbackType);
            
            var cmbFeedbackType = new ComboBox
            {
                Name = "cmbFeedbackType",
                Location = new Point(170, 100),
                Size = new Size(200, 25),
                DropDownStyle = ComboBoxStyle.DropDownList,
                DisplayMember = "Text",
                ValueMember = "Value"
            };
            cmbFeedbackType.Items.Add(new { Text = "None", Value = FeedbackType.None });
            cmbFeedbackType.Items.Add(new { Text = "Comment", Value = FeedbackType.Comment });
            cmbFeedbackType.Items.Add(new { Text = "Complaint", Value = FeedbackType.Complaint });
            cmbFeedbackType.Items.Add(new { Text = "Recommendation", Value = FeedbackType.Recommendation });
            cmbFeedbackType.SelectedIndex = 0;
            controls.Add(cmbFeedbackType);
            
            // Feedback text
            var lblSplitFeedback = new Label
            {
                Text = "Feedback:",
                Location = new Point(20, 140),
                Size = new Size(150, 25)
            };
            controls.Add(lblSplitFeedback);
            
            var txtSplitFeedback = new TextBox
            {
                Name = "txtSplitFeedback",
                Location = new Point(20, 170),
                Size = new Size(350, 80),
                Multiline = true,
                Enabled = false
            };
            controls.Add(txtSplitFeedback);
            
            // Enable/disable feedback text field based on selected feedback type
            cmbFeedbackType.SelectedIndexChanged += (s, e) => {
                dynamic selectedItem = cmbFeedbackType.SelectedItem;
                FeedbackType ft = (FeedbackType)selectedItem.Value;
                txtSplitFeedback.Enabled = ft != FeedbackType.None;
                if (ft == FeedbackType.None)
                    txtSplitFeedback.Text = "";
            };
            
            // Buttons
            var btnPay = new Button
            {
                Text = "Pay",
                DialogResult = DialogResult.OK,
                Location = new Point(200, 270),
                Size = new Size(80, 30)
            };
            controls.Add(btnPay);
            
            var btnCancel = new Button
            {
                Text = "Cancel",
                DialogResult = DialogResult.Cancel,
                Location = new Point(290, 270),
                Size = new Size(80, 30)
            };
            controls.Add(btnCancel);
            
            return controls;
        }

        private void FinalizeSplitPayment()
        {
            // Show a summary of the payments using the invoice's payment collection
            StringBuilder summary = new StringBuilder();
            summary.AppendLine("Split Bill Payment Summary:");
            summary.AppendLine("---------------------------");
            
            for (int i = 0; i < currentInvoice.Payments.Count; i++)
            {
                var payment = currentInvoice.Payments[i];
                summary.AppendLine($"Payment {i+1}: {payment.PaymentMethod} - €{payment.Amount:0.00}");
                
                // Add feedback information
                if (payment.FeedbackType != FeedbackType.None && !string.IsNullOrEmpty(payment.Feedback))
                {
                    summary.AppendLine($"  Feedback ({payment.FeedbackType}): {payment.Feedback}");
                }
            }
            
            summary.AppendLine($"\nTotal Paid: €{currentInvoice.GetTotalPaidAmount():0.00}");
            summary.AppendLine($"Invoice Total: €{GetFinalAmount():0.00}");
            summary.AppendLine($"Status: {(currentInvoice.IsFullyPaid() ? "FULLY PAID" : "PARTIALLY PAID")}");
            
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
            bool isCustom = IsCustomSplit();
            
            btnConfigureCustomSplit.Visible = isCustom;
            lblCustomSplitStatus.Visible = isCustom;
            
            if (isCustom)
            {
                ReinitializeCustomSplitAmounts();
                lblSplitAmount.Visible = false; // Hide equal split display
            }
            else
            {
                currentInvoice.Payments.Clear();
                UpdateSplitAmount(); // Show equal split display
            }
        }

        private void btnConfigureCustomSplit_Click(object sender, EventArgs e)
        {
            ShowCustomSplitDialog();
        }

        private void UpdateCustomSplitDisplay()
        {
            if (currentInvoice.Payments.Count > 0)
            {
                decimal totalCustom = currentInvoice.Payments.Sum(p => p.Amount);
                decimal finalAmount = GetFinalAmount();
                
                if (Math.Abs(totalCustom - finalAmount) < 0.01m) // Account for rounding differences
                {
                    lblCustomSplitStatus.Text = $"Custom amounts configured (Total: €{totalCustom:0.00})";
                    lblCustomSplitStatus.ForeColor = Color.DarkGreen;
                }
                else
                {
                    lblCustomSplitStatus.Text = $"Custom amounts mismatch! Expected: €{finalAmount:0.00}, Current: €{totalCustom:0.00}";
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
                
                decimal finalAmount = GetFinalAmount();
                
                // Header label
                Label lblHeader = new Label();
                lblHeader.Text = $"Total Amount to Split: €{finalAmount:0.00}";
                lblHeader.Location = new Point(20, 20);
                lblHeader.Size = new Size(450, 25);
                lblHeader.Font = new Font("Segoe UI", 11, FontStyle.Bold);
                lblHeader.TextAlign = ContentAlignment.MiddleCenter;
                
                // Create text boxes for each split amount
                List<TextBox> amountTextBoxes = new List<TextBox>();
                int yPosition = 60;
                
                for (int i = 0; i < GetNumberOfSplits(); i++)
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
                    txtAmount.Text = currentInvoice.Payments.Count > i ? currentInvoice.Payments[i].Amount.ToString("0.00") : "0.00";
                    
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
                    
                    if (Math.Abs(total - finalAmount) < 0.01m)
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
                    decimal evenAmount = decimal.Round(finalAmount / GetNumberOfSplits(), 2);
                    for (int i = 0; i < amountTextBoxes.Count; i++)
                    {
                        amountTextBoxes[i].Text = evenAmount.ToString("0.00");
                    }
                    // Adjust last amount for rounding
                    if (amountTextBoxes.Count > 0)
                    {
                        decimal totalEven = evenAmount * GetNumberOfSplits();
                        decimal difference = finalAmount - totalEven;
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
                    // Save the custom amounts to the invoice's payment collection
                    currentInvoice.Payments.Clear();
                    decimal totalEntered = 0;
                    
                    foreach (TextBox tb in amountTextBoxes)
                    {
                        decimal amount = 0;
                        if (decimal.TryParse(tb.Text, out amount))
                        {
                            totalEntered += amount;
                        }
                        
                        Payment splitPayment = new Payment
                        {
                            Amount = amount,
                            InvoiceId = currentInvoice
                        };
                        currentInvoice.AddPayment(splitPayment);
                    }
                    
                    // Validate total
                    if (Math.Abs(totalEntered - finalAmount) >= 0.01m)
                    {
                        MessageBox.Show(
                            $"The sum of individual amounts (€{totalEntered:0.00}) does not match the total bill amount (€{finalAmount:0.00}).\n\nPlease adjust the amounts so they add up correctly.",
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