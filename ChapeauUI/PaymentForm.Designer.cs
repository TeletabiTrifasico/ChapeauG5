using System;
using System.Drawing;
using System.Windows.Forms;

namespace ChapeauG5
{
    partial class PaymentForm
    {
        private System.ComponentModel.IContainer components = null;
        
        private ListView lvInvoiceItems;
        private Label lblOrderInfo;
        private Label lblSubtotalLabel;
        private Label lblSubtotal;
        private Label lblVatAmountLabel;
        private Label lblVatAmount;
        private Label lblTotalLabel;
        private Label lblTotal;
        private Panel pnlExistingPayments;
        private Label lblExistingPayments;
        private ListView lvExistingPayments;
        private Label lblTotalPaid;
        private Label lblRemaining;
        private Panel pnlPayment;
        private GroupBox grpPaymentMethod;
        private ComboBox cmbPaymentMethod;
        private Label lblTipAmount;
        private TextBox txtTipAmount;
        private Button btnSplitBill;
        private Button btnProcessPayment;
        private GroupBox grpFeedback;
        private RadioButton rdoComment;
        private RadioButton rdoComplaint;
        private RadioButton rdoCommendation;
        private TextBox txtFeedback;
        
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }
        
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.Text = "Payment";
            this.Size = new System.Drawing.Size(800, 700);
            this.StartPosition = FormStartPosition.CenterScreen;
            
            // Invoice items list view
            this.lvInvoiceItems = new ListView();
            this.lvInvoiceItems.Location = new Point(20, 60);
            this.lvInvoiceItems.Size = new Size(550, 200);
            this.lvInvoiceItems.View = View.Details;
            this.lvInvoiceItems.FullRowSelect = true;
            this.lvInvoiceItems.GridLines = true;
            this.lvInvoiceItems.Columns.Add("Item", 250);
            this.lvInvoiceItems.Columns.Add("Qty", 50);
            this.lvInvoiceItems.Columns.Add("Price", 80);
            this.lvInvoiceItems.Columns.Add("Subtotal", 100);
            this.lvInvoiceItems.Columns.Add("VAT", 70);
            
            // Order info label
            this.lblOrderInfo = new Label();
            this.lblOrderInfo.Location = new Point(20, 20);
            this.lblOrderInfo.Size = new Size(550, 30);
            this.lblOrderInfo.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            this.lblOrderInfo.Text = "Order Invoice";
            
            // Invoice summary labels
            this.lblSubtotalLabel = new Label();
            this.lblSubtotalLabel.Location = new Point(430, 270);
            this.lblSubtotalLabel.Size = new Size(70, 20);
            this.lblSubtotalLabel.Text = "Subtotal:";
            this.lblSubtotalLabel.TextAlign = ContentAlignment.MiddleRight;
            
            this.lblSubtotal = new Label();
            this.lblSubtotal.Location = new Point(500, 270);
            this.lblSubtotal.Size = new Size(70, 20);
            this.lblSubtotal.TextAlign = ContentAlignment.MiddleRight;
            
            this.lblVatAmountLabel = new Label();
            this.lblVatAmountLabel.Location = new Point(430, 295);
            this.lblVatAmountLabel.Size = new Size(70, 20);
            this.lblVatAmountLabel.Text = "VAT:";
            this.lblVatAmountLabel.TextAlign = ContentAlignment.MiddleRight;
            
            this.lblVatAmount = new Label();
            this.lblVatAmount.Location = new Point(500, 295);
            this.lblVatAmount.Size = new Size(70, 20);
            this.lblVatAmount.TextAlign = ContentAlignment.MiddleRight;
            
            this.lblTotalLabel = new Label();
            this.lblTotalLabel.Location = new Point(430, 325);
            this.lblTotalLabel.Size = new Size(70, 20);
            this.lblTotalLabel.Text = "Total:";
            this.lblTotalLabel.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            this.lblTotalLabel.TextAlign = ContentAlignment.MiddleRight;
            
            this.lblTotal = new Label();
            this.lblTotal.Location = new Point(500, 325);
            this.lblTotal.Size = new Size(70, 20);
            this.lblTotal.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            this.lblTotal.TextAlign = ContentAlignment.MiddleRight;
            
            // Existing payments panel
            this.pnlExistingPayments = new Panel();
            this.pnlExistingPayments.Location = new Point(20, 360);
            this.pnlExistingPayments.Size = new Size(750, 150);
            this.pnlExistingPayments.Visible = false;
            
            this.lblExistingPayments = new Label();
            this.lblExistingPayments.Location = new Point(0, 0);
            this.lblExistingPayments.Size = new Size(150, 25);
            this.lblExistingPayments.Text = "Existing Payments:";
            this.lblExistingPayments.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            
            this.lvExistingPayments = new ListView();
            this.lvExistingPayments.Location = new Point(0, 25);
            this.lvExistingPayments.Size = new Size(750, 80);
            this.lvExistingPayments.View = View.Details;
            this.lvExistingPayments.FullRowSelect = true;
            this.lvExistingPayments.GridLines = true;
            this.lvExistingPayments.Columns.Add("ID", 50);
            this.lvExistingPayments.Columns.Add("Method", 100);
            this.lvExistingPayments.Columns.Add("Amount", 80);
            this.lvExistingPayments.Columns.Add("Tip", 80);
            this.lvExistingPayments.Columns.Add("Total", 80);
            
            this.lblTotalPaid = new Label();
            this.lblTotalPaid.Location = new Point(0, 110);
            this.lblTotalPaid.Size = new Size(200, 25);
            this.lblTotalPaid.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            
            this.lblRemaining = new Label();
            this.lblRemaining.Location = new Point(200, 110);
            this.lblRemaining.Size = new Size(200, 25);
            this.lblRemaining.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            
            this.pnlExistingPayments.Controls.Add(this.lblExistingPayments);
            this.pnlExistingPayments.Controls.Add(this.lvExistingPayments);
            this.pnlExistingPayments.Controls.Add(this.lblTotalPaid);
            this.pnlExistingPayments.Controls.Add(this.lblRemaining);
            
            // Payment panel
            this.pnlPayment = new Panel();
            this.pnlPayment.Location = new Point(20, 510);
            this.pnlPayment.Size = new Size(750, 150);
            
            // Payment method group
            this.grpPaymentMethod = new GroupBox();
            this.grpPaymentMethod.Location = new Point(0, 0);
            this.grpPaymentMethod.Size = new Size(250, 110);
            this.grpPaymentMethod.Text = "Payment Method";
            
            this.cmbPaymentMethod = new ComboBox();
            this.cmbPaymentMethod.Location = new Point(20, 30);
            this.cmbPaymentMethod.Size = new Size(210, 25);
            this.cmbPaymentMethod.DropDownStyle = ComboBoxStyle.DropDownList;
            
            this.lblTipAmount = new Label();
            this.lblTipAmount.Location = new Point(20, 65);
            this.lblTipAmount.Size = new Size(80, 25);
            this.lblTipAmount.Text = "Tip Amount:";
            this.lblTipAmount.TextAlign = ContentAlignment.MiddleLeft;
            
            this.txtTipAmount = new TextBox();
            this.txtTipAmount.Location = new Point(110, 65);
            this.txtTipAmount.Size = new Size(120, 25);
            this.txtTipAmount.Text = "0.00";
            
            this.grpPaymentMethod.Controls.Add(this.cmbPaymentMethod);
            this.grpPaymentMethod.Controls.Add(this.lblTipAmount);
            this.grpPaymentMethod.Controls.Add(this.txtTipAmount);
            
            // Feedback group
            this.grpFeedback = new GroupBox();
            this.grpFeedback.Location = new Point(270, 0);
            this.grpFeedback.Size = new Size(350, 110);
            this.grpFeedback.Text = "Customer Feedback";
            
            this.rdoComment = new RadioButton();
            this.rdoComment.Location = new Point(20, 25);
            this.rdoComment.Size = new Size(100, 20);
            this.rdoComment.Text = "Comment";
            this.rdoComment.Checked = true;
            
            this.rdoComplaint = new RadioButton();
            this.rdoComplaint.Location = new Point(120, 25);
            this.rdoComplaint.Size = new Size(100, 20);
            this.rdoComplaint.Text = "Complaint";
            
            this.rdoCommendation = new RadioButton();
            this.rdoCommendation.Location = new Point(220, 25);
            this.rdoCommendation.Size = new Size(120, 20);
            this.rdoCommendation.Text = "Commendation";
            
            this.txtFeedback = new TextBox();
            this.txtFeedback.Location = new Point(20, 50);
            this.txtFeedback.Size = new Size(310, 50);
            this.txtFeedback.Multiline = true;
            
            this.grpFeedback.Controls.Add(this.rdoComment);
            this.grpFeedback.Controls.Add(this.rdoComplaint);
            this.grpFeedback.Controls.Add(this.rdoCommendation);
            this.grpFeedback.Controls.Add(this.txtFeedback);
            
            this.pnlPayment.Controls.Add(this.grpPaymentMethod);
            this.pnlPayment.Controls.Add(this.grpFeedback);
            
            // Buttons
            this.btnSplitBill = new Button();
            this.btnSplitBill.Location = new Point(600, 60);
            this.btnSplitBill.Size = new Size(150, 40);
            this.btnSplitBill.Text = "Split Bill";
            this.btnSplitBill.Click += new EventHandler(this.btnSplitBill_Click);
            
            this.btnProcessPayment = new Button();
            this.btnProcessPayment.Location = new Point(630, 530);
            this.btnProcessPayment.Size = new Size(150, 50);
            this.btnProcessPayment.Text = "Process Payment";
            this.btnProcessPayment.BackColor = Color.LightGreen;
            this.btnProcessPayment.Click += new EventHandler(this.btnProcessPayment_Click);
            
            // Add controls to form
            this.Controls.Add(this.lvInvoiceItems);
            this.Controls.Add(this.lblOrderInfo);
            this.Controls.Add(this.lblSubtotalLabel);
            this.Controls.Add(this.lblSubtotal);
            this.Controls.Add(this.lblVatAmountLabel);
            this.Controls.Add(this.lblVatAmount);
            this.Controls.Add(this.lblTotalLabel);
            this.Controls.Add(this.lblTotal);
            this.Controls.Add(this.pnlExistingPayments);
            this.Controls.Add(this.pnlPayment);
            this.Controls.Add(this.btnSplitBill);
            this.Controls.Add(this.btnProcessPayment);
        }
    }
}