using ChapeauG5.ChapeauModels;
using ChapeauG5.ChapeauService;
using System;
using System.Linq;
using System.Windows.Forms;
using ChapeauG5.ChapeauUI.Forms;

namespace ChapeauG5.ChapeauUI;

public partial class LoginForm : Form
{
    private AuthenticationService authService;
    private bool isLogging = false;

    public LoginForm()
    {
        InitializeComponent();
        authService = new AuthenticationService();
        InitializeForm();
    }

    private void InitializeForm()
    {
        // Set up enter key behavior
        this.AcceptButton = buttonLogin;

        // Focus on username field
        textBoxUsername.Focus();

        // Add event handlers
        textBoxUsername.KeyDown += TextBox_KeyDown;
        textBoxPassword.KeyDown += TextBox_KeyDown;

        // Add placeholder text
        SetPlaceholder(textBoxUsername, "Employee ID");
        SetPlaceholder(textBoxPassword, "PIN Code");
    }

    private void TextBox_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.KeyCode == Keys.Enter)
        {
            e.SuppressKeyPress = true;

            if (sender == textBoxUsername && string.IsNullOrEmpty(textBoxPassword.Text))
            {
                textBoxPassword.Focus();
            }
            else
            {
                buttonLogin.PerformClick();
            }
        }
    }

    private void SetPlaceholder(TextBox textBox, string placeholder)
    {
        textBox.Text = placeholder;
        textBox.ForeColor = SystemColors.GrayText;

        textBox.Enter += (sender, e) =>
        {
            if (textBox.Text == placeholder)
            {
                textBox.Text = "";
                textBox.ForeColor = SystemColors.WindowText;
            }
        };

        textBox.Leave += (sender, e) =>
        {
            if (string.IsNullOrEmpty(textBox.Text))
            {
                textBox.Text = placeholder;
                textBox.ForeColor = SystemColors.GrayText;
            }
        };
    }

    private async void buttonLogin_Click(object sender, EventArgs e)
    {
        if (isLogging) return;

        string username = textBoxUsername.Text;
        string password = textBoxPassword.Text;

        // Validate input
        if (string.IsNullOrEmpty(username) || username == "Employee ID")
        {
            ShowError("Please enter your Employee ID");
            textBoxUsername.Focus();
            return;
        }

        if (string.IsNullOrEmpty(password) || password == "PIN Code")
        {
            ShowError("Please enter your PIN code");
            textBoxPassword.Focus();
            return;
        }

        isLogging = true;
        SetLoginState(false);

        try
        {
            var credentials = new LoginCredentials
            {
                Username = username,
                PIN = password
            };

            var result = await authService.LoginAsync(credentials);

            if (result.Success)
            {
                // Store the current user
                AuthenticationService.SetCurrentUser(result.Employee);

                // Show the main form
                MainForm mainForm = new MainForm();
                mainForm.Show();

                // Hide this login form
                this.Hide();
            }
            else
            {
                ShowError(result.Message);

                // Clear password for security
                textBoxPassword.Text = "";
                textBoxPassword.Focus();

                // Add login attempt logging here if needed
            }
        }
        catch (Exception ex)
        {
            ShowError("An error occurred during login. Please try again.");
            // Log the exception
        }
        finally
        {
            isLogging = false;
            SetLoginState(true);
        }
    }

    private void SetLoginState(bool enabled)
    {
        textBoxUsername.Enabled = enabled;
        textBoxPassword.Enabled = enabled;
        buttonLogin.Enabled = enabled;

        if (enabled)
        {
            buttonLogin.Text = "Login";
            Cursor = Cursors.Default;
        }
        else
        {
            buttonLogin.Text = "Logging in...";
            Cursor = Cursors.WaitCursor;
        }
    }

    private void ShowError(string message)
    {
        labelError.Text = message;
        labelError.Visible = true;

        // Hide error after 5 seconds
        var timer = new System.Windows.Forms.Timer { Interval = 5000 };
        timer.Tick += (s, e) =>
        {
            labelError.Visible = false;
            timer.Stop();
            timer.Dispose();
        };
        timer.Start();
    }

    private void linkLabelForgotPassword_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
    {
        MessageBox.Show("Please contact your manager to reset your password.",
            "Password Reset",
            MessageBoxButtons.OK,
            MessageBoxIcon.Information);
    }

    private void LoginForm_FormClosing(object sender, FormClosingEventArgs e)
    {
        Application.Exit();
    }

    // Quick login buttons for development/demo
    private void buttonQuickWaiter_Click(object sender, EventArgs e)
    {
        textBoxUsername.Text = "W001";
        textBoxPassword.Text = "1234";
        buttonLogin.PerformClick();
    }

    private void buttonQuickKitchen_Click(object sender, EventArgs e)
    {
        textBoxUsername.Text = "K001";
        textBoxPassword.Text = "1234";
        buttonLogin.PerformClick();
    }

    private void buttonQuickBar_Click(object sender, EventArgs e)
    {
        textBoxUsername.Text = "B001";
        textBoxPassword.Text = "1234";
        buttonLogin.PerformClick();
    }

    private void buttonQuickManager_Click(object sender, EventArgs e)
    {
        textBoxUsername.Text = "M001";
        textBoxPassword.Text = "1234";
        buttonLogin.PerformClick();
    }
}