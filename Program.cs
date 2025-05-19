using System;
using System.Windows.Forms;
using ChapeauG5.ChapeauUI.Forms;
using ChapeauG5.ChapeauUI;
using System.Data.SqlClient;

namespace ChapeauG5
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            try
            {
                // Safely show splash screen, app loading
                SplashForm splash = null;
                try
                {
                    splash = new SplashForm();
                    splash.Show();
                    Application.DoEvents();

                    int splashDisplayTime = 2000; // 2 seconds
                    System.Threading.Thread.Sleep(splashDisplayTime);
                }
                finally
                {
                    // close and dispose the splash form
                    if (splash != null && !splash.IsDisposed)
                    {
                        splash.Close();
                        splash.Dispose();
                    }
                }
                Application.Run(new LoginForm());
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An unexpected error occurred: {ex.Message}\n\nThe application will now close.",
                    "Fatal Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }
    }
}