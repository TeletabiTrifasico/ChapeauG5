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
        private readonly OrderService orderService; // Add OrderService
        
        // Main payment object that contains all payment-related data
        private Payment currentPayment;
        private Order currentOrder;
        
        // Split payment tracking
        private List<Payment> splitPayments;
        private int currentSplitIndex = 0;

        public PaymentForm(int orderId, Employee employee)
        {
            InitializeComponent();
            this.orderId = orderId;
            this.loggedInEmployee = employee;
            this.paymentService = new PaymentService();
            this.orderService = new OrderService(); // Initialize OrderService
            
            // Initialize the main payment object
            this.currentPayment = new Payment();
            this.splitPayments = new List<Payment>();
        }

        private void PaymentForm_Load(object sender, EventArgs e)
        {
            try
            {                
                // Load the order with all items using OrderService instead of PaymentService
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
                
                // Initialize payment with order totals
                InitializePaymentData();
                
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

        private void InitializePaymentData()
        {
            // Calculate order totals and store in the payment's invoice
            var totals = paymentService.CalculateOrderTotals(currentOrder.OrderItems);
            
            // Create invoice object with calculated totals
            currentPayment.InvoiceId = new Invoice
            {
                OrderId = currentOrder,
                TotalAmount = totals.totalWithVat,
                TotalVat = totals.lowVat + totals.highVat,
                LowVatAmount = totals.lowVat,
                HighVatAmount = totals.highVat,
                TotalExcludingVat = totals.totalExVat,
                TotalTipAmount = 0, // Will be updated when tip is entered
                CreatedAt = DateTime.Now
            };
            
            // Set initial payment amount (will be updated with tip)
            currentPayment.Amount = totals.totalWithVat;
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
            var invoice = currentPayment.InvoiceId;
            
            // Update display labels
            lblSubtotalValue.Text = $"€{invoice.TotalExcludingVat:0.00}";
            lblLowVatValue.Text = $"€{invoice.LowVatAmount:0.00}";
            lblHighVatValue.Text = $"€{invoice.HighVatAmount:0.00}";
            lblTotalValue.Text = $"€{invoice.TotalAmount:0.00}";
            
            // Calculate and display final amount with tip
            UpdateFinalAmount();
        }
        
        private void UpdateFinalAmount()
        {
            // Get tip amount from input
            if (decimal.TryParse(txtTip.Text, out decimal tip))
            {
                currentPayment.InvoiceId.TotalTipAmount = tip;
            }
            else
            {
                currentPayment.InvoiceId.TotalTipAmount = 0;
            }
            
            // Update payment amount (total + tip)
            currentPayment.Amount = currentPayment.InvoiceId.TotalAmount + currentPayment.InvoiceId.TotalTipAmount;
            
            // Update display
            lblFinalAmountValue.Text = $"€{currentPayment.Amount:0.00}";
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
            // Clear existing split payments and reinitialize with even amounts
            splitPayments.Clear();
            decimal evenSplit = decimal.Round(currentPayment.Amount / GetNumberOfSplits(), 2);
            
            for (int i = 0; i < GetNumberOfSplits(); i++)
            {
                splitPayments.Add(new Payment
                {
                    Amount = evenSplit,
                    InvoiceId = currentPayment.InvoiceId
                });
            }
            
            // Adjust last amount to account for rounding
            if (splitPayments.Count > 0)
            {
                decimal totalCustom = splitPayments.Sum(p => p.Amount);
                decimal difference = currentPayment.Amount - totalCustom;
                splitPayments[splitPayments.Count - 1].Amount += difference;
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
                splitPayments.Clear();
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
                decimal splitAmount = decimal.Round(currentPayment.Amount / GetNumberOfSplits(), 2);
                lblSplitAmount.Text = $"Each person pays: €{splitAmount:0.00}";
                lblSplitAmount.Visible = true;
            }
        }

        private void btnProcessPayment_Click(object sender, EventArgs e)
        {
            try
            {
                // Set payment method and feedback from form
                dynamic selectedPaymentMethod = cmbPaymentMethod.SelectedItem;
                currentPayment.PaymentMethod = (PaymentMethod)selectedPaymentMethod.Value;
                
                dynamic selectedFeedbackType = cmbFeedbackType.SelectedItem;
                currentPayment.FeedbackType = (FeedbackType)selectedFeedbackType.Value;
                currentPayment.Feedback = txtFeedback.Text;

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
            // Confirm payment
            DialogResult result = MessageBox.Show(
                $"Process payment of {lblFinalAmountValue.Text} using {currentPayment.PaymentMethod}?",
                "Confirm Payment",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);
            
            if (result == DialogResult.Yes)
            {
                // Create invoice
                int invoiceId = paymentService.CreateInvoice(
                    orderId, 
                    currentPayment.InvoiceId.TotalAmount, 
                    currentPayment.InvoiceId.TotalVat, 
                    currentPayment.InvoiceId.LowVatAmount, 
                    currentPayment.InvoiceId.HighVatAmount, 
                    currentPayment.InvoiceId.TotalExcludingVat, 
                    currentPayment.InvoiceId.TotalTipAmount);
                
                // Process payment
                int paymentId = paymentService.ProcessPayment(
                    invoiceId, 
                    currentPayment.PaymentMethod, 
                    currentPayment.Amount, 
                    currentPayment.Feedback, 
                    currentPayment.FeedbackType);
                
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
                if (splitPayments.Count != GetNumberOfSplits())
                {
                    MessageBox.Show(
                        "Please configure the custom split amounts before proceeding.",
                        "Custom Split Not Configured",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    return;
                }
                
                decimal totalCustom = splitPayments.Sum(p => p.Amount);
                if (Math.Abs(totalCustom - currentPayment.Amount) >= 0.01m)
                {
                    MessageBox.Show(
                        $"The sum of custom amounts (€{totalCustom:0.00}) does not match the total bill (€{currentPayment.Amount:0.00}).\n\nPlease reconfigure the amounts.",
                        "Amount Mismatch",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    return;
                }
            }
            
            // Create invoice
            int invoiceId = paymentService.CreateInvoice(
                orderId, 
                currentPayment.InvoiceId.TotalAmount, 
                currentPayment.InvoiceId.TotalVat, 
                currentPayment.InvoiceId.LowVatAmount, 
                currentPayment.InvoiceId.HighVatAmount, 
                currentPayment.InvoiceId.TotalExcludingVat, 
                currentPayment.InvoiceId.TotalTipAmount);
            
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
            decimal splitAmount = decimal.Round(currentPayment.Amount / GetNumberOfSplits(), 2);
            decimal lastSplitAmount = currentPayment.Amount - (splitAmount * (GetNumberOfSplits() - 1));
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
            
            decimal amountToPay = splitPayments[currentSplitIndex - 1].Amount;
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
                    
                    // Create a payment record for tracking
                    Payment splitPayment = new Payment
                    {
                        PaymentMethod = paymentMethod,
                        Amount = amountToPay,
                        Feedback = splitFeedback,
                        FeedbackType = splitFeedbackType
                    };
                    
                    // Add to tracking list (reuse splitPayments for tracking completed payments)
                    if (splitPayments.Count < currentSplitIndex)
                    {
                        splitPayments.Add(splitPayment);
                    }
                    else
                    {
                        splitPayments[currentSplitIndex - 1] = splitPayment;
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
            // Show a summary of the payments
            StringBuilder summary = new StringBuilder();
            summary.AppendLine("Split Bill Payment Summary:");
            summary.AppendLine("---------------------------");
            
            for (int i = 0; i < splitPayments.Count; i++)
            {
                summary.AppendLine($"Payment {i+1}: {splitPayments[i].PaymentMethod} - €{splitPayments[i].Amount:0.00}");
                
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
                splitPayments.Clear();
                UpdateSplitAmount(); // Show equal split display
            }
        }

        private void btnConfigureCustomSplit_Click(object sender, EventArgs e)
        {
            ShowCustomSplitDialog();
        }

        private void UpdateCustomSplitDisplay()
        {
            if (splitPayments.Count > 0)
            {
                decimal totalCustom = splitPayments.Sum(p => p.Amount);
                
                if (Math.Abs(totalCustom - currentPayment.Amount) < 0.01m) // Account for rounding differences
                {
                    lblCustomSplitStatus.Text = $"Custom amounts configured (Total: €{totalCustom:0.00})";
                    lblCustomSplitStatus.ForeColor = Color.DarkGreen;
                }
                else
                {
                    lblCustomSplitStatus.Text = $"Custom amounts mismatch! Expected: €{currentPayment.Amount:0.00}, Current: €{totalCustom:0.00}";
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
                lblHeader.Text = $"Total Amount to Split: €{currentPayment.Amount:0.00}";
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
                    txtAmount.Text = splitPayments.Count > i ? splitPayments[i].Amount.ToString("0.00") : "0.00";
                    
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
                    
                    if (Math.Abs(total - currentPayment.Amount) < 0.01m)
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
                    decimal evenAmount = decimal.Round(currentPayment.Amount / GetNumberOfSplits(), 2);
                    for (int i = 0; i < amountTextBoxes.Count; i++)
                    {
                        amountTextBoxes[i].Text = evenAmount.ToString("0.00");
                    }
                    // Adjust last amount for rounding
                    if (amountTextBoxes.Count > 0)
                    {
                        decimal totalEven = evenAmount * GetNumberOfSplits();
                        decimal difference = currentPayment.Amount - totalEven;
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
                    splitPayments.Clear();
                    decimal totalEntered = 0;
                    
                    foreach (TextBox tb in amountTextBoxes)
                    {
                        decimal amount = 0;
                        if (decimal.TryParse(tb.Text, out amount))
                        {
                            totalEntered += amount;
                        }
                        
                        splitPayments.Add(new Payment
                        {
                            Amount = amount,
                            InvoiceId = currentPayment.InvoiceId
                        });
                    }
                    
                    // Validate total
                    if (Math.Abs(totalEntered - currentPayment.Amount) >= 0.01m)
                    {
                        MessageBox.Show(
                            $"The sum of individual amounts (€{totalEntered:0.00}) does not match the total bill amount (€{currentPayment.Amount:0.00}).\n\nPlease adjust the amounts so they add up correctly.",
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