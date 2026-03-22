using System;
using System.Drawing;
using System.Windows.Forms;

namespace APUCodeCamp
{
    public class frmLecturerDashboard : Form
    {
        private Label  lblTitle;
        private Label  lblWelcome;
        private Label  lblDateTime;
        private Button btnEnrolStudent;
        private Button btnApproveRequests;
        private Button btnDeleteStudent;
        private Button btnViewStudents;
        private Button btnUpdateProfile;
        private Button btnLogout;
        private System.Windows.Forms.Timer tmrClock;

        public frmLecturerDashboard()
        {
            InitializeComponent();
        }

        private void frmLecturerDashboard_Load(object sender, EventArgs e)
        {
            lblWelcome.Text  = "Welcome, " + UserSession.Name + "  |  Staff: " + UserSession.LecturerStaffID;
            lblDateTime.Text = DateTime.Now.ToString("dd/MM/yyyy  hh:mm tt");
        }

        private void tmrClock_Tick(object sender, EventArgs e)
        {
            lblDateTime.Text = DateTime.Now.ToString("dd/MM/yyyy  hh:mm tt");
        }

        private void btnEnrolStudent_Click(object sender, EventArgs e)    { new frmEnrolStudent().ShowDialog(); }
        private void btnApproveRequests_Click(object sender, EventArgs e) { new frmApproveRequests().ShowDialog(); }
        private void btnDeleteStudent_Click(object sender, EventArgs e)   { new frmDeleteStudent().ShowDialog(); }
        private void btnViewStudents_Click(object sender, EventArgs e)    { new frmLecturerViewStudents().ShowDialog(); }

        private void btnUpdateProfile_Click(object sender, EventArgs e)
        {
            new frmUpdateProfile().ShowDialog();
            lblWelcome.Text = "Welcome, " + UserSession.Name + "  |  Staff: " + UserSession.LecturerStaffID;
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
            this.btnEnrolStudent    = new Button();
            this.btnApproveRequests = new Button();
            this.btnDeleteStudent   = new Button();
            this.btnViewStudents    = new Button();
            this.btnUpdateProfile   = new Button();
            this.btnLogout          = new Button();
            this.tmrClock           = new System.Windows.Forms.Timer();

            this.SuspendLayout();

            this.Text            = "Lecturer Dashboard";
            this.Size            = new Size(500, 430);
            this.StartPosition   = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox     = false;

            // Title
            this.lblTitle.Text      = "APU CodeCamp - Lecturer Portal";
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
            this.btnEnrolStudent.Text      = "1.  Register & Enrol Student";
            this.btnEnrolStudent.Font      = new Font("Arial", 10);
            this.btnEnrolStudent.TextAlign = ContentAlignment.MiddleLeft;
            this.btnEnrolStudent.Location  = new Point(80, 80);
            this.btnEnrolStudent.Size      = new Size(320, 38);
            this.btnEnrolStudent.Click    += new EventHandler(this.btnEnrolStudent_Click);

            // Button 2
            this.btnApproveRequests.Text      = "2.  Approve / Reject Student Requests";
            this.btnApproveRequests.Font      = new Font("Arial", 10);
            this.btnApproveRequests.TextAlign = ContentAlignment.MiddleLeft;
            this.btnApproveRequests.Location  = new Point(80, 128);
            this.btnApproveRequests.Size      = new Size(320, 38);
            this.btnApproveRequests.Click    += new EventHandler(this.btnApproveRequests_Click);

            // Button 3
            this.btnDeleteStudent.Text      = "3.  Delete Student (Completed)";
            this.btnDeleteStudent.Font      = new Font("Arial", 10);
            this.btnDeleteStudent.TextAlign = ContentAlignment.MiddleLeft;
            this.btnDeleteStudent.Location  = new Point(80, 176);
            this.btnDeleteStudent.Size      = new Size(320, 38);
            this.btnDeleteStudent.Click    += new EventHandler(this.btnDeleteStudent_Click);

            // Button 4
            this.btnViewStudents.Text      = "4.  View Students List";
            this.btnViewStudents.Font      = new Font("Arial", 10);
            this.btnViewStudents.TextAlign = ContentAlignment.MiddleLeft;
            this.btnViewStudents.Location  = new Point(80, 224);
            this.btnViewStudents.Size      = new Size(320, 38);
            this.btnViewStudents.Click    += new EventHandler(this.btnViewStudents_Click);

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
            this.Controls.Add(this.btnEnrolStudent);
            this.Controls.Add(this.btnApproveRequests);
            this.Controls.Add(this.btnDeleteStudent);
            this.Controls.Add(this.btnViewStudents);
            this.Controls.Add(this.btnUpdateProfile);
            this.Controls.Add(this.btnLogout);

            this.Load += new EventHandler(this.frmLecturerDashboard_Load);
            this.ResumeLayout(false);
        }
    }
}
