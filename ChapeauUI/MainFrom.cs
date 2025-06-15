using ChapeauG5.ChapeauUI;
using ChapeauModel;
using ChapeauService;
using System;
using System.Windows.Forms;
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
        // Set the current user info using team's authentication
        var currentUser = ChapeauApp.LoggedInUser;
        if (currentUser != null)
        {
            labelUserName.Text = currentUser.FullName;
            labelUserRole.Text = currentUser.Role.ToString();
        }
        else
        {
            MessageBox.Show("No user logged in. Returning to login.", "Authentication Error");
            Application.Exit();
        }

        // Setup menu based on user role
        ConfigureMenuBasedOnRole();

        // Load default form based on role
        LoadDefaultFormForRole();
    }

    private void LoadDefaultFormForRole()
    {
        var currentUser = ChapeauApp.LoggedInUser;
        if (currentUser == null) return;

        // Open the most appropriate form for the user's role 
        switch (currentUser.Role)
        {
            case EmployeeRole.Manager:
                // OpenChildForm(new TableOverviewForm());
                MessageBox.Show("Manager dashboard not yet implemented.", "Info");
                break;
            case EmployeeRole.Kitchen:
                OpenChildForm(new KitchenBarViewForm(true)); // true for kitchen
                break;
            case EmployeeRole.Bar:
                OpenChildForm(new KitchenBarViewForm(false)); // false for bar
                break;
            case EmployeeRole.Waiter:
            default:
                // OpenChildForm(new TableOverviewForm());
                MessageBox.Show("Waiter interface not yet implemented in MainForm.", "Info");
                break;
        }
    }

    private void ConfigureMenuBasedOnRole()
    {
        var currentUser = ChapeauApp.LoggedInUser;
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
                menuOrders.Visible = false;
                break;

            case EmployeeRole.Bar:
                menuBar.Visible = true;
                menuOrders.Visible = false;
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
        // OpenChildForm(new TableOverviewForm());
        MessageBox.Show("Table overview not yet implemented.", "Info");
    }

    private void menuOrders_Click(object sender, EventArgs e)
    {
        MessageBox.Show("Orders overview not yet implemented.", "Info");
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
        MessageBox.Show("Please select a table with active order to process payment.",
            "Payment Information",
            MessageBoxButtons.OK,
            MessageBoxIcon.Information);
    }

    private void menuManagement_Click(object sender, EventArgs e)
    {
        // OpenChildForm(new ManagementForm());
        MessageBox.Show("Management interface not yet implemented.", "Info");
    }

    private void menuReports_Click(object sender, EventArgs e)
    {
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
            ChapeauApp.LoggedInUser = null;

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