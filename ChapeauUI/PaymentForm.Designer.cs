using System;
using System.Drawing;
using System.Windows.Forms;

namespace ChapeauG5
{
    partial class PaymentForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            
            // Form settings
            this.Text = "Process Payment";
            this.Size = new Size(800, 800);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            
            // Header Label
            this.lblHeader = new Label();
            this.lblHeader.Location = new Point(20, 20);
            this.lblHeader.Size = new Size(760, 30);
            this.lblHeader.Font = new Font("Segoe UI", 14, FontStyle.Bold);
            this.lblHeader.TextAlign = ContentAlignment.MiddleCenter;
            this.lblHeader.Text = "Payment for Table X";
            
            // Order Items ListView
            this.lvOrderItems = new ListView();
            this.lvOrderItems.Location = new Point(20, 60);
            this.lvOrderItems.Size = new Size(760, 300);
            this.lvOrderItems.View = View.Details;
            this.lvOrderItems.FullRowSelect = true;
            this.lvOrderItems.Columns.Add("Item", 250);
            this.lvOrderItems.Columns.Add("Qty", 50);
            this.lvOrderItems.Columns.Add("Price", 80);
            this.lvOrderItems.Columns.Add("VAT", 60);
            this.lvOrderItems.Columns.Add("Subtotal", 100);
            
            // Totals Panel
            Panel totalsPanel = new Panel();
            totalsPanel.Location = new Point(20, 370);
            totalsPanel.Size = new Size(760, 150);
            totalsPanel.BorderStyle = BorderStyle.FixedSingle;
            
            // Subtotal (ex. VAT)
            Label lblSubtotal = new Label();
            lblSubtotal.Location = new Point(10, 10);
            lblSubtotal.Size = new Size(200, 25);
            lblSubtotal.Text = "Subtotal (ex. VAT):";
            lblSubtotal.TextAlign = ContentAlignment.MiddleLeft;
            lblSubtotal.Font = new Font("Segoe UI", 10);
            
            this.lblSubtotalValue = new Label();
            this.lblSubtotalValue.Location = new Point(220, 10);
            this.lblSubtotalValue.Size = new Size(100, 25);
            this.lblSubtotalValue.TextAlign = ContentAlignment.MiddleRight;
            this.lblSubtotalValue.Font = new Font("Segoe UI", 10);
            this.lblSubtotalValue.Text = "€0.00";
            
            // Low VAT Amount (9%)
            Label lblLowVat = new Label();
            lblLowVat.Location = new Point(10, 40);
            lblLowVat.Size = new Size(200, 25);
            lblLowVat.Text = "Low VAT Amount (9%):";
            lblLowVat.TextAlign = ContentAlignment.MiddleLeft;
            lblLowVat.Font = new Font("Segoe UI", 10);
            
            this.lblLowVatValue = new Label();
            this.lblLowVatValue.Location = new Point(220, 40);
            this.lblLowVatValue.Size = new Size(100, 25);
            this.lblLowVatValue.TextAlign = ContentAlignment.MiddleRight;
            this.lblLowVatValue.Font = new Font("Segoe UI", 10);
            this.lblLowVatValue.Text = "€0.00";
            
            // High VAT Amount (21%)
            Label lblHighVat = new Label();
            lblHighVat.Location = new Point(10, 70);
            lblHighVat.Size = new Size(200, 25);
            lblHighVat.Text = "High VAT Amount (21%):";
            lblHighVat.TextAlign = ContentAlignment.MiddleLeft;
            lblHighVat.Font = new Font("Segoe UI", 10);
            
            this.lblHighVatValue = new Label();
            this.lblHighVatValue.Location = new Point(220, 70);
            this.lblHighVatValue.Size = new Size(100, 25);
            this.lblHighVatValue.TextAlign = ContentAlignment.MiddleRight;
            this.lblHighVatValue.Font = new Font("Segoe UI", 10);
            this.lblHighVatValue.Text = "€0.00";
            
            // Total (incl. VAT)
            Label lblTotal = new Label();
            lblTotal.Location = new Point(10, 110);
            lblTotal.Size = new Size(200, 25);
            lblTotal.Text = "Total (incl. VAT):";
            lblTotal.TextAlign = ContentAlignment.MiddleLeft;
            lblTotal.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            
            this.lblTotalValue = new Label();
            this.lblTotalValue.Location = new Point(220, 110);
            this.lblTotalValue.Size = new Size(100, 25);
            this.lblTotalValue.TextAlign = ContentAlignment.MiddleRight;
            this.lblTotalValue.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            this.lblTotalValue.Text = "€0.00";
            
            // Optional Tip
            Label lblTip = new Label();
            lblTip.Location = new Point(400, 10);
            lblTip.Size = new Size(150, 25);
            lblTip.Text = "Optional Tip (€):";
            lblTip.TextAlign = ContentAlignment.MiddleLeft;
            lblTip.Font = new Font("Segoe UI", 10);
            
            this.txtTip = new TextBox();
            this.txtTip.Location = new Point(550, 10);
            this.txtTip.Size = new Size(100, 25);
            this.txtTip.TextAlign = HorizontalAlignment.Right;
            this.txtTip.Text = "0.00";
            this.txtTip.TextChanged += new EventHandler(this.txtTip_TextChanged);
            
            // Final Amount
            Label lblFinalAmount = new Label();
            lblFinalAmount.Location = new Point(400, 110);
            lblFinalAmount.Size = new Size(150, 25);
            lblFinalAmount.Text = "Final Amount:";
            lblFinalAmount.TextAlign = ContentAlignment.MiddleLeft;
            lblFinalAmount.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            
            this.lblFinalAmountValue = new Label();
            this.lblFinalAmountValue.Location = new Point(550, 110);
            this.lblFinalAmountValue.Size = new Size(100, 25);
            this.lblFinalAmountValue.TextAlign = ContentAlignment.MiddleRight;
            this.lblFinalAmountValue.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            this.lblFinalAmountValue.Text = "€0.00";
            
            // Add controls to totals panel
            totalsPanel.Controls.Add(lblSubtotal);
            totalsPanel.Controls.Add(this.lblSubtotalValue);
            totalsPanel.Controls.Add(lblLowVat);
            totalsPanel.Controls.Add(this.lblLowVatValue);
            totalsPanel.Controls.Add(lblHighVat);
            totalsPanel.Controls.Add(this.lblHighVatValue);
            totalsPanel.Controls.Add(lblTotal);
            totalsPanel.Controls.Add(this.lblTotalValue);
            totalsPanel.Controls.Add(lblTip);
            totalsPanel.Controls.Add(this.txtTip);
            totalsPanel.Controls.Add(lblFinalAmount);
            totalsPanel.Controls.Add(this.lblFinalAmountValue);
            
            // Payment Method Section
            Label lblPaymentMethod = new Label();
            lblPaymentMethod.Location = new Point(20, 530);
            lblPaymentMethod.Size = new Size(150, 25);
            lblPaymentMethod.Text = "Payment Method:";
            lblPaymentMethod.TextAlign = ContentAlignment.MiddleLeft;
            lblPaymentMethod.Font = new Font("Segoe UI", 10);
            
            this.cmbPaymentMethod = new ComboBox();
            this.cmbPaymentMethod.Location = new Point(170, 530);
            this.cmbPaymentMethod.Size = new Size(200, 25);
            this.cmbPaymentMethod.DropDownStyle = ComboBoxStyle.DropDownList;
            
            // Feedback Section
            Label lblFeedbackType = new Label();
            lblFeedbackType.Location = new Point(20, 570);
            lblFeedbackType.Size = new Size(150, 25);
            lblFeedbackType.Text = "Feedback Type:";
            lblFeedbackType.TextAlign = ContentAlignment.MiddleLeft;
            lblFeedbackType.Font = new Font("Segoe UI", 10);
            
            this.cmbFeedbackType = new ComboBox();
            this.cmbFeedbackType.Location = new Point(170, 570);
            this.cmbFeedbackType.Size = new Size(200, 25);
            this.cmbFeedbackType.DropDownStyle = ComboBoxStyle.DropDownList;
            this.cmbFeedbackType.SelectedIndexChanged += new EventHandler(this.cmbFeedbackType_SelectedIndexChanged);
            
            Label lblFeedback = new Label();
            lblFeedback.Location = new Point(20, 610);
            lblFeedback.Size = new Size(150, 25);
            lblFeedback.Text = "Feedback:";
            lblFeedback.TextAlign = ContentAlignment.MiddleLeft;
            lblFeedback.Font = new Font("Segoe UI", 10);
            
            this.txtFeedback = new TextBox();
            this.txtFeedback.Location = new Point(170, 610);
            this.txtFeedback.Size = new Size(610, 80);
            this.txtFeedback.Multiline = true;
            this.txtFeedback.Enabled = false;
            
            // Process Payment Button
            this.btnProcessPayment = new Button();
            this.btnProcessPayment.Location = new Point(450, 700);
            this.btnProcessPayment.Size = new Size(160, 50);
            this.btnProcessPayment.Text = "Process Payment";
            this.btnProcessPayment.BackColor = Color.LightGreen;
            this.btnProcessPayment.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            this.btnProcessPayment.Click += new EventHandler(this.btnProcessPayment_Click);
            
            // Cancel Button
            this.btnCancel = new Button();
            this.btnCancel.Location = new Point(620, 700);
            this.btnCancel.Size = new Size(160, 50);
            this.btnCancel.Text = "Cancel";
            this.btnCancel.BackColor = Color.LightCoral;
            this.btnCancel.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            this.btnCancel.Click += new EventHandler(this.btnCancel_Click);
            
            // Add controls to form
            this.Controls.Add(this.lblHeader);
            this.Controls.Add(this.lvOrderItems);
            this.Controls.Add(totalsPanel);
            this.Controls.Add(lblPaymentMethod);
            this.Controls.Add(this.cmbPaymentMethod);
            this.Controls.Add(lblFeedbackType);
            this.Controls.Add(this.cmbFeedbackType);
            this.Controls.Add(lblFeedback);
            this.Controls.Add(this.txtFeedback);
            this.Controls.Add(this.btnProcessPayment);
            this.Controls.Add(this.btnCancel);
            
            // Load event
            this.Load += new EventHandler(this.PaymentForm_Load);
        }

        #endregion

        private Label lblHeader;
        private ListView lvOrderItems;
        private Label lblSubtotalValue;
        private Label lblLowVatValue;
        private Label lblHighVatValue;
        private Label lblTotalValue;
        private TextBox txtTip;
        private Label lblFinalAmountValue;
        private ComboBox cmbPaymentMethod;
        private ComboBox cmbFeedbackType;
        private TextBox txtFeedback;
        private Button btnProcessPayment;
        private Button btnCancel;
    }
}