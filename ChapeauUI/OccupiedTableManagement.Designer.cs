namespace ChapeauG5.ChapeauUI
{
    partial class OccupiedTableManagement
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
            freeTablebtn = new Button();
            takeOrderbtn = new Button();
            lbltabelNumber = new Label();
            setAsServedBtn = new Button();
            lvReadyToBeServedItems = new ListView();
            label1 = new Label();
            SuspendLayout();
            // 
            // freeTablebtn
            // 
            freeTablebtn.BackColor = Color.LightGreen;
            freeTablebtn.Location = new Point(404, 126);
            freeTablebtn.Name = "freeTablebtn";
            freeTablebtn.Size = new Size(390, 142);
            freeTablebtn.TabIndex = 0;
            freeTablebtn.Text = "Free Table";
            freeTablebtn.UseVisualStyleBackColor = false;
            freeTablebtn.Click += freeTablebtn_Click;
            // 
            // takeOrderbtn
            // 
            takeOrderbtn.BackColor = Color.LightBlue;
            takeOrderbtn.Location = new Point(404, 325);
            takeOrderbtn.Name = "takeOrderbtn";
            takeOrderbtn.Size = new Size(390, 142);
            takeOrderbtn.TabIndex = 1;
            takeOrderbtn.Text = "Take Order";
            takeOrderbtn.UseVisualStyleBackColor = false;
            takeOrderbtn.Click += takeOrderbtn_Click;
            // 
            // lbltabelNumber
            // 
            lbltabelNumber.AutoSize = true;
            lbltabelNumber.Location = new Point(542, 80);
            lbltabelNumber.Name = "lbltabelNumber";
            lbltabelNumber.Size = new Size(24, 32);
            lbltabelNumber.TabIndex = 2;
            lbltabelNumber.Text = "..";
            // 
            // setAsServedBtn
            // 
            setAsServedBtn.BackColor = Color.Coral;
            setAsServedBtn.Location = new Point(404, 509);
            setAsServedBtn.Name = "setAsServedBtn";
            setAsServedBtn.Size = new Size(390, 142);
            setAsServedBtn.TabIndex = 3;
            setAsServedBtn.Text = "Set as Served";
            setAsServedBtn.UseVisualStyleBackColor = false;
            setAsServedBtn.Click += setAsServedBtn_Click;
            // 
            // lvReadyToBeServedItems
            // 
            lvReadyToBeServedItems.Location = new Point(895, 211);
            lvReadyToBeServedItems.Name = "lvReadyToBeServedItems";
            lvReadyToBeServedItems.Size = new Size(400, 450);
            lvReadyToBeServedItems.TabIndex = 4;
            lvReadyToBeServedItems.UseCompatibleStateImageBehavior = false;
            lvReadyToBeServedItems.View = View.Details;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(895, 162);
            label1.Name = "label1";
            label1.Size = new Size(288, 32);
            label1.TabIndex = 5;
            label1.Text = "Ready to be served items:";
            // 
            // OccupiedTableManagement
            // 
            AutoScaleDimensions = new SizeF(13F, 32F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1400, 777);
            Controls.Add(label1);
            Controls.Add(lvReadyToBeServedItems);
            Controls.Add(setAsServedBtn);
            Controls.Add(lbltabelNumber);
            Controls.Add(takeOrderbtn);
            Controls.Add(freeTablebtn);
            Name = "OccupiedTableManagement";
            Text = "OccupiedTableManagement";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button freeTablebtn;
        private Button takeOrderbtn;
        private Label lbltabelNumber;
        private Button setAsServedBtn;
        private ListView lvReadyToBeServedItems;
        private Label label1;
    }
}