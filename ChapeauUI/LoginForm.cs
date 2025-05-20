using System;
using System.Drawing;
using System.Windows.Forms;
using ChapeauModel;
using ChapeauService;

namespace ChapeauG5
{
    public partial class LoginForm : Form
    {
        private AuthenticationService authService;
        
        public LoginForm()
        {
            InitializeComponent();
            authService = new AuthenticationService();
        }
        
        private async void btnLogin_Click(object sender, EventArgs e)
        {
            try
            {
                // Disable login button to prevent multiple clicks
                btnLogin.Enabled = false;
                
                // Show a loading indicator
                lblError.Text = "Logging in...";
                lblError.ForeColor = Color.Blue;
                
                // Validate input
                if (string.IsNullOrWhiteSpace(txtUsername.Text) || string.IsNullOrWhiteSpace(txtPassword.Text))
                {
                    lblError.Text = "Please enter both username and password.";
                    lblError.ForeColor = Color.Red;
                    return;
                }
                
                // Use the async version with a timeout
                var loginTask = authService.LoginAsync(txtUsername.Text, txtPassword.Text);
                
                // Add timeout to prevent indefinite freezing
                var timeoutTask = Task.Delay(10000); // 10 second timeout
                
                if (await Task.WhenAny(loginTask, timeoutTask) == timeoutTask)
                {
                    lblError.Text = "Login timed out. Check your database connection.";
                    lblError.ForeColor = Color.Red;
                    return;
                }
                
                // Get the result after we know the task completed
                Employee loggedInEmployee = await loginTask;
                
                // If we get here, login was successful
                ChapeauApp.LoggedInUser = loggedInEmployee;
                
                lblError.Text = ""; // Clear any error message
                
                // Open the appropriate form based on employee role
                OpenAppropriateForm(loggedInEmployee.Role);
                
                // Hide the login form
                this.Hide();
            }
            catch (Exception ex)
            {
                // Show errors on the label but if needed this can be changed to a popup
                lblError.Text = ex.Message;
                lblError.ForeColor = Color.Red;
            }
            finally
            {
                // Always re-enables the button
                btnLogin.Enabled = true;
            }
        }
        
        private void OpenAppropriateForm(EmployeeRole role)
        {
            Form formToShow = new Form1(); // Temporarily use Form1 for all roles

            // Here is where we change forms based on role

            /*
            //Example:
            Form formToShow;
            
            switch (role)
            {
                case EmployeeRole.Waiter:
                    formToShow = new Form1(); // Your waiter dashboard
                    break;
                case EmployeeRole.Bar:
                    formToShow = new BarDashboardForm(); // Create this form
                    break;
                case EmployeeRole.Kitchen:
                    formToShow = new KitchenDashboardForm(); // Create this form
                    break;
                case EmployeeRole.Manager:
                    formToShow = new ManagerDashboardForm(); // Create this form
                    break;
                default:
                    formToShow = new Form1();
                    break;
            }
            */
            
            // Set the form's start position and size
            formToShow.StartPosition = FormStartPosition.CenterScreen;
            formToShow.FormClosed += (s, args) => this.Close();
            formToShow.Show();
        }
    }
}