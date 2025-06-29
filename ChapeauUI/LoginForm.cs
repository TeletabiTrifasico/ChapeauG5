using Sysyem;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
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
            LoadLogoImage();
        }

        private void LoadLogoImage()
        {
            string[] possiblePaths = new string[]
            {
                "logo.png",
                Path.Combine(Application.StartupPath, "logo.png"),
                Path.Combine("..", "logo.png"),
                Path.Combine("..", "..", "..", "logo.png"),
                Path.Combine("..", "..", "..", "..", "logo.png"),
                "https://raw.githubusercontent.com/TeletabiTrifasico/ChapeauG5/Hugo/logo.png"
            };

            foreach (string path in possiblePaths)
            {
                try
                {
                    if (path.StartsWith("http"))
                    {
                        using (var webClient = new System.Net.WebClient())
                        {
                            byte[] data = webClient.DownloadData(path);
                            using (var ms = new MemoryStream(data))
                            {
                                pictureBoxLogo.Image = Image.FromStream(ms);
                                return;
                            }
                        }
                    }
                    else if (File.Exists(path))
                    {
                        pictureBoxLogo.Image = Image.FromFile(path);
                        return;
                    }
                }
                catch
                {
                    // Try the other way
                    continue;
                }
            }
            // If no logo is found, set a default image or leave it blank
            Console.WriteLine("Logo not found");
        }

        private async void btnLogin_Click(object sender, EventArgs e)
        {
            await LoginAsync();
        }

        private async Task LoginAsync()
        {
            try
            {
                btnLogin.Enabled = false;
                ShowStatus("Logging in...", Color.Blue);

                if (string.IsNullOrWhiteSpace(txtUsername.Text) || string.IsNullOrWhiteSpace(txtPassword.Text))
                {
                    ShowStatus("Please enter both username and password.", Color.Red);
                    return;
                }

                var loginTask = authService.LoginAsync(txtUsername.Text, txtPassword.Text);
                var timeoutTask = Task.Delay(20000);

                if (await Task.WhenAny(loginTask, timeoutTask) == timeoutTask)
                {
                    ShowStatus("Login timed out. Check your database connection.", Color.Red);
                    return;
                }

                Employee loggedInEmployee = await loginTask;
                ChapeauApp.LoggedInUser = loggedInEmployee;
                ShowStatus("", Color.Black);

                OpenAppropriateForm(loggedInEmployee.Role);
                this.Hide();
            }
            catch (Exception ex)
            {
                ShowStatus(ex.Message, Color.Red);
            }
            finally
            {
                btnLogin.Enabled = true;
            }
        }

        private void OpenAppropriateForm(EmployeeRole role)
        {
            Form formToShow;
            switch (role)
            {
                case EmployeeRole.Waiter:
                    formToShow = new TableView(ChapeauApp.LoggedInUser);
                    break;
                // Di?er roller eklenebilir
                default:
                    formToShow = new TableView(ChapeauApp.LoggedInUser);
                    break;
            }
            formToShow.StartPosition = FormStartPosition.CenterScreen;
            formToShow.FormClosed += (s, args) => this.Close();
            formToShow.Show();
        }

        private void ShowStatus(string message, Color color)
        {
            lblError.Text = message;
            lblError.ForeColor = color;
        }

        public void ClearCredentials()
        {
            txtUsername.Text = string.Empty;
            txtPassword.Text = string.Empty;
            lblError.Text = string.Empty;
        }
    }
}