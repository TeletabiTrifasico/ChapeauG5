using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using ChapeauModel;
using ChapeauService;

namespace ChapeauG5
{
    public partial class SplitInvoicesForm : Form
    {
        private List<Invoice> splitInvoices;
        private int originalInvoiceId;
        private Employee loggedInEmployee;
        private PaymentService paymentService;
        private List<bool> paidStatus;
        
        public bool AllPaid => paidStatus.TrueForAll(status => status);
        
        public SplitInvoicesForm(List<Invoice> splitInvoices, int originalInvoiceId, Employee employee)
        {
            InitializeComponent();
            this.splitInvoices = splitInvoices;
            this.originalInvoiceId = originalInvoiceId;
            this.loggedInEmployee = employee;
            this.paymentService = new PaymentService();
            this.paidStatus = new List<bool>();
            
            // Initialize all as unpaid
            for (int i = 0; i < splitInvoices.Count; i++)
            {
                paidStatus.Add(false);
            }
        }
        
        private void SplitInvoicesForm_Load(object sender, EventArgs e)
        {
            DisplaySplitInvoices();
        }
        
        private void DisplaySplitInvoices()
        {
            // Clear existing tabs
            tabSplitInvoices.TabPages.Clear();
            
            // Create a tab for each split invoice
            for (int i = 0; i < splitInvoices.Count; i++)
            {
                Invoice invoice = splitInvoices[i];
                
                // Create the tab page
                TabPage tabPage = new TabPage($"Person {i + 1}");
                
                // Create the items list view for this tab
                ListView lvItems = new ListView();
                lvItems.Location = new Point(10, 10);
                lvItems.Size = new Size(520, 200);
                lvItems.View = View.Details;
                lvItems.FullRowSelect = true;
                lvItems.GridLines = true;
                lvItems.Columns.Add("Item", 250);
                lvItems.Columns.Add("Qty", 50);
                lvItems.Columns.Add("Price", 80);
                lvItems.Columns.Add("Subtotal", 80);
                lvItems.Columns.Add("VAT", 60);
                
                // Add invoice items to list view
                foreach (InvoiceItem item in invoice.Items)
                {
                    ListViewItem lvi = new ListViewItem(item.ItemName);
                    lvi.SubItems.Add(item.Quantity.ToString());
                    lvi.SubItems.Add($"€{item.UnitPrice:0.00}");
                    lvi.SubItems.Add($"€{item.Subtotal:0.00}");
                    lvi.SubItems.Add($"{item.VatPercentage}%");
                    lvItems.Items.Add(lvi);
                }
                
                // Create the summary labels
                Label lblSubtotalLabel = new Label();
                lblSubtotalLabel.Location = new Point(370, 220);
                lblSubtotalLabel.Size = new Size(70, 20);
                lblSubtotalLabel.Text = "Subtotal:";
                lblSubtotalLabel.TextAlign = ContentAlignment.MiddleRight;
                
                Label lblSubtotal = new Label();
                lblSubtotal.Location = new Point(440, 220);
                lblSubtotal.Size = new Size(90, 20);
                lblSubtotal.Text = $"€{invoice.TotalAmount:0.00}";
                lblSubtotal.TextAlign = ContentAlignment.MiddleRight;
                
                Label lblVatLabel = new Label();
                lblVatLabel.Location = new Point(370, 240);
                lblVatLabel.Size = new Size(70, 20);
                lblVatLabel.Text = "VAT:";
                lblVatLabel.TextAlign = ContentAlignment.MiddleRight;
                
                Label lblVat = new Label();
                lblVat.Location = new Point(440, 240);
                lblVat.Size = new Size(90, 20);
                lblVat.Text = $"€{invoice.TotalVat:0.00}";
                lblVat.TextAlign = ContentAlignment.MiddleRight;
                
                Label lblTotalLabel = new Label();
                lblTotalLabel.Location = new Point(370, 265);
                lblTotalLabel.Size = new Size(70, 20);
                lblTotalLabel.Text = "Total:";
                lblTotalLabel.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
                lblTotalLabel.TextAlign = ContentAlignment.MiddleRight;
                
                Label lblTotal = new Label();
                lblTotal.Location = new Point(440, 265);
                lblTotal.Size = new Size(90, 20);
                lblTotal.Text = $"€{(invoice.TotalAmount + invoice.TotalVat):0.00}";
                lblTotal.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
                lblTotal.TextAlign = ContentAlignment.MiddleRight;
                
                // Payment method controls
                GroupBox grpPayment = new GroupBox();
                grpPayment.Location = new Point(10, 220);
                grpPayment.Size = new Size(350, 100);
                grpPayment.Text = "Payment Method";
                
                ComboBox cmbPaymentMethod = new ComboBox();
                cmbPaymentMethod.Location = new Point(20, 30);
                cmbPaymentMethod.Size = new Size(200, 25);
                cmbPaymentMethod.DropDownStyle = ComboBoxStyle.DropDownList;
                cmbPaymentMethod.Items.Add("Cash");
                cmbPaymentMethod.Items.Add("Debit Card");
                cmbPaymentMethod.Items.Add("Credit Card (VISA)");
                cmbPaymentMethod.Items.Add("Credit Card (AMEX)");
                cmbPaymentMethod.SelectedIndex = 0;
                cmbPaymentMethod.Tag = i; // Store the invoice index in the tag
                
                Label lblTip = new Label();
                lblTip.Location = new Point(20, 65);
                lblTip.Size = new Size(80, 25);
                lblTip.Text = "Tip Amount:";
                lblTip.TextAlign = ContentAlignment.MiddleLeft;
                
                TextBox txtTip = new TextBox();
                txtTip.Location = new Point(110, 65);
                txtTip.Size = new Size(100, 25);
                txtTip.Text = "0.00";
                txtTip.Tag = i; // Store the invoice index
                
                grpPayment.Controls.Add(cmbPaymentMethod);
                grpPayment.Controls.Add(lblTip);
                grpPayment.Controls.Add(txtTip);
                
                // Process payment button
                Button btnPay = new Button();
                btnPay.Location = new Point(370, 330);
                btnPay.Size = new Size(160, 40);
                btnPay.Text = "Process Payment";
                btnPay.Tag = i; // Store the invoice index
                btnPay.BackColor = Color.LightGreen;
                btnPay.Click += new EventHandler(BtnPay_Click);
                
                // Add controls to tab page
                tabPage.Controls.Add(lvItems);
                tabPage.Controls.Add(lblSubtotalLabel);
                tabPage.Controls.Add(lblSubtotal);
                tabPage.Controls.Add(lblVatLabel);
                tabPage.Controls.Add(lblVat);
                tabPage.Controls.Add(lblTotalLabel);
                tabPage.Controls.Add(lblTotal);
                tabPage.Controls.Add(grpPayment);
                tabPage.Controls.Add(btnPay);
                
                // Add the tab page to the control
                tabSplitInvoices.TabPages.Add(tabPage);
            }
        }
        
        private void BtnPay_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            int invoiceIndex = (int)btn.Tag;
            
            // Get the current tab page
            TabPage currentTab = tabSplitInvoices.TabPages[invoiceIndex];
            
            // Find the payment method combo box and tip textbox in this tab
            ComboBox cmbMethod = null;
            TextBox txtTip = null;
            
            foreach (Control control in currentTab.Controls)
            {
                if (control is GroupBox grp)
                {
                    foreach (Control innerControl in grp.Controls)
                    {
                        if (innerControl is ComboBox cmb && (int)cmb.Tag == invoiceIndex)
                            cmbMethod = cmb;
                        else if (innerControl is TextBox txt && (int)txt.Tag == invoiceIndex)
                            txtTip = txt;
                    }
                }
            }
            
            if (cmbMethod != null && txtTip != null)
            {
                try
                {
                    // Get payment method
                    PaymentMethod paymentMethod = PaymentMethod.Cash;
                    switch (cmbMethod.SelectedIndex)
                    {
                        case 0: paymentMethod = PaymentMethod.Cash; break;
                        case 1: paymentMethod = PaymentMethod.DebitCard; break;
                        case 2: paymentMethod = PaymentMethod.CreditCardVisa; break;
                        case 3: paymentMethod = PaymentMethod.CreditCardAmex; break;
                    }
                    
                    // Get tip amount
                    decimal tipAmount = 0;
                    if (!string.IsNullOrEmpty(txtTip.Text))
                    {
                        tipAmount = decimal.Parse(txtTip.Text);
                    }
                    
                    // Get invoice details
                    Invoice splitInvoice = splitInvoices[invoiceIndex];
                    decimal totalAmount = splitInvoice.TotalAmount + splitInvoice.TotalVat;
                    
                    // Create payment
                    Payment payment = new Payment
                    {
                        InvoiceId = originalInvoiceId, // Associate with the original invoice
                        PaymentMethod = paymentMethod,
                        TotalPrice = totalAmount,
                        VatPercentage = splitInvoice.TotalAmount != 0 ? 
                        (int)Math.Round(splitInvoice.TotalVat / splitInvoice.TotalAmount * 100, 0) : 0,
                        TipAmount = tipAmount,
                        FinalAmount = totalAmount + tipAmount,
                        Feedback = $"Split payment {invoiceIndex + 1} of {splitInvoices.Count}",
                        EmployeeId = loggedInEmployee
                    };
                    
                    // Process payment
                    int paymentId = paymentService.ProcessPayment(payment);
                    
                    // Mark as paid
                    paidStatus[invoiceIndex] = true;
                    btn.Enabled = false;
                    btn.Text = "Paid";
                    
                    // Check if all paid
                    if (AllPaid)
                    {
                        MessageBox.Show("All split payments have been processed!", "Success", 
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                        this.DialogResult = DialogResult.OK;
                        this.Close();
                    }
                    else
                    {
                        // Move to next unpaid tab
                        for (int i = 0; i < paidStatus.Count; i++)
                        {
                            int nextIndex = (invoiceIndex + i + 1) % paidStatus.Count;
                            if (!paidStatus[nextIndex])
                            {
                                tabSplitInvoices.SelectedIndex = nextIndex;
                                break;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error processing payment: {ex.Message}", 
                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
    
    public partial class SplitInvoicesForm
    {
        private TabControl tabSplitInvoices;
        private Button btnClose;
        
        private void InitializeComponent()
        {
            this.Text = "Split Bills";
            this.Size = new Size(580, 450);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.StartPosition = FormStartPosition.CenterParent;
            this.MaximizeBox = false;
            
            this.tabSplitInvoices = new TabControl();
            this.tabSplitInvoices.Location = new Point(10, 10);
            this.tabSplitInvoices.Size = new Size(550, 380);
            
            this.btnClose = new Button();
            this.btnClose.Text = "Close";
            this.btnClose.DialogResult = DialogResult.Cancel;
            this.btnClose.Location = new Point(460, 395);
            this.btnClose.Size = new Size(100, 30);
            
            this.Controls.Add(this.tabSplitInvoices);
            this.Controls.Add(this.btnClose);
            
            this.Load += new EventHandler(this.SplitInvoicesForm_Load);
        }
    }
}