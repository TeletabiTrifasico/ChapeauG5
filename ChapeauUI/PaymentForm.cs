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
        private readonly TableService tableService;
        
        // Main invoice object that contains all payment-related data including payments list
        private Invoice currentInvoice;
        private Order currentOrder;

        public PaymentForm(int orderId, Employee employee)
        {
            InitializeComponent();
            this.orderId = orderId;
            this.loggedInEmployee = employee;
            this.paymentService = new PaymentService();
            this.orderService = new OrderService();
            this.tableService = new TableService();
            this.currentInvoice = new Invoice();
        }

        private void PaymentForm_Load(object sender, EventArgs e)
        {
            try
            {
                if (!LoadOrderData()) return;
                
                InitializeFormData();
                SetupFormControls();
            }
            catch (Exception ex)
            {
                HandleLoadError(ex);
            }
        }

        private bool LoadOrderData()
        {
            try
            {
                currentOrder = orderService.GetOrderWithItemsById(orderId);
                
                if (!ValidateOrderExists()) return false;
                if (!ValidateOrderHasItems()) return false;
                
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading order data: {ex.Message}");
                ShowErrorAndClose("Database Error", $"Failed to load order information: {ex.Message}");
                return false;
            }
        }

        private bool ValidateOrderExists()
        {
            if (currentOrder == null)
            {
                Console.WriteLine($"Error: Order #{orderId} not found in database");
                ShowErrorAndClose("Order Not Found", $"Order #{orderId} not found in the database.");
                return false;
            }
            return true;
        }

        private bool ValidateOrderHasItems()
        {
            if (currentOrder.OrderItems == null || currentOrder.OrderItems.Count == 0)
            {
                Console.WriteLine($"Error: Order #{orderId} has no items");
                ShowErrorAndClose("Empty Order", "No items found in this order.");
                return false;
            }
            return true;
        }

        private void ShowErrorAndClose(string title, string message)
        {
            MessageBox.Show(message, title, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            this.Close();
        }

        private void InitializeFormData()
        {
            SetHeaderText();
            InitializeInvoiceData();
            LoadOrderItems();
            UpdateTotalsDisplay();
        }

        private void SetHeaderText()
        {
            lblHeader.Text = $"Payment for Table {currentOrder.TableId.TableNumber}";
        }

        private void SetupFormControls()
        {
            SetupPaymentMethodComboBox();
            SetupFeedbackTypeComboBox();
        }

        private void HandleLoadError(Exception ex)
        {
            Console.WriteLine($"Critical error in PaymentForm_Load: {ex.Message}");
            Console.WriteLine($"Stack trace: {ex.StackTrace}");
            MessageBox.Show($"Error loading payment information: {ex.Message}", 
                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            this.Close();
        }

        private void InitializeInvoiceData()
        {
            try
            {
                // Set basic invoice info first
                currentInvoice.OrderId = currentOrder;
                currentInvoice.TotalTipAmount = 0;
                currentInvoice.CreatedAt = DateTime.Now;
                
                // Calculate totals and populate the invoice directly
                paymentService.CalculateOrderTotals(currentOrder.OrderItems, currentInvoice);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error calculating order totals: {ex.Message}");
                MessageBox.Show($"Error calculating order totals: {ex.Message}", 
                    "Calculation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                throw; // Re-throw to be caught by parent method
            }
        }

        // Remove SetInvoiceBasicInfo and SetInvoiceAmounts methods - no longer needed

        private void LoadOrderItems()
        {
            lvOrderItems.Items.Clear();
            
            foreach (OrderItem item in currentOrder.OrderItems)
            {
                AddOrderItemToListView(item);
            }
        }

        private void AddOrderItemToListView(OrderItem item)
        {
            string vatRate = GetVatRateDisplay(item.MenuItemId.VatPercentage);
            decimal subtotal = CalculateItemSubtotal(item);
            
            ListViewItem lvi = CreateListViewItem(item, vatRate, subtotal);
            lvOrderItems.Items.Add(lvi);
        }

        private string GetVatRateDisplay(int vatPercentage)
        {
            return vatPercentage == 21 ? "21%" : "9%";
        }

        private decimal CalculateItemSubtotal(OrderItem item)
        {
            return item.Quantity * item.MenuItemId.Price;
        }

        private ListViewItem CreateListViewItem(OrderItem item, string vatRate, decimal subtotal)
        {
            ListViewItem lvi = new ListViewItem(item.MenuItemId.Name);
            lvi.SubItems.Add(item.Quantity.ToString());
            lvi.SubItems.Add($"€{item.MenuItemId.Price:0.00}");
            lvi.SubItems.Add(vatRate);
            lvi.SubItems.Add($"€{subtotal:0.00}");
            lvi.Tag = item;
            return lvi;
        }

        private void UpdateTotalsDisplay()
        {
            SetTotalLabels();
            UpdateFinalAmount();
        }

        private void SetTotalLabels()
        {
            lblSubtotalValue.Text = $"€{currentInvoice.TotalExcludingVat:0.00}";
            lblLowVatValue.Text = $"€{currentInvoice.LowVatAmount:0.00}";
            lblHighVatValue.Text = $"€{currentInvoice.HighVatAmount:0.00}";
            lblTotalValue.Text = $"€{currentInvoice.TotalAmount:0.00}";
        }
        
        private void UpdateFinalAmount()
        {
            UpdateTipAmount();
            CalculateAndDisplayFinalAmount();
        }

        private void UpdateTipAmount()
        {
            if (decimal.TryParse(txtTip.Text, out decimal tip))
            {
                currentInvoice.TotalTipAmount = tip;
            }
            else
            {
                currentInvoice.TotalTipAmount = 0;
            }
        }

        private void CalculateAndDisplayFinalAmount()
        {
            decimal finalAmount = GetFinalAmount();
            lblFinalAmountValue.Text = $"€{finalAmount:0.00}";
        }

        private decimal GetFinalAmount()
        {
            return currentInvoice.TotalAmount + currentInvoice.TotalTipAmount;
        }

        private void SetupPaymentMethodComboBox()
        {
            InitializePaymentMethodCombo();
            AddPaymentMethodItems();
            SetDefaultPaymentMethod();
        }

        private void InitializePaymentMethodCombo()
        {
            cmbPaymentMethod.Items.Clear();
            cmbPaymentMethod.DisplayMember = "Text";
            cmbPaymentMethod.ValueMember = "Value";
        }

        private void AddPaymentMethodItems()
        {
            cmbPaymentMethod.Items.Add(new { Text = "Cash", Value = PaymentMethod.Cash });
            cmbPaymentMethod.Items.Add(new { Text = "Debit Card", Value = PaymentMethod.DebitCard });
            cmbPaymentMethod.Items.Add(new { Text = "Credit Card", Value = PaymentMethod.CreditCard });
        }

        private void SetDefaultPaymentMethod()
        {
            cmbPaymentMethod.SelectedIndex = 0;
        }
        
        private void SetupFeedbackTypeComboBox()
        {
            InitializeFeedbackTypeCombo();
            AddFeedbackTypeItems();
            SetDefaultFeedbackType();
        }

        private void InitializeFeedbackTypeCombo()
        {
            cmbFeedbackType.Items.Clear();
            cmbFeedbackType.DisplayMember = "Text";
            cmbFeedbackType.ValueMember = "Value";
        }

        private void AddFeedbackTypeItems()
        {
            cmbFeedbackType.Items.Add(new { Text = "None", Value = FeedbackType.None });
            cmbFeedbackType.Items.Add(new { Text = "Comment", Value = FeedbackType.Comment });
            cmbFeedbackType.Items.Add(new { Text = "Complaint", Value = FeedbackType.Complaint });
            cmbFeedbackType.Items.Add(new { Text = "Recommendation", Value = FeedbackType.Recommendation });
        }

        private void SetDefaultFeedbackType()
        {
            cmbFeedbackType.SelectedIndex = 0;
        }

        private void txtTip_TextChanged(object sender, EventArgs e)
        {
            UpdateFinalAmount();
            HandleTipChangeForCustomSplit();
        }

        private void HandleTipChangeForCustomSplit()
        {
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
            ClearExistingPayments();
            CreateEvenSplitPayments();
            AdjustForRounding();
            UpdateCustomSplitDisplay();
        }

        private void ClearExistingPayments()
        {
            currentInvoice.Payments.Clear();
        }

        private void CreateEvenSplitPayments()
        {
            decimal finalAmount = GetFinalAmount();
            decimal evenSplit = decimal.Round(finalAmount / GetNumberOfSplits(), 2);
            
            for (int i = 0; i < GetNumberOfSplits(); i++)
            {
                AddSplitPayment(evenSplit);
            }
        }

        private void AddSplitPayment(decimal amount)
        {
            Payment splitPayment = new Payment
            {
                Amount = amount,
                Invoice = currentInvoice
            };
            currentInvoice.Payments.Add(splitPayment);
        }

        private void AdjustForRounding()
        {
            if (currentInvoice.Payments.Count > 0)
            {
                decimal totalCustom = currentInvoice.Payments.Sum(p => p.Amount);
                decimal difference = GetFinalAmount() - totalCustom;
                currentInvoice.Payments[currentInvoice.Payments.Count - 1].Amount += difference;
            }
        }
        
        private void cmbFeedbackType_SelectedIndexChanged(object sender, EventArgs e)
        {
            dynamic selectedItem = cmbFeedbackType.SelectedItem;
            FeedbackType feedbackType = (FeedbackType)selectedItem.Value;
            
            UpdateFeedbackTextBoxState(feedbackType);
        }

        private void UpdateFeedbackTextBoxState(FeedbackType feedbackType)
        {
            txtFeedback.Enabled = feedbackType != FeedbackType.None;
            if (feedbackType == FeedbackType.None)
            {
                txtFeedback.Text = "";
            }
        }

        private void chkSplitBill_CheckedChanged(object sender, EventArgs e)
        {
            bool isSplitting = IsSplittingBill();
            
            ToggleSplitControls(isSplitting);
            HandleSplitBillChange(isSplitting);
        }

        private void ToggleSplitControls(bool isSplitting)
        {
            nudNumberOfSplits.Visible = isSplitting;
            lblNumberOfSplits.Visible = isSplitting;
            chkCustomSplit.Visible = isSplitting;
        }

        private void HandleSplitBillChange(bool isSplitting)
        {
            if (isSplitting)
            {
                UpdateSplitAmount();
            }
            else
            {
                HideCustomSplitControls();
                ClearExistingPayments();
            }
        }

        private void HideCustomSplitControls()
        {
            chkCustomSplit.Checked = false;
            btnConfigureCustomSplit.Visible = false;
            lblCustomSplitStatus.Visible = false;
            lblSplitAmount.Visible = false;
        }

        private void nudNumberOfSplits_ValueChanged(object sender, EventArgs e)
        {
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
                ShowEqualSplitAmount();
            }
        }

        private void ShowEqualSplitAmount()
        {
            decimal finalAmount = GetFinalAmount();
            decimal splitAmount = decimal.Round(finalAmount / GetNumberOfSplits(), 2);
            lblSplitAmount.Text = $"Each person pays: €{splitAmount:0.00}";
            lblSplitAmount.Visible = true;
        }

        private void btnProcessPayment_Click(object sender, EventArgs e)
        {
            try
            {
                ProcessPaymentBasedOnType();
            }
            catch (Exception ex)
            {
                ShowPaymentError(ex);
            }
        }

        private void ProcessPaymentBasedOnType()
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

        private void ShowPaymentError(Exception ex)
        {
            Console.WriteLine($"Payment processing error: {ex.Message}");
            MessageBox.Show($"Error processing payment: {ex.Message}", 
                "Payment Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void ProcessSinglePayment()
        {
            Payment payment = new Payment();
            PopulatePaymentFromForm(payment);
            
            if (ConfirmSinglePayment(payment.PaymentMethod, payment.Amount))
            {
                CompleteSinglePayment(payment);
            }
        }

        private void PopulatePaymentFromForm(Payment payment)
        {
            dynamic selectedPaymentMethod = cmbPaymentMethod.SelectedItem;
            dynamic selectedFeedbackType = cmbFeedbackType.SelectedItem;
            
            payment.PaymentMethod = (PaymentMethod)selectedPaymentMethod.Value;
            payment.FeedbackType = (FeedbackType)selectedFeedbackType.Value;
            payment.Feedback = txtFeedback.Text;
            payment.Amount = GetFinalAmount();
            payment.Invoice = currentInvoice;
        }

        private bool ConfirmSinglePayment(PaymentMethod paymentMethod, decimal finalAmount)
        {
            DialogResult result = MessageBox.Show(
                $"Process payment of €{finalAmount:0.00} using {paymentMethod}?",
                "Confirm Payment",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);
            
            return result == DialogResult.Yes;
        }

        private void CompleteSinglePayment(Payment payment)
        {
            currentInvoice.AddPayment(payment);
            
            SavePaymentToDatabase();
            ShowSuccessAndClose();
        }

        private void SavePaymentToDatabase()
        {
            try
            {
                paymentService.ProcessCompleteInvoice(currentInvoice);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Database error saving payment: {ex.Message}");
                MessageBox.Show($"Error saving payment: {ex.Message}", 
                    "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                throw;
            }
        }

        private void ShowSuccessAndClose()
        {
            MessageBox.Show(
                "Payment processed successfully!",
                "Payment Complete",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
            
            FreeTableAfterPayment();
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void FreeTableAfterPayment()
        {
            try
            {
                if (currentOrder?.TableId != null)
                {
                    tableService.UpdateTableStatus(currentOrder.TableId.TableId, TableStatus.Free);
                    Console.WriteLine($"Table {currentOrder.TableId.TableNumber} marked as free after payment");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Warning: Could not free table after payment: {ex.Message}");
            }
        }

        private void ProcessSplitPayment()
        {
            try
            {
                if (!ValidateSplitPayment()) return;
                
                var allPayments = PrepareAllSplitPayments();
                
                if (allPayments != null && allPayments.Count > 0)
                {
                    CompleteSplitPayment(allPayments);
                }
                else
                {
                    ShowPaymentCanceled();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing split payment: {ex.Message}");
                MessageBox.Show($"Error processing split payment: {ex.Message}", 
                    "Split Payment Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private bool ValidateSplitPayment()
        {
            if (IsCustomSplit())
            {
                return ValidateCustomSplitConfiguration() && ValidateCustomSplitAmounts();
            }
            return true;
        }

        private bool ValidateCustomSplitConfiguration()
        {
            if (currentInvoice.Payments.Count != GetNumberOfSplits())
            {
                MessageBox.Show(
                    "Please configure the custom split amounts before proceeding.",
                    "Custom Split Not Configured",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return false;
            }
            return true;
        }

        private bool ValidateCustomSplitAmounts()
        {
            decimal totalCustom = currentInvoice.Payments.Sum(p => p.Amount);
            decimal finalAmount = GetFinalAmount();
            
            if (Math.Abs(totalCustom - finalAmount) >= 0.01m)
            {
                ShowAmountMismatchError(totalCustom, finalAmount);
                return false;
            }
            return true;
        }

        private void ShowAmountMismatchError(decimal totalCustom, decimal finalAmount)
        {
            MessageBox.Show(
                $"The sum of custom amounts (€{totalCustom:0.00}) does not match the total bill (€{finalAmount:0.00}).\n\nPlease reconfigure the amounts.",
                "Amount Mismatch",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning);
        }

        private void CompleteSplitPayment(List<Payment> allPayments)
        {
            UpdateInvoiceWithPayments(allPayments);
            SaveSplitPaymentToDatabase();
            ShowSplitPaymentSummary();
        }

        private void UpdateInvoiceWithPayments(List<Payment> allPayments)
        {
            currentInvoice.Payments.Clear();
            foreach (Payment payment in allPayments)
            {
                currentInvoice.AddPayment(payment);
            }
        }

        private void SaveSplitPaymentToDatabase()
        {
            try
            {
                paymentService.ProcessCompleteInvoice(currentInvoice);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Database error saving split payments: {ex.Message}");
                MessageBox.Show($"Error saving split payments: {ex.Message}", 
                    "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                throw;
            }
        }

        private void ShowSplitPaymentSummary()
        {
            ShowPaymentSummary();
            FreeTableAfterPayment();
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void ShowPaymentCanceled()
        {
            MessageBox.Show("Payment process was canceled.", 
                "Payment Canceled", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private List<Payment> PrepareAllSplitPayments()
        {
            List<Payment> payments = new List<Payment>();
            int numberOfSplits = GetNumberOfSplits();
            
            int currentPaymentIndex = 0;
            while (currentPaymentIndex < numberOfSplits)
            {
                Payment splitPayment = ProcessSingleSplitPayment(currentPaymentIndex, numberOfSplits);
                
                if (splitPayment == null) return null;
                
                payments.Add(splitPayment);
                currentPaymentIndex++;
            }
            
            return payments;
        }

        private Payment ProcessSingleSplitPayment(int currentPaymentIndex, int numberOfSplits)
        {
            decimal amountToPay = GetAmountForSplit(currentPaymentIndex, numberOfSplits);
            return CollectSplitPaymentInfo(currentPaymentIndex + 1, numberOfSplits, amountToPay);
        }

        private decimal GetAmountForSplit(int currentPaymentIndex, int numberOfSplits)
        {
            if (IsCustomSplit())
            {
                return currentInvoice.Payments[currentPaymentIndex].Amount;
            }
            else
            {
                return CalculateEqualSplitAmount(currentPaymentIndex, numberOfSplits);
            }
        }

        private decimal CalculateEqualSplitAmount(int currentPaymentIndex, int numberOfSplits)
        {
            decimal finalAmount = GetFinalAmount();
            decimal splitAmount = decimal.Round(finalAmount / numberOfSplits, 2);
            decimal lastSplitAmount = finalAmount - (splitAmount * (numberOfSplits - 1));
            
            return (currentPaymentIndex == numberOfSplits - 1) ? lastSplitAmount : splitAmount;
        }

        private Payment CollectSplitPaymentInfo(int paymentNumber, int totalPayments, decimal amountToPay)
        {
            using (Form paymentDialog = CreateSplitPaymentDialog(paymentNumber, totalPayments))
            {
                AddControlsToDialog(paymentDialog, amountToPay);
                
                if (paymentDialog.ShowDialog() == DialogResult.OK)
                {
                    return ExtractPaymentFromDialog(paymentDialog, amountToPay);
                }
                else
                {
                    return null;
                }
            }
        }

        private Form CreateSplitPaymentDialog(int paymentNumber, int totalPayments)
        {
            Form paymentDialog = new Form();
            paymentDialog.Text = $"Payment {paymentNumber} of {totalPayments}";
            paymentDialog.Size = new Size(400, 350);
            paymentDialog.StartPosition = FormStartPosition.CenterParent;
            paymentDialog.FormBorderStyle = FormBorderStyle.FixedDialog;
            paymentDialog.MaximizeBox = false;
            paymentDialog.MinimizeBox = false;
            return paymentDialog;
        }

        private void AddControlsToDialog(Form paymentDialog, decimal amountToPay)
        {
            var controls = CreateSplitPaymentDialogControls(amountToPay);
            
            foreach (Control control in controls)
            {
                paymentDialog.Controls.Add(control);
            }
        }

        private Payment ExtractPaymentFromDialog(Form paymentDialog, decimal amountToPay)
        {
            Payment payment = new Payment();
            PopulatePaymentFromDialog(paymentDialog, payment, amountToPay);
            return payment;
        }

        private void PopulatePaymentFromDialog(Form paymentDialog, Payment payment, decimal amountToPay)
        {
            var dialogData = GetDialogValues(paymentDialog);
            
            payment.PaymentMethod = dialogData.paymentMethod;
            payment.FeedbackType = dialogData.feedbackType;
            payment.Feedback = dialogData.feedback;
            payment.Amount = amountToPay;
            payment.Invoice = currentInvoice;
        }

        private (PaymentMethod paymentMethod, FeedbackType feedbackType, string feedback) GetDialogValues(Form paymentDialog)
        {
            var cmbMethod = paymentDialog.Controls.OfType<ComboBox>().First(c => c.Name == "cmbMethod");
            var cmbFeedbackType = paymentDialog.Controls.OfType<ComboBox>().First(c => c.Name == "cmbFeedbackType");
            var txtSplitFeedback = paymentDialog.Controls.OfType<TextBox>().First(c => c.Name == "txtSplitFeedback");
            
            dynamic selectedMethod = cmbMethod.SelectedItem;
            dynamic selectedFeedbackType = cmbFeedbackType.SelectedItem;
            
            return (
                (PaymentMethod)selectedMethod.Value,
                (FeedbackType)selectedFeedbackType.Value,
                txtSplitFeedback.Text
            );
        }

        private List<Control> CreateSplitPaymentDialogControls(decimal amountToPay)
        {
            var controls = new List<Control>();
            
            AddAmountLabel(controls, amountToPay);
            AddPaymentMethodControls(controls);
            AddFeedbackControls(controls);
            AddDialogButtons(controls);
            
            return controls;
        }

        private void AddAmountLabel(List<Control> controls, decimal amountToPay)
        {
            var lblAmount = new Label
            {
                Text = $"Amount: €{amountToPay:0.00}",
                Location = new Point(20, 20),
                Size = new Size(200, 25),
                Font = new Font("Segoe UI", 11, FontStyle.Bold)
            };
            controls.Add(lblAmount);
        }

        private void AddPaymentMethodControls(List<Control> controls)
        {
            var lblMethod = CreateLabel("Payment Method:", new Point(20, 60));
            var cmbMethod = CreatePaymentMethodCombo();
            
            controls.Add(lblMethod);
            controls.Add(cmbMethod);
        }

        private Label CreateLabel(string text, Point location)
        {
            return new Label
            {
                Text = text,
                Location = location,
                Size = new Size(150, 25)
            };
        }

        private ComboBox CreatePaymentMethodCombo()
        {
            var cmbMethod = new ComboBox
            {
                Name = "cmbMethod",
                Location = new Point(170, 60),
                Size = new Size(200, 25),
                DropDownStyle = ComboBoxStyle.DropDownList,
                DisplayMember = "Text",
                ValueMember = "Value"
            };
            
            PopulatePaymentMethodCombo(cmbMethod);
            return cmbMethod;
        }

        private void PopulatePaymentMethodCombo(ComboBox cmbMethod)
        {
            cmbMethod.Items.Add(new { Text = "Cash", Value = PaymentMethod.Cash });
            cmbMethod.Items.Add(new { Text = "Debit Card", Value = PaymentMethod.DebitCard });
            cmbMethod.Items.Add(new { Text = "Credit Card", Value = PaymentMethod.CreditCard });
            cmbMethod.SelectedIndex = 0;
        }

        private void AddFeedbackControls(List<Control> controls)
        {
            var lblFeedbackType = CreateLabel("Feedback Type:", new Point(20, 100));
            var cmbFeedbackType = CreateFeedbackTypeCombo();
            var lblSplitFeedback = CreateLabel("Feedback:", new Point(20, 140));
            var txtSplitFeedback = CreateFeedbackTextBox();
            
            SetupFeedbackTypeEventHandler(cmbFeedbackType, txtSplitFeedback);
            
            controls.AddRange(new Control[] { lblFeedbackType, cmbFeedbackType, lblSplitFeedback, txtSplitFeedback });
        }

        private ComboBox CreateFeedbackTypeCombo()
        {
            var cmbFeedbackType = new ComboBox
            {
                Name = "cmbFeedbackType",
                Location = new Point(170, 100),
                Size = new Size(200, 25),
                DropDownStyle = ComboBoxStyle.DropDownList,
                DisplayMember = "Text",
                ValueMember = "Value"
            };
            
            PopulateFeedbackTypeCombo(cmbFeedbackType);
            return cmbFeedbackType;
        }

        private void PopulateFeedbackTypeCombo(ComboBox cmbFeedbackType)
        {
            cmbFeedbackType.Items.Add(new { Text = "None", Value = FeedbackType.None });
            cmbFeedbackType.Items.Add(new { Text = "Comment", Value = FeedbackType.Comment });
            cmbFeedbackType.Items.Add(new { Text = "Complaint", Value = FeedbackType.Complaint });
            cmbFeedbackType.Items.Add(new { Text = "Recommendation", Value = FeedbackType.Recommendation });
            cmbFeedbackType.SelectedIndex = 0;
        }

        private TextBox CreateFeedbackTextBox()
        {
            return new TextBox
            {
                Name = "txtSplitFeedback",
                Location = new Point(20, 170),
                Size = new Size(350, 80),
                Multiline = true,
                Enabled = false
            };
        }

        private void SetupFeedbackTypeEventHandler(ComboBox cmbFeedbackType, TextBox txtSplitFeedback)
        {
            cmbFeedbackType.SelectedIndexChanged += (s, e) => {
                dynamic selectedItem = cmbFeedbackType.SelectedItem;
                FeedbackType ft = (FeedbackType)selectedItem.Value;
                txtSplitFeedback.Enabled = ft != FeedbackType.None;
                if (ft == FeedbackType.None) txtSplitFeedback.Text = "";
            };
        }

        private void AddDialogButtons(List<Control> controls)
        {
            var btnPay = CreateButton("Pay", new Point(200, 270), DialogResult.OK);
            var btnCancel = CreateButton("Cancel", new Point(290, 270), DialogResult.Cancel);
            
            controls.Add(btnPay);
            controls.Add(btnCancel);
        }

        private Button CreateButton(string text, Point location, DialogResult dialogResult)
        {
            return new Button
            {
                Text = text,
                DialogResult = dialogResult,
                Location = location,
                Size = new Size(80, 30)
            };
        }

        private void ShowPaymentSummary()
        {
            StringBuilder summary = BuildPaymentSummary();
            MessageBox.Show(summary.ToString(), "Payment Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private StringBuilder BuildPaymentSummary()
        {
            StringBuilder summary = new StringBuilder();
            AddSummaryHeader(summary);
            AddPaymentDetails(summary);
            AddSummaryTotals(summary);
            return summary;
        }

        private void AddSummaryHeader(StringBuilder summary)
        {
            summary.AppendLine("Split Bill Payment Summary:");
            summary.AppendLine("---------------------------");
        }

        private void AddPaymentDetails(StringBuilder summary)
        {
            for (int i = 0; i < currentInvoice.Payments.Count; i++)
            {
                var payment = currentInvoice.Payments[i];
                summary.AppendLine($"Payment {i+1}: {payment.PaymentMethod} - €{payment.Amount:0.00}");
                
                AddFeedbackToSummary(summary, payment);
            }
        }

        private void AddFeedbackToSummary(StringBuilder summary, Payment payment)
        {
            if (payment.FeedbackType != FeedbackType.None && !string.IsNullOrEmpty(payment.Feedback))
            {
                summary.AppendLine($"  Feedback ({payment.FeedbackType}): {payment.Feedback}");
            }
        }

        private void AddSummaryTotals(StringBuilder summary)
        {
            summary.AppendLine($"\nTotal Paid: €{currentInvoice.GetTotalPaidAmount():0.00}");
            summary.AppendLine($"Invoice Total: €{GetFinalAmount():0.00}");
            summary.AppendLine($"Status: {(currentInvoice.IsFullyPaid() ? "FULLY PAID" : "PARTIALLY PAID")}");
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void chkCustomSplit_CheckedChanged(object sender, EventArgs e)
        {
            bool isCustom = IsCustomSplit();
            
            ToggleCustomSplitControls(isCustom);
            HandleCustomSplitChange(isCustom);
        }

        private void ToggleCustomSplitControls(bool isCustom)
        {
            btnConfigureCustomSplit.Visible = isCustom;
            lblCustomSplitStatus.Visible = isCustom;
        }

        private void HandleCustomSplitChange(bool isCustom)
        {
            if (isCustom)
            {
                ReinitializeCustomSplitAmounts();
                lblSplitAmount.Visible = false;
            }
            else
            {
                ClearExistingPayments();
                UpdateSplitAmount();
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
                ShowCustomSplitStatus();
            }
            else
            {
                ShowNotConfiguredStatus();
            }
        }

        private void ShowCustomSplitStatus()
        {
            decimal totalCustom = currentInvoice.Payments.Sum(p => p.Amount);
            decimal finalAmount = GetFinalAmount();
            
            if (Math.Abs(totalCustom - finalAmount) < 0.01m)
            {
                ShowValidCustomSplitStatus(totalCustom);
            }
            else
            {
                ShowInvalidCustomSplitStatus(finalAmount, totalCustom);
            }
        }

        private void ShowValidCustomSplitStatus(decimal totalCustom)
        {
            lblCustomSplitStatus.Text = $"Custom amounts configured (Total: €{totalCustom:0.00})";
            lblCustomSplitStatus.ForeColor = Color.DarkGreen;
        }

        private void ShowInvalidCustomSplitStatus(decimal finalAmount, decimal totalCustom)
        {
            lblCustomSplitStatus.Text = $"Custom amounts mismatch! Expected: €{finalAmount:0.00}, Current: €{totalCustom:0.00}";
            lblCustomSplitStatus.ForeColor = Color.Red;
        }

        private void ShowNotConfiguredStatus()
        {
            lblCustomSplitStatus.Text = "Custom amounts not configured";
            lblCustomSplitStatus.ForeColor = Color.DarkBlue;
        }

        private void ShowCustomSplitDialog()
        {
            using (Form customSplitDialog = CreateCustomSplitDialog())
            {
                SetupCustomSplitDialogContent(customSplitDialog);
                
                if (customSplitDialog.ShowDialog() == DialogResult.OK)
                {
                    ProcessCustomSplitDialogResult(customSplitDialog);
                }
            }
        }

        private Form CreateCustomSplitDialog()
        {
            Form customSplitDialog = new Form();
            customSplitDialog.Text = "Configure Custom Split Amounts";
            customSplitDialog.Size = new Size(500, 400);
            customSplitDialog.StartPosition = FormStartPosition.CenterParent;
            customSplitDialog.FormBorderStyle = FormBorderStyle.FixedDialog;
            customSplitDialog.MaximizeBox = false;
            customSplitDialog.MinimizeBox = false;
            return customSplitDialog;
        }

        private void SetupCustomSplitDialogContent(Form customSplitDialog)
        {
            decimal finalAmount = GetFinalAmount();
            
            AddDialogHeader(customSplitDialog, finalAmount);
            var amountTextBoxes = AddAmountInputs(customSplitDialog);
            var lblTotal = AddTotalDisplay(customSplitDialog, amountTextBoxes.Count);
            SetupTotalCalculation(amountTextBoxes, lblTotal, finalAmount);
            AddCustomSplitDialogButtons(customSplitDialog, amountTextBoxes, finalAmount);
        }

        private void AddDialogHeader(Form customSplitDialog, decimal finalAmount)
        {
            Label lblHeader = new Label();
            lblHeader.Text = $"Total Amount to Split: €{finalAmount:0.00}";
            lblHeader.Location = new Point(20, 20);
            lblHeader.Size = new Size(450, 25);
            lblHeader.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            lblHeader.TextAlign = ContentAlignment.MiddleCenter;
            customSplitDialog.Controls.Add(lblHeader);
        }

        private List<TextBox> AddAmountInputs(Form customSplitDialog)
        {
            List<TextBox> amountTextBoxes = new List<TextBox>();
            int yPosition = 60;
            
            for (int i = 0; i < GetNumberOfSplits(); i++)
            {
                CreatePersonAmountInput(customSplitDialog, i, yPosition, amountTextBoxes);
                yPosition += 35;
            }
            
            return amountTextBoxes;
        }

        private void CreatePersonAmountInput(Form customSplitDialog, int personIndex, int yPosition, List<TextBox> amountTextBoxes)
        {
            Label lblPerson = CreatePersonLabel(personIndex, yPosition);
            TextBox txtAmount = CreateAmountTextBox(personIndex, yPosition);
            Label lblEuro = CreateEuroLabel(yPosition);
            
            customSplitDialog.Controls.AddRange(new Control[] { lblPerson, txtAmount, lblEuro });
            amountTextBoxes.Add(txtAmount);
        }

        private Label CreatePersonLabel(int personIndex, int yPosition)
        {
            return new Label
            {
                Text = $"Person {personIndex + 1}:",
                Location = new Point(20, yPosition),
                Size = new Size(80, 25),
                TextAlign = ContentAlignment.MiddleLeft
            };
        }

        private TextBox CreateAmountTextBox(int personIndex, int yPosition)
        {
            return new TextBox
            {
                Location = new Point(110, yPosition),
                Size = new Size(100, 25),
                TextAlign = HorizontalAlignment.Right,
                Text = GetInitialAmountText(personIndex)
            };
        }

        private string GetInitialAmountText(int personIndex)
        {
            return currentInvoice.Payments.Count > personIndex 
                ? currentInvoice.Payments[personIndex].Amount.ToString("0.00") 
                : "0.00";
        }

        private Label CreateEuroLabel(int yPosition)
        {
            return new Label
            {
                Text = "€",
                Location = new Point(220, yPosition),
                Size = new Size(20, 25),
                TextAlign = ContentAlignment.MiddleLeft
            };
        }

        private Label AddTotalDisplay(Form customSplitDialog, int numberOfInputs)
        {
            int yPosition = 60 + (numberOfInputs * 35);
            
            Label lblTotal = new Label();
            lblTotal.Location = new Point(20, yPosition + 10);
            lblTotal.Size = new Size(220, 25);
            lblTotal.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            lblTotal.Text = "Total of amounts above: €0.00";
            
            customSplitDialog.Controls.Add(lblTotal);
            return lblTotal;
        }

        private void SetupTotalCalculation(List<TextBox> amountTextBoxes, Label lblTotal, decimal finalAmount)
        {
            EventHandler updateTotal = CreateUpdateTotalHandler(amountTextBoxes, lblTotal, finalAmount);
            
            foreach (TextBox tb in amountTextBoxes)
            {
                tb.TextChanged += updateTotal;
            }
            
            updateTotal(this, EventArgs.Empty);
        }

        private EventHandler CreateUpdateTotalHandler(List<TextBox> amountTextBoxes, Label lblTotal, decimal finalAmount)
        {
            return (s, e) =>
            {
                decimal total = CalculateTotalFromTextBoxes(amountTextBoxes);
                UpdateTotalLabel(lblTotal, total, finalAmount);
            };
        }

        private decimal CalculateTotalFromTextBoxes(List<TextBox> amountTextBoxes)
        {
            decimal total = 0;
            foreach (TextBox tb in amountTextBoxes)
            {
                if (decimal.TryParse(tb.Text, out decimal amount))
                {
                    total += amount;
                }
            }
            return total;
        }

        private void UpdateTotalLabel(Label lblTotal, decimal total, decimal finalAmount)
        {
            lblTotal.Text = $"Total of amounts above: €{total:0.00}";
            lblTotal.ForeColor = Math.Abs(total - finalAmount) < 0.01m ? Color.DarkGreen : Color.Red;
        }

        private void AddCustomSplitDialogButtons(Form customSplitDialog, List<TextBox> amountTextBoxes, decimal finalAmount)
        {
            int yPosition = 60 + (amountTextBoxes.Count * 35);
            
            Button btnEqualSplit = CreateEqualSplitButton(yPosition, amountTextBoxes, finalAmount);
            Button btnOK = CreateOKButton(yPosition);
            Button btnCancel = CreateCancelButton(yPosition);
            
            customSplitDialog.Controls.AddRange(new Control[] { btnEqualSplit, btnOK, btnCancel });
            customSplitDialog.AcceptButton = btnOK;
            customSplitDialog.CancelButton = btnCancel;
        }

        private Button CreateEqualSplitButton(int yPosition, List<TextBox> amountTextBoxes, decimal finalAmount)
        {
            Button btnEqualSplit = new Button();
            btnEqualSplit.Text = "Equal Split";
            btnEqualSplit.Location = new Point(250, yPosition + 10);
            btnEqualSplit.Size = new Size(100, 25);
            btnEqualSplit.BackColor = Color.LightBlue;
            btnEqualSplit.Click += (s, e) => FillEqualSplitAmounts(amountTextBoxes, finalAmount);
            return btnEqualSplit;
        }

        private void FillEqualSplitAmounts(List<TextBox> amountTextBoxes, decimal finalAmount)
        {
            decimal evenAmount = decimal.Round(finalAmount / GetNumberOfSplits(), 2);
            
            FillTextBoxesWithEvenAmount(amountTextBoxes, evenAmount);
            AdjustLastAmountForRounding(amountTextBoxes, evenAmount, finalAmount);
        }

        private void FillTextBoxesWithEvenAmount(List<TextBox> amountTextBoxes, decimal evenAmount)
        {
            for (int i = 0; i < amountTextBoxes.Count; i++)
            {
                amountTextBoxes[i].Text = evenAmount.ToString("0.00");
            }
        }

        private void AdjustLastAmountForRounding(List<TextBox> amountTextBoxes, decimal evenAmount, decimal finalAmount)
        {
            if (amountTextBoxes.Count > 0)
            {
                decimal totalEven = evenAmount * GetNumberOfSplits();
                decimal difference = finalAmount - totalEven;
                decimal lastAmount = evenAmount + difference;
                amountTextBoxes[amountTextBoxes.Count - 1].Text = lastAmount.ToString("0.00");
            }
        }

        private Button CreateOKButton(int yPosition)
        {
            return new Button
            {
                Text = "OK",
                DialogResult = DialogResult.OK,
                Location = new Point(250, yPosition + 40),
                Size = new Size(80, 30)
            };
        }

        private Button CreateCancelButton(int yPosition)
        {
            return new Button
            {
                Text = "Cancel",
                DialogResult = DialogResult.Cancel,
                Location = new Point(340, yPosition + 40),
                Size = new Size(80, 30)
            };
        }

        private void ProcessCustomSplitDialogResult(Form customSplitDialog)
        {
            var amountTextBoxes = customSplitDialog.Controls.OfType<TextBox>().ToList();
            
            SaveCustomAmounts(amountTextBoxes);
            
            if (!ValidateCustomAmountsTotal(amountTextBoxes))
            {
                ShowCustomSplitDialog(); // Show dialog again for correction
                return;
            }
            
            UpdateCustomSplitDisplay();
        }

        private void SaveCustomAmounts(List<TextBox> amountTextBoxes)
        {
            currentInvoice.Payments.Clear();
            
            foreach (TextBox tb in amountTextBoxes)
            {
                decimal amount = decimal.TryParse(tb.Text, out decimal parsedAmount) ? parsedAmount : 0;
                AddCustomSplitPayment(amount);
            }
        }

        private void AddCustomSplitPayment(decimal amount)
        {
            Payment splitPayment = new Payment
            {
                Amount = amount,
                Invoice = currentInvoice
            };
            currentInvoice.AddPayment(splitPayment);
        }

        private bool ValidateCustomAmountsTotal(List<TextBox> amountTextBoxes)
        {
            decimal totalEntered = CalculateTotalFromTextBoxes(amountTextBoxes);
            decimal finalAmount = GetFinalAmount();
            
            if (Math.Abs(totalEntered - finalAmount) >= 0.01m)
            {
                ShowCustomAmountValidationError(totalEntered, finalAmount);
                return false;
            }
            return true;
        }

        private void ShowCustomAmountValidationError(decimal totalEntered, decimal finalAmount)
        {
            MessageBox.Show(
                $"The sum of individual amounts (€{totalEntered:0.00}) does not match the total bill amount (€{finalAmount:0.00}).\n\nPlease adjust the amounts so they add up correctly.",
                "Amount Mismatch",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning);
        }
    }
}