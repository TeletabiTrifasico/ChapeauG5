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
            SuspendLayout();
            // 
            // freeTablebtn
            // 
            freeTablebtn.Location = new Point(404, 186);
            freeTablebtn.Name = "freeTablebtn";
            freeTablebtn.Size = new Size(390, 153);
            freeTablebtn.TabIndex = 0;
            freeTablebtn.Text = "Free Table";
            freeTablebtn.UseVisualStyleBackColor = true;
            freeTablebtn.Click += freeTablebtn_Click;
            // 
            // takeOrderbtn
            // 
            takeOrderbtn.Location = new Point(404, 410);
            takeOrderbtn.Name = "takeOrderbtn";
            takeOrderbtn.Size = new Size(390, 150);
            takeOrderbtn.TabIndex = 1;
            takeOrderbtn.Text = "Take Order";
            takeOrderbtn.UseVisualStyleBackColor = true;
            takeOrderbtn.Click += takeOrderbtn_Click;
            // 
            // lbltabelNumber
            // 
            lbltabelNumber.AutoSize = true;
            lbltabelNumber.Location = new Point(542, 103);
            lbltabelNumber.Name = "lbltabelNumber";
            lbltabelNumber.Size = new Size(24, 32);
            lbltabelNumber.TabIndex = 2;
            lbltabelNumber.Text = "..";
            // 
            // OccupiedTableManagement
            // 
            AutoScaleDimensions = new SizeF(13F, 32F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1191, 777);
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
    }
}