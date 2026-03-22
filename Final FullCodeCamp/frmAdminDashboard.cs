using System;
using System.Drawing;
using System.Windows.Forms;

namespace APUCodeCamp
{
    public class frmAdminDashboard : Form
    {
        private Label  lblTitle;
        private Label  lblWelcome;
        private Label  lblDateTime;
        private Button btnManageTrainers;
        private Button btnAssignTrainer;
        private Button btnViewFeedback;
        private Button btnMonthlyReport;
        private Button btnUpdateProfile;
        private Button btnLogout;
        private System.Windows.Forms.Timer tmrClock;

        public frmAdminDashboard()
        {
            InitializeComponent();
        }

        private void frmAdminDashboard_Load(object sender, EventArgs e)
        {
            lblWelcome.Text  = "Welcome, " + UserSession.Name + "  |  Staff: " + UserSession.AdminStaffID;
            lblDateTime.Text = DateTime.Now.ToString("dd/MM/yyyy  hh:mm tt");
        }

        private void tmrClock_Tick(object sender, EventArgs e)
        {
            lblDateTime.Text = DateTime.Now.ToString("dd/MM/yyyy  hh:mm tt");
        }

        private void btnManageTrainers_Click(object sender, EventArgs e)  { new frmManageTrainers().ShowDialog(); }
        private void btnAssignTrainer_Click(object sender, EventArgs e)   { new frmAssignTrainer().ShowDialog(); }
        private void btnViewFeedback_Click(object sender, EventArgs e)    { new frmViewFeedback().ShowDialog(); }
        private void btnMonthlyReport_Click(object sender, EventArgs e)   { new frmMonthlyReport().ShowDialog(); }

        private void btnUpdateProfile_Click(object sender, EventArgs e)
        {
            new frmUpdateProfile().ShowDialog();
            lblWelcome.Text = "Welcome, " + UserSession.Name + "  |  Staff: " + UserSession.AdminStaffID;
        }

        private void btnLogout_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to logout?", "Confirm Logout",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                UserSession.ClearSession();
                new frmLogin().Show();
                this.Close();
            }
        }

        private void InitializeComponent()
        {
            this.lblTitle           = new Label();
            this.lblWelcome         = new Label();
            this.lblDateTime        = new Label();
            this.btnManageTrainers  = new Button();
            this.btnAssignTrainer   = new Button();
            this.btnViewFeedback    = new Button();
            this.btnMonthlyReport   = new Button();
            this.btnUpdateProfile   = new Button();
            this.btnLogout          = new Button();
            this.tmrClock           = new System.Windows.Forms.Timer();

            this.SuspendLayout();

            // Form
            this.Text            = "Administrator Dashboard";
            this.Size            = new Size(500, 430);
            this.StartPosition   = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox     = false;

            // Title
            this.lblTitle.Text      = "APU CodeCamp - Administrator Portal";
            this.lblTitle.Font      = new Font("Arial", 12, FontStyle.Bold);
            this.lblTitle.Location  = new Point(10, 15);
            this.lblTitle.Size      = new Size(463, 25);
            this.lblTitle.TextAlign = ContentAlignment.MiddleCenter;

            // Welcome
            this.lblWelcome.Text     = "Welcome";
            this.lblWelcome.Font     = new Font("Arial", 8);
            this.lblWelcome.Location = new Point(10, 48);
            this.lblWelcome.Size     = new Size(310, 18);

            // DateTime
            this.lblDateTime.Font      = new Font("Arial", 8);
            this.lblDateTime.Location  = new Point(330, 48);
            this.lblDateTime.Size      = new Size(143, 18);
            this.lblDateTime.TextAlign = ContentAlignment.MiddleRight;

            // Button 1
            this.btnManageTrainers.Text      = "1.  Manage Trainers (Register / Remove)";
            this.btnManageTrainers.Font      = new Font("Arial", 10);
            this.btnManageTrainers.TextAlign = ContentAlignment.MiddleLeft;
            this.btnManageTrainers.Location  = new Point(80, 80);
            this.btnManageTrainers.Size      = new Size(320, 38);
            this.btnManageTrainers.Click    += new EventHandler(this.btnManageTrainers_Click);

            // Button 2
            this.btnAssignTrainer.Text      = "2.  Assign Trainer to Module & Level";
            this.btnAssignTrainer.Font      = new Font("Arial", 10);
            this.btnAssignTrainer.TextAlign = ContentAlignment.MiddleLeft;
            this.btnAssignTrainer.Location  = new Point(80, 128);
            this.btnAssignTrainer.Size      = new Size(320, 38);
            this.btnAssignTrainer.Click    += new EventHandler(this.btnAssignTrainer_Click);

            // Button 3
            this.btnViewFeedback.Text      = "3.  View Trainer Feedback";
            this.btnViewFeedback.Font      = new Font("Arial", 10);
            this.btnViewFeedback.TextAlign = ContentAlignment.MiddleLeft;
            this.btnViewFeedback.Location  = new Point(80, 176);
            this.btnViewFeedback.Size      = new Size(320, 38);
            this.btnViewFeedback.Click    += new EventHandler(this.btnViewFeedback_Click);

            // Button 4
            this.btnMonthlyReport.Text      = "4.  Monthly Income Report";
            this.btnMonthlyReport.Font      = new Font("Arial", 10);
            this.btnMonthlyReport.TextAlign = ContentAlignment.MiddleLeft;
            this.btnMonthlyReport.Location  = new Point(80, 224);
            this.btnMonthlyReport.Size      = new Size(320, 38);
            this.btnMonthlyReport.Click    += new EventHandler(this.btnMonthlyReport_Click);

            // Button 5
            this.btnUpdateProfile.Text      = "5.  Update My Profile";
            this.btnUpdateProfile.Font      = new Font("Arial", 10);
            this.btnUpdateProfile.TextAlign = ContentAlignment.MiddleLeft;
            this.btnUpdateProfile.Location  = new Point(80, 272);
            this.btnUpdateProfile.Size      = new Size(320, 38);
            this.btnUpdateProfile.Click    += new EventHandler(this.btnUpdateProfile_Click);

            // Logout
            this.btnLogout.Text     = "Logout";
            this.btnLogout.Font     = new Font("Arial", 9);
            this.btnLogout.Location = new Point(370, 328);
            this.btnLogout.Size     = new Size(100, 28);
            this.btnLogout.Click   += new EventHandler(this.btnLogout_Click);

            // Timer
            this.tmrClock.Interval = 1000;
            this.tmrClock.Enabled  = true;
            this.tmrClock.Tick    += new EventHandler(this.tmrClock_Tick);

            this.Controls.Add(this.lblTitle);
            this.Controls.Add(this.lblWelcome);
            this.Controls.Add(this.lblDateTime);
            this.Controls.Add(this.btnManageTrainers);
            this.Controls.Add(this.btnAssignTrainer);
            this.Controls.Add(this.btnViewFeedback);
            this.Controls.Add(this.btnMonthlyReport);
            this.Controls.Add(this.btnUpdateProfile);
            this.Controls.Add(this.btnLogout);

            this.Load += new EventHandler(this.frmAdminDashboard_Load);
            this.ResumeLayout(false);
        }
    }
}
