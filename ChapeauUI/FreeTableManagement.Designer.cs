namespace ChapeauG5.ChapeauUI
{
    partial class FreeTableManagement
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
            occupyTableBtn = new Button();
            tableNumberlbl = new Label();
            SuspendLayout();
            // 
            // occupyTableBtn
            // 
            occupyTableBtn.Location = new Point(404, 280);
            occupyTableBtn.Name = "occupyTableBtn";
            occupyTableBtn.Size = new Size(380, 154);
            occupyTableBtn.TabIndex = 0;
            occupyTableBtn.Text = "Occupy Table";
            occupyTableBtn.UseVisualStyleBackColor = true;
            occupyTableBtn.Click += occupyTableBtn_Click;
            // 
            // tableNumberlbl
            // 
            tableNumberlbl.AutoSize = true;
            tableNumberlbl.Location = new Point(542, 161);
            tableNumberlbl.Name = "tableNumberlbl";
            tableNumberlbl.Size = new Size(24, 32);
            tableNumberlbl.TabIndex = 1;
            tableNumberlbl.Text = "..";
            // 
            // FreeTableManagement
            // 
            AutoScaleDimensions = new SizeF(13F, 32F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1217, 766);
            Controls.Add(tableNumberlbl);
            Controls.Add(occupyTableBtn);
            Name = "FreeTableManagement";
            Text = "FreeTableManagement";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button occupyTableBtn;
        private Label tableNumberlbl;
    }
}