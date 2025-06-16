using ChapeauModel;
using ChapeauService;
using ChapeauUI;
using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace ChapeauG5
{
    public partial class LoginForm : Form
    {
        private AuthenticationService authService;
        
        public LoginForm()
        {
            InitializeComponent();
            authService = new AuthenticationService();
            LoadLogoImage();
        }
        
        private void LoadLogoImage()
        {
            try
            {
                // Try multiple locations in order of preference
                string[] possiblePaths = new string[]
                {
                    // 1. Look in the output directory (where the exe runs)
                    "logo.png",
                    Path.Combine(Application.StartupPath, "logo.png"),
                    
                    // 2. Look relative to the output directory
                    Path.Combine("..", "logo.png"),
                    
                    // 3. Look in the project root directory (for development)
                    Path.Combine("..", "..", "..", "logo.png"),
                    Path.Combine("..", "..", "..", "..", "logo.png"),
                    
                    // 4. Fallback to GitHub URL (if internet is available)
                    "https://raw.githubusercontent.com/TeletabiTrifasico/ChapeauG5/Hugo/logo.png"
                };

                // Try each path until we find the image
                foreach (string path in possiblePaths)
                {
                    try
                    {
                        if (path.StartsWith("http"))
                        {
                            // Download from URL
                            using (var webClient = new System.Net.WebClient())
                            {
                                byte[] data = webClient.DownloadData(path);
                                using (var ms = new MemoryStream(data))
                                {
                                    pictureBoxLogo.Image = Image.FromStream(ms);
                                    Console.WriteLine($"Logo loaded from URL: {path}");
                                    return;
                                }
                            }
                        }
                        else if (File.Exists(path))
                        {
                            // Load from file system
                            pictureBoxLogo.Image = Image.FromFile(path);
                            Console.WriteLine($"Logo loaded from: {path}");
                            return;
                        }
                    }
                    catch
                    {
                        // Continue to the next path if this one fails
                        continue;
                    }
                }

                // If we get here, we couldn't find the logo in any location
                Console.WriteLine("Could not find logo image in any standard location");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading logo: {ex.Message}");
            }
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
                var timeoutTask = Task.Delay(20000); // 10 second timeout
                
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


            switch (role)
            {
                case EmployeeRole.Waiter:
                    formToShow = new Form1(); // Your waiter dashboard
                    break;
            
                    break;
                case EmployeeRole.Manager:
                    formToShow = new ManagerDashboardForm(); // Create this form
                    break;
                default:
                    formToShow = new Form1();
                    break;
            }


            // Here is where we change forms based on role

            Form formToShow;
            
            switch (role)
            {
                case EmployeeRole.Waiter:
                    formToShow = new TableView(ChapeauApp.LoggedInUser);
                    break;
                /*
                case EmployeeRole.Bar:
                    formToShow = new BarDashboardForm(); // Create this form
                    break;
                case EmployeeRole.Kitchen:
                    formToShow = new KitchenDashboardForm(); // Create this form
                    break;
                case EmployeeRole.Manager:
                    formToShow = new ManagerDashboardForm(); // Create this form
                    break;
                */
                default:
                    formToShow = new TableView(ChapeauApp.LoggedInUser);
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