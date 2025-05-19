using ChapeauG5.ChapeauModels;
using ChapeauG5.ChapeauUI.Forms;
using ChapeauG5.ChapeauService;
using System;
using System.Windows.Forms;
using ChapeauG5.ChapeauEnums;

namespace ChapeauG5.ChapeauUI.Forms;

public partial class MainForm : Form
{
    private Form currentChildForm;

    public MainForm()
    {
        InitializeComponent();
        InitializeForm();
    }

    private void InitializeForm()
    {
        // Set the current user info
        var currentUser = AuthenticationService.GetCurrentUser();
        if (currentUser != null)
        {
            labelUserName.Text = currentUser.FullName;
            labelUserRole.Text = currentUser.Role.ToString();
        }

        // Setup menu based on user role
        ConfigureMenuBasedOnRole();

        // Load default form based on role
        LoadDefaultFormForRole();
    }

    private void LoadDefaultFormForRole()
    {
        var currentUser = AuthenticationService.GetCurrentUser();
        if (currentUser == null) return;

        // Open the most appropriate form for the user's role 
        switch (currentUser.Role)
        {
            case EmployeeRole.Manager:

                OpenChildForm(new TableOverviewForm());
                break;
            case EmployeeRole.Kitchen:
                OpenChildForm(new KitchenBarViewForm(true)); // true for kitchen
                break;
            case EmployeeRole.Bar:
                OpenChildForm(new KitchenBarViewForm(false)); // false for bar
                break;
            case EmployeeRole.Waiter:
            default:
                OpenChildForm(new TableOverviewForm());
                break;
        }
    }

    private void ConfigureMenuBasedOnRole()
    {
        var currentUser = AuthenticationService.GetCurrentUser();
        if (currentUser == null) return;

        // Reset all menu visibility
        foreach (ToolStripMenuItem item in menuStrip.Items)
        {
            item.Visible = false;
        }

        // Configure based on role
        switch (currentUser.Role)
        {
            case EmployeeRole.Manager:
                menuTables.Visible = true;
                menuOrders.Visible = true;
                menuKitchen.Visible = true;
                menuBar.Visible = true;
                menuPayment.Visible = true;
                menuManagement.Visible = true;
                menuReports.Visible = true;
                break;

            case EmployeeRole.Waiter:
                menuTables.Visible = true;
                menuOrders.Visible = true;
                menuPayment.Visible = true;
                break;

            case EmployeeRole.Kitchen:
                menuKitchen.Visible = true;
                menuOrders.Visible = true; 
                break;

            case EmployeeRole.Bar:
                menuBar.Visible = true;
                menuOrders.Visible = true; 
                break;
        }

        // Help and logout are always visible
        menuHelp.Visible = true;
        menuLogout.Visible = true;
    }

    private void OpenChildForm(Form childForm)
    {
        if (currentChildForm != null)
        {
            currentChildForm.Close();
            currentChildForm.Dispose();
        }

        currentChildForm = childForm;
        childForm.TopLevel = false;
        childForm.FormBorderStyle = FormBorderStyle.None;
        childForm.Dock = DockStyle.Fill;
        panelContent.Controls.Clear();
        panelContent.Controls.Add(childForm);
        panelContent.Tag = childForm;
        childForm.BringToFront();
        childForm.Show();
    }

    private void menuTables_Click(object sender, EventArgs e)
    {
        OpenChildForm(new TableOverviewForm());
    }

    private void menuOrders_Click(object sender, EventArgs e)
    {
       
    }

    private void menuKitchen_Click(object sender, EventArgs e)
    {
        OpenChildForm(new KitchenBarViewForm(true));
    }

    private void menuBar_Click(object sender, EventArgs e)
    {
        OpenChildForm(new KitchenBarViewForm(false));
    }

    private void menuPayment_Click(object sender, EventArgs e)
    {
        // For now, just show message
        MessageBox.Show("Please select a table with active order to process payment.",
            "Payment Information",
            MessageBoxButtons.OK,
            MessageBoxIcon.Information);
    }

    private void menuManagement_Click(object sender, EventArgs e)
    {
        OpenChildForm(new ManagementForm());
    }

    private void menuReports_Click(object sender, EventArgs e)
    {
        // Show management form with financial tab selected
        MessageBox.Show("Reports are accessed through the Management menu.",
            "Information",
            MessageBoxButtons.OK,
            MessageBoxIcon.Information);
    }

    private void menuHelp_Click(object sender, EventArgs e)
    {
        MessageBox.Show("Chapeau Restaurant Ordering System\nVersion 1\n\nFor support, contact your system admin.",
            "Help",
            MessageBoxButtons.OK,
            MessageBoxIcon.Information);
    }

    private void menuLogout_Click(object sender, EventArgs e)
    {
        DialogResult result = MessageBox.Show("Are you sure you want to log out?",
            "Confirm Logout",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Question);

        if (result == DialogResult.Yes)
        {
            // Clear current user
            AuthenticationService.ClearCurrentUser();

            // Close all child forms
            if (currentChildForm != null)
            {
                currentChildForm.Close();
                currentChildForm.Dispose();
                currentChildForm = null;
            }

            // Show login form
            LoginForm loginForm = new LoginForm();
            loginForm.Show();
            this.Hide();
        }
    }

    private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
    {
        Application.Exit();
    }

    // Timer for updating time
    private void timer_Tick(object sender, EventArgs e)
    {
        labelDateTime.Text = DateTime.Now.ToString("dddd, MMMM dd, yyyy - hh:mm:ss tt");
    }
}
