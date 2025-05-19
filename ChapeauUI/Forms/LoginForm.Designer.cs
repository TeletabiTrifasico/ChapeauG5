namespace ChapeauG5.ChapeauUI
{
    partial class LoginForm
    {
        private System.ComponentModel.IContainer components = null;
        private Panel panelMain;
        private Panel panelLogin;
        private PictureBox pictureBoxLogo;
        private Label labelAppName;
        private Label labelWelcome;
        private TextBox textBoxUsername;
        private TextBox textBoxPassword;
        private Button buttonLogin;
        private Label labelError;
        private LinkLabel linkLabelForgotPassword;
        private Panel panelQuickLogin;
        private Button buttonQuickWaiter;
        private Button buttonQuickKitchen;
        private Button buttonQuickBar;
        private Button buttonQuickManager;

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
            panelMain = new Panel();
            panelLogin = new Panel();
            pictureBoxLogo = new PictureBox();
            labelAppName = new Label();
            labelWelcome = new Label();
            textBoxUsername = new TextBox();
            textBoxPassword = new TextBox();
            buttonLogin = new Button();
            labelError = new Label();
            linkLabelForgotPassword = new LinkLabel();
            panelQuickLogin = new Panel();
            buttonQuickWaiter = new Button();
            buttonQuickKitchen = new Button();
            buttonQuickBar = new Button();
            buttonQuickManager = new Button();
            panelMain.SuspendLayout();
            panelLogin.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBoxLogo).BeginInit();
            panelQuickLogin.SuspendLayout();
            SuspendLayout();
            // 
            // panelMain
            // 
            panelMain.BackColor = Color.FromArgb(236, 240, 241);
            panelMain.Controls.Add(panelLogin);
            panelMain.Dock = DockStyle.Fill;
            panelMain.Location = new Point(0, 0);
            panelMain.Margin = new Padding(5, 6, 5, 6);
            panelMain.Name = "panelMain";
            panelMain.Size = new Size(1707, 1170);
            panelMain.TabIndex = 0;
            // 
            // panelLogin
            // 
            panelLogin.BackColor = Color.White;
            panelLogin.Controls.Add(pictureBoxLogo);
            panelLogin.Controls.Add(labelAppName);
            panelLogin.Controls.Add(labelWelcome);
            panelLogin.Controls.Add(textBoxUsername);
            panelLogin.Controls.Add(textBoxPassword);
            panelLogin.Controls.Add(buttonLogin);
            panelLogin.Controls.Add(labelError);
            panelLogin.Controls.Add(linkLabelForgotPassword);
            panelLogin.Controls.Add(panelQuickLogin);
            panelLogin.Dock = DockStyle.Fill;
            panelLogin.Location = new Point(0, 0);
            panelLogin.Margin = new Padding(5, 6, 5, 6);
            panelLogin.Name = "panelLogin";
            panelLogin.Size = new Size(1707, 1170);
            panelLogin.TabIndex = 0;
            panelLogin.Paint += PanelLogin_Paint;
            // 
            // pictureBoxLogo
            // 
            pictureBoxLogo.BackColor = Color.FromArgb(52, 73, 94);
            pictureBoxLogo.Location = new Point(502, 15);
            pictureBoxLogo.Margin = new Padding(5, 6, 5, 6);
            pictureBoxLogo.Name = "pictureBoxLogo";
            pictureBoxLogo.Size = new Size(167, 192);
            pictureBoxLogo.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBoxLogo.TabIndex = 0;
            pictureBoxLogo.TabStop = false;
            // 
            // labelAppName
            // 
            labelAppName.AutoSize = true;
            labelAppName.Font = new Font("Segoe UI", 18F, FontStyle.Bold);
            labelAppName.ForeColor = Color.FromArgb(52, 73, 94);
            labelAppName.Location = new Point(380, 231);
            labelAppName.Margin = new Padding(5, 0, 5, 0);
            labelAppName.Name = "labelAppName";
            labelAppName.Size = new Size(414, 48);
            labelAppName.TabIndex = 1;
            labelAppName.Text = "Chapeau Restaurant G5";
            labelAppName.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // labelWelcome
            // 
            labelWelcome.AutoSize = true;
            labelWelcome.Font = new Font("Segoe UI", 12F);
            labelWelcome.ForeColor = Color.FromArgb(149, 165, 166);
            labelWelcome.Location = new Point(451, 293);
            labelWelcome.Margin = new Padding(5, 0, 5, 0);
            labelWelcome.Name = "labelWelcome";
            labelWelcome.Size = new Size(279, 32);
            labelWelcome.TabIndex = 2;
            labelWelcome.Text = "Please log in to continue";
            labelWelcome.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // textBoxUsername
            // 
            textBoxUsername.BorderStyle = BorderStyle.FixedSingle;
            textBoxUsername.Font = new Font("Segoe UI", 12F);
            textBoxUsername.Location = new Point(335, 380);
            textBoxUsername.Margin = new Padding(5, 6, 5, 6);
            textBoxUsername.Name = "textBoxUsername";
            textBoxUsername.Size = new Size(499, 39);
            textBoxUsername.TabIndex = 3;
            // 
            // textBoxPassword
            // 
            textBoxPassword.BorderStyle = BorderStyle.FixedSingle;
            textBoxPassword.Font = new Font("Segoe UI", 12F);
            textBoxPassword.Location = new Point(335, 457);
            textBoxPassword.Margin = new Padding(5, 6, 5, 6);
            textBoxPassword.Name = "textBoxPassword";
            textBoxPassword.Size = new Size(499, 39);
            textBoxPassword.TabIndex = 4;
            textBoxPassword.UseSystemPasswordChar = true;
            // 
            // buttonLogin
            // 
            buttonLogin.BackColor = Color.FromArgb(39, 174, 96);
            buttonLogin.FlatStyle = FlatStyle.Flat;
            buttonLogin.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            buttonLogin.ForeColor = Color.White;
            buttonLogin.Location = new Point(335, 553);
            buttonLogin.Margin = new Padding(5, 6, 5, 6);
            buttonLogin.Name = "buttonLogin";
            buttonLogin.Size = new Size(500, 77);
            buttonLogin.TabIndex = 5;
            buttonLogin.Text = "Login";
            buttonLogin.UseVisualStyleBackColor = false;
            buttonLogin.Click += buttonLogin_Click;
            // 
            // labelError
            // 
            labelError.AutoSize = true;
            labelError.Font = new Font("Segoe UI", 10F);
            labelError.ForeColor = Color.FromArgb(231, 76, 60);
            labelError.Location = new Point(335, 515);
            labelError.Margin = new Padding(5, 0, 5, 0);
            labelError.Name = "labelError";
            labelError.Size = new Size(0, 28);
            labelError.TabIndex = 6;
            labelError.TextAlign = ContentAlignment.MiddleCenter;
            labelError.Visible = false;
            // 
            // linkLabelForgotPassword
            // 
            linkLabelForgotPassword.AutoSize = true;
            linkLabelForgotPassword.Font = new Font("Segoe UI", 10F);
            linkLabelForgotPassword.Location = new Point(477, 649);
            linkLabelForgotPassword.Margin = new Padding(5, 0, 5, 0);
            linkLabelForgotPassword.Name = "linkLabelForgotPassword";
            linkLabelForgotPassword.Size = new Size(167, 28);
            linkLabelForgotPassword.TabIndex = 7;
            linkLabelForgotPassword.TabStop = true;
            linkLabelForgotPassword.Text = "Forgot Password?";
            linkLabelForgotPassword.TextAlign = ContentAlignment.MiddleCenter;
            linkLabelForgotPassword.LinkClicked += linkLabelForgotPassword_LinkClicked;
            // 
            // panelQuickLogin
            // 
            panelQuickLogin.Controls.Add(buttonQuickWaiter);
            panelQuickLogin.Controls.Add(buttonQuickKitchen);
            panelQuickLogin.Controls.Add(buttonQuickBar);
            panelQuickLogin.Controls.Add(buttonQuickManager);
            panelQuickLogin.Location = new Point(335, 707);
            panelQuickLogin.Margin = new Padding(5, 6, 5, 6);
            panelQuickLogin.Name = "panelQuickLogin";
            panelQuickLogin.Size = new Size(500, 154);
            panelQuickLogin.TabIndex = 8;
            // 
            // buttonQuickWaiter
            // 
            buttonQuickWaiter.BackColor = Color.FromArgb(52, 152, 219);
            buttonQuickWaiter.FlatStyle = FlatStyle.Flat;
            buttonQuickWaiter.ForeColor = Color.White;
            buttonQuickWaiter.Location = new Point(0, 0);
            buttonQuickWaiter.Margin = new Padding(5, 6, 5, 6);
            buttonQuickWaiter.Name = "buttonQuickWaiter";
            buttonQuickWaiter.Size = new Size(117, 135);
            buttonQuickWaiter.TabIndex = 0;
            buttonQuickWaiter.Text = "Waiter";
            buttonQuickWaiter.UseVisualStyleBackColor = false;
            buttonQuickWaiter.Click += buttonQuickWaiter_Click;
            // 
            // buttonQuickKitchen
            // 
            buttonQuickKitchen.BackColor = Color.FromArgb(155, 89, 182);
            buttonQuickKitchen.FlatStyle = FlatStyle.Flat;
            buttonQuickKitchen.ForeColor = Color.White;
            buttonQuickKitchen.Location = new Point(133, 0);
            buttonQuickKitchen.Margin = new Padding(5, 6, 5, 6);
            buttonQuickKitchen.Name = "buttonQuickKitchen";
            buttonQuickKitchen.Size = new Size(117, 135);
            buttonQuickKitchen.TabIndex = 1;
            buttonQuickKitchen.Text = "Kitchen";
            buttonQuickKitchen.UseVisualStyleBackColor = false;
            buttonQuickKitchen.Click += buttonQuickKitchen_Click;
            // 
            // buttonQuickBar
            // 
            buttonQuickBar.BackColor = Color.FromArgb(243, 156, 18);
            buttonQuickBar.FlatStyle = FlatStyle.Flat;
            buttonQuickBar.ForeColor = Color.White;
            buttonQuickBar.Location = new Point(267, 0);
            buttonQuickBar.Margin = new Padding(5, 6, 5, 6);
            buttonQuickBar.Name = "buttonQuickBar";
            buttonQuickBar.Size = new Size(117, 135);
            buttonQuickBar.TabIndex = 2;
            buttonQuickBar.Text = "Bar";
            buttonQuickBar.UseVisualStyleBackColor = false;
            buttonQuickBar.Click += buttonQuickBar_Click;
            // 
            // buttonQuickManager
            // 
            buttonQuickManager.BackColor = Color.FromArgb(231, 76, 60);
            buttonQuickManager.FlatStyle = FlatStyle.Flat;
            buttonQuickManager.ForeColor = Color.White;
            buttonQuickManager.Location = new Point(400, 0);
            buttonQuickManager.Margin = new Padding(5, 6, 5, 6);
            buttonQuickManager.Name = "buttonQuickManager";
            buttonQuickManager.Size = new Size(100, 135);
            buttonQuickManager.TabIndex = 3;
            buttonQuickManager.Text = "Manager";
            buttonQuickManager.UseVisualStyleBackColor = false;
            buttonQuickManager.Click += buttonQuickManager_Click;
            // 
            // LoginForm
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1707, 1170);
            Controls.Add(panelMain);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Margin = new Padding(5, 6, 5, 6);
            MaximizeBox = false;
            Name = "LoginForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Chapeau - Login";
            FormClosing += LoginForm_FormClosing;
            panelMain.ResumeLayout(false);
            panelLogin.ResumeLayout(false);
            panelLogin.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBoxLogo).EndInit();
            panelQuickLogin.ResumeLayout(false);
            ResumeLayout(false);
        }

        private void PanelLogin_Paint(object sender, PaintEventArgs e)
        {
            // Add subtle shadow effect to login panel
            e.Graphics.DrawRectangle(new Pen(Color.FromArgb(189, 195, 199), 1),
                new Rectangle(0, 0, panelLogin.Width - 1, panelLogin.Height - 1));
        }
    }
}