namespace ChapeauG5.ChapeauUI.Forms
{
    partial class SplashForm
    {
        private System.ComponentModel.IContainer components = null;
        private Panel panelContainer;
        private PictureBox pictureBoxLogo;
        private Label labelAppName;
        private Label labelVersion;
        private ProgressBar progressBar;

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
            this.panelContainer = new Panel();
            this.pictureBoxLogo = new PictureBox();
            this.labelAppName = new Label();
            this.labelVersion = new Label();
            this.progressBar = new ProgressBar();
            this.panelContainer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxLogo)).BeginInit();
            this.SuspendLayout();

            // 
            // panelContainer
            // 
            this.panelContainer.BackColor = Color.White;
            this.panelContainer.Controls.Add(this.pictureBoxLogo);
            this.panelContainer.Controls.Add(this.labelAppName);
            this.panelContainer.Controls.Add(this.labelVersion);
            this.panelContainer.Controls.Add(this.progressBar);
            this.panelContainer.Dock = DockStyle.Fill;
            this.panelContainer.Location = new Point(0, 0);
            this.panelContainer.Size = new Size(400, 300);

            // 
            // pictureBoxLogo
            // 
            // Comment out the problematic line:
            // this.pictureBoxLogo.Image = Properties.Resources.logo;
            this.pictureBoxLogo.Location = new Point(150, 50);
            this.pictureBoxLogo.Size = new Size(100, 100);
            this.pictureBoxLogo.SizeMode = PictureBoxSizeMode.StretchImage;
            this.pictureBoxLogo.BackColor = Color.FromArgb(52, 73, 94); // Placeholder background color

            // 
            // labelAppName
            // 
            this.labelAppName.AutoSize = true;
            this.labelAppName.Font = new Font("Segoe UI", 16F, FontStyle.Bold);
            this.labelAppName.ForeColor = Color.FromArgb(52, 73, 94);
            this.labelAppName.Location = new Point(75, 160);
            this.labelAppName.Size = new Size(250, 30);
            this.labelAppName.Text = "Chapeau Restaurant";
            this.labelAppName.TextAlign = ContentAlignment.MiddleCenter;

            // 
            // labelVersion
            // 
            this.labelVersion.AutoSize = true;
            this.labelVersion.Font = new Font("Segoe UI", 10F);
            this.labelVersion.ForeColor = Color.FromArgb(149, 165, 166);
            this.labelVersion.Location = new Point(170, 195);
            this.labelVersion.Size = new Size(60, 19);
            this.labelVersion.Text = "Version 1.0";
            this.labelVersion.TextAlign = ContentAlignment.MiddleCenter;

            // 
            // progressBar
            // 
            this.progressBar.Location = new Point(50, 240);
            this.progressBar.Size = new Size(300, 20);
            this.progressBar.Style = ProgressBarStyle.Marquee;
            this.progressBar.ForeColor = Color.FromArgb(39, 174, 96);

            // 
            // SplashForm
            // 
            this.AutoScaleDimensions = new SizeF(6F, 13F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.ClientSize = new Size(400, 300);
            this.Controls.Add(this.panelContainer);
            this.FormBorderStyle = FormBorderStyle.None;
            this.Name = "SplashForm";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Text = "Loading...";
            this.BackColor = Color.White;
            this.Shown += new EventHandler(this.SplashForm_Shown);
            this.panelContainer.ResumeLayout(false);
            this.panelContainer.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxLogo)).EndInit();
            this.ResumeLayout(false);
        }
    }
}