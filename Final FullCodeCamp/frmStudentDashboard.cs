using System;
using System.Drawing;
using System.Windows.Forms;

namespace APUCodeCamp
{
    public class frmStudentDashboard : Form
    {
        private Label  lblTitle;
        private Label  lblWelcome;
        private Label  lblDateTime;
        private Button btnViewSchedule;
        private Button btnSendRequest;
        private Button btnManageRequests;
        private Button btnInvoicePayment;
        private Button btnUpdateProfile;
        private Button btnLogout;
        private System.Windows.Forms.Timer tmrClock;

        public frmStudentDashboard()
        {
            InitializeComponent();
        }

        private void frmStudentDashboard_Load(object sender, EventArgs e)
        {
            lblWelcome.Text  = "Welcome, " + StudentSession.Name +
                               "  |  " + StudentSession.TPNumber +
                               "  |  " + StudentSession.StudyLevel;
            lblDateTime.Text = DateTime.Now.ToString("dd/MM/yyyy  hh:mm tt");
        }

        private void tmrClock_Tick(object sender, EventArgs e)
        {
            lblDateTime.Text = DateTime.Now.ToString("dd/MM/yyyy  hh:mm tt");
        }

        private void btnViewSchedule_Click(object sender, EventArgs e)
        {
            new frmViewSchedule().ShowDialog();
        }

        private void btnSendRequest_Click(object sender, EventArgs e)
        {
            new frmSendRequest().ShowDialog();
        }

        private void btnManageRequests_Click(object sender, EventArgs e)
        {
            new frmManageRequests().ShowDialog();
        }

        private void btnInvoicePayment_Click(object sender, EventArgs e)
        {
            new frmInvoicePayment().ShowDialog();
        }

        private void btnUpdateProfile_Click(object sender, EventArgs e)
        {
            new frmUpdateProfile().ShowDialog();
            lblWelcome.Text = "Welcome, " + StudentSession.Name +
                              "  |  " + StudentSession.TPNumber +
                              "  |  " + StudentSession.StudyLevel;
        }

        private void btnLogout_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to logout?", "Confirm Logout",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                StudentSession.ClearSession();
                new frmLogin().Show();
                this.Close();
            }
        }

        private void InitializeComponent()
        {
            this.lblTitle          = new Label();
            this.lblWelcome        = new Label();
            this.lblDateTime       = new Label();
            this.btnViewSchedule   = new Button();
            this.btnSendRequest    = new Button();
            this.btnManageRequests = new Button();
            this.btnInvoicePayment = new Button();
            this.btnUpdateProfile  = new Button();
            this.btnLogout         = new Button();
            this.tmrClock          = new System.Windows.Forms.Timer();

            this.SuspendLayout();

            // Form
            this.Text            = "Student Dashboard";
            this.Size            = new Size(500, 520);
            this.StartPosition   = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox     = false;

            // Title
            this.lblTitle.Text      = "APU CodeCamp - Student Portal";
            this.lblTitle.Font      = new Font("Arial", 12, FontStyle.Bold);
            this.lblTitle.Location  = new Point(10, 15);
            this.lblTitle.Size      = new Size(465, 25);
            this.lblTitle.TextAlign = ContentAlignment.MiddleCenter;

            // Separator line
            Panel line1 = new Panel();
            line1.BorderStyle = BorderStyle.FixedSingle;
            line1.Location    = new Point(10, 43);
            line1.Size        = new Size(463, 1);

            // Welcome label
            this.lblWelcome.Text     = "Welcome";
            this.lblWelcome.Font     = new Font("Arial", 8);
            this.lblWelcome.Location = new Point(10, 52);
            this.lblWelcome.Size     = new Size(350, 18);

            // Date/Time label
            this.lblDateTime.Font      = new Font("Arial", 8);
            this.lblDateTime.Location  = new Point(330, 52);
            this.lblDateTime.Size      = new Size(150, 18);
            this.lblDateTime.TextAlign = ContentAlignment.MiddleRight;

            // Menu buttons - all plain, stacked at Y: 90, 138, 186, 234, 282
            this.btnViewSchedule.Text      = "1.  View My Class Schedule";
            this.btnViewSchedule.Font      = new Font("Arial", 10);
            this.btnViewSchedule.TextAlign = ContentAlignment.MiddleLeft;
            this.btnViewSchedule.Location  = new Point(80, 90);
            this.btnViewSchedule.Size      = new Size(320, 38);
            this.btnViewSchedule.Click    += new EventHandler(this.btnViewSchedule_Click);

            this.btnSendRequest.Text      = "2.  Send Coaching Enrolment Request";
            this.btnSendRequest.Font      = new Font("Arial", 10);
            this.btnSendRequest.TextAlign = ContentAlignment.MiddleLeft;
            this.btnSendRequest.Location  = new Point(80, 138);
            this.btnSendRequest.Size      = new Size(320, 38);
            this.btnSendRequest.Click    += new EventHandler(this.btnSendRequest_Click);

            this.btnManageRequests.Text      = "3.  My Coaching Requests";
            this.btnManageRequests.Font      = new Font("Arial", 10);
            this.btnManageRequests.TextAlign = ContentAlignment.MiddleLeft;
            this.btnManageRequests.Location  = new Point(80, 186);
            this.btnManageRequests.Size      = new Size(320, 38);
            this.btnManageRequests.Click    += new EventHandler(this.btnManageRequests_Click);

            this.btnInvoicePayment.Text      = "4.  Invoice & Payment";
            this.btnInvoicePayment.Font      = new Font("Arial", 10);
            this.btnInvoicePayment.TextAlign = ContentAlignment.MiddleLeft;
            this.btnInvoicePayment.Location  = new Point(80, 234);
            this.btnInvoicePayment.Size      = new Size(320, 38);
            this.btnInvoicePayment.Click    += new EventHandler(this.btnInvoicePayment_Click);

            this.btnUpdateProfile.Text      = "5.  Update My Profile";
            this.btnUpdateProfile.Font      = new Font("Arial", 10);
            this.btnUpdateProfile.TextAlign = ContentAlignment.MiddleLeft;
            this.btnUpdateProfile.Location  = new Point(80, 282);
            this.btnUpdateProfile.Size      = new Size(320, 38);
            this.btnUpdateProfile.Click    += new EventHandler(this.btnUpdateProfile_Click);

            // Separator before logout
            Panel line2 = new Panel();
            line2.BorderStyle = BorderStyle.FixedSingle;
            line2.Location    = new Point(10, 335);
            line2.Size        = new Size(463, 1);

            // Logout button
            this.btnLogout.Text     = "Logout";
            this.btnLogout.Font     = new Font("Arial", 9);
            this.btnLogout.Location = new Point(370, 348);
            this.btnLogout.Size     = new Size(100, 28);
            this.btnLogout.Click   += new EventHandler(this.btnLogout_Click);

            // Clock timer - 1 second interval
            this.tmrClock.Interval = 1000;
            this.tmrClock.Enabled  = true;
            this.tmrClock.Tick    += new EventHandler(this.tmrClock_Tick);

            this.Controls.Add(this.lblTitle);
            this.Controls.Add(line1);
            this.Controls.Add(this.lblWelcome);
            this.Controls.Add(this.lblDateTime);
            this.Controls.Add(this.btnViewSchedule);
            this.Controls.Add(this.btnSendRequest);
            this.Controls.Add(this.btnManageRequests);
            this.Controls.Add(this.btnInvoicePayment);
            this.Controls.Add(this.btnUpdateProfile);
            this.Controls.Add(line2);
            this.Controls.Add(this.btnLogout);

            this.Load += new EventHandler(this.frmStudentDashboard_Load);
            this.ResumeLayout(false);
        }
    }
}
