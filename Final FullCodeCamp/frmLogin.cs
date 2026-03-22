using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace APUCodeCamp
{
    // Unified login form – routes each role to its own dashboard
    public class frmLogin : Form
    {
        private Label    lblTitle;
        private Label    lblSubtitle;
        private Label    lblUsername;
        private Label    lblPassword;
        private TextBox  txtUsername;
        private TextBox  txtPassword;
        private CheckBox chkShowPassword;
        private Button   btnLogin;
        private Button   btnClear;
        private Button   btnExit;
        private Label    lblDemo;

        public frmLogin()
        {
            InitializeComponent();
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Text.Trim();

            if (username == "")
            {
                MessageBox.Show("Please enter your username.", "Validation",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtUsername.Focus();
                return;
            }
            if (password == "")
            {
                MessageBox.Show("Please enter your password.", "Validation",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtPassword.Focus();
                return;
            }

            string query =
                "SELECT UserID, Role, Name, Email, Phone, Address " +
                "FROM Users " +
                "WHERE Username = @Username AND Password = @Password AND IsActive = 1";

            SqlParameter[] parameters =
            {
                new SqlParameter("@Username", username),
                new SqlParameter("@Password", password)
            };

            DataTable dt = DatabaseHelper.ExecuteQuery(query, parameters);

            if (dt.Rows.Count == 0)
            {
                MessageBox.Show("Wrong username or password.", "Login Failed",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtPassword.Clear();
                txtPassword.Focus();
                return;
            }

            DataRow row  = dt.Rows[0];
            string  role = row["Role"].ToString();
            int  userID  = Convert.ToInt32(row["UserID"]);

            // Populate common session fields
            UserSession.UserID   = userID;
            UserSession.Role     = role;
            UserSession.Name     = row["Name"].ToString();
            UserSession.Email    = row["Email"].ToString();
            UserSession.Phone    = row["Phone"].ToString();
            UserSession.Address  = row["Address"].ToString();
            UserSession.Username = username;

            // Route to the correct dashboard
            switch (role)
            {
                case "Student":
                    LoadStudentSession(userID);
                    new frmStudentDashboard().Show();
                    break;

                case "Lecturer":
                    LoadLecturerSession(userID);
                    new frmLecturerDashboard().Show();
                    break;

                case "Trainer":
                    LoadTrainerSession(userID);
                    new frmTrainerDashboard().Show();
                    break;

                case "Administrator":
                    LoadAdminSession(userID);
                    new frmAdminDashboard().Show();
                    break;

                default:
                    MessageBox.Show("Unknown role: " + role, "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
            }

            this.Hide();
        }

        // ── Session loaders ──────────────────────────────────────────

        private void LoadStudentSession(int userID)
        {
            string query = "SELECT StudentID, TPNumber, StudyLevel FROM Students WHERE UserID = @UserID";
            SqlParameter[] p = { new SqlParameter("@UserID", userID) };
            DataTable dt = DatabaseHelper.ExecuteQuery(query, p);
            if (dt.Rows.Count > 0)
            {
                UserSession.StudentID  = Convert.ToInt32(dt.Rows[0]["StudentID"]);
                UserSession.TPNumber   = dt.Rows[0]["TPNumber"].ToString();
                UserSession.StudyLevel = dt.Rows[0]["StudyLevel"].ToString();
            }
        }

        private void LoadLecturerSession(int userID)
        {
            string query = "SELECT LecturerID, StaffID FROM Lecturers WHERE UserID = @UserID";
            SqlParameter[] p = { new SqlParameter("@UserID", userID) };
            DataTable dt = DatabaseHelper.ExecuteQuery(query, p);
            if (dt.Rows.Count > 0)
            {
                UserSession.LecturerID     = Convert.ToInt32(dt.Rows[0]["LecturerID"]);
                UserSession.LecturerStaffID= dt.Rows[0]["StaffID"].ToString();
            }
        }

        private void LoadTrainerSession(int userID)
        {
            string query = "SELECT TrainerID, StaffID, Specialisation FROM Trainers WHERE UserID = @UserID";
            SqlParameter[] p = { new SqlParameter("@UserID", userID) };
            DataTable dt = DatabaseHelper.ExecuteQuery(query, p);
            if (dt.Rows.Count > 0)
            {
                UserSession.TrainerID            = Convert.ToInt32(dt.Rows[0]["TrainerID"]);
                UserSession.TrainerStaffID       = dt.Rows[0]["StaffID"].ToString();
                UserSession.TrainerSpecialisation= dt.Rows[0]["Specialisation"].ToString();
            }
        }

        private void LoadAdminSession(int userID)
        {
            string query = "SELECT AdminID, StaffID FROM Administrators WHERE UserID = @UserID";
            SqlParameter[] p = { new SqlParameter("@UserID", userID) };
            DataTable dt = DatabaseHelper.ExecuteQuery(query, p);
            if (dt.Rows.Count > 0)
            {
                UserSession.AdminID     = Convert.ToInt32(dt.Rows[0]["AdminID"]);
                UserSession.AdminStaffID= dt.Rows[0]["StaffID"].ToString();
            }
        }

        // ── Button handlers ──────────────────────────────────────────

        private void btnClear_Click(object sender, EventArgs e)
        {
            txtUsername.Clear();
            txtPassword.Clear();
            txtUsername.Focus();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to exit?", "Confirm Exit",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                Application.Exit();
        }

        private void chkShowPassword_CheckedChanged(object sender, EventArgs e)
        {
            txtPassword.UseSystemPasswordChar = !chkShowPassword.Checked;
        }

        private void InitializeComponent()
        {
            this.lblTitle        = new Label();
            this.lblSubtitle     = new Label();
            this.lblUsername     = new Label();
            this.lblPassword     = new Label();
            this.txtUsername     = new TextBox();
            this.txtPassword     = new TextBox();
            this.chkShowPassword = new CheckBox();
            this.btnLogin        = new Button();
            this.btnClear        = new Button();
            this.btnExit         = new Button();
            this.lblDemo         = new Label();

            this.SuspendLayout();

            // Form
            this.Text            = "APU CodeCamp - Login";
            this.Size            = new Size(420, 380);
            this.StartPosition   = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox     = false;
            this.BackColor       = System.Drawing.Color.White;

            // Title
            this.lblTitle.Text      = "APU CodeCamp Management System";
            this.lblTitle.Font      = new Font("Arial", 12, FontStyle.Bold);
            this.lblTitle.Location  = new Point(10, 18);
            this.lblTitle.Size      = new Size(385, 26);
            this.lblTitle.TextAlign = ContentAlignment.MiddleCenter;

            // Subtitle
            this.lblSubtitle.Text      = "School of Computing – Coding Coaching Programme";
            this.lblSubtitle.Font      = new Font("Arial", 8, FontStyle.Italic);
            this.lblSubtitle.Location  = new Point(10, 48);
            this.lblSubtitle.Size      = new Size(385, 18);
            this.lblSubtitle.TextAlign = ContentAlignment.MiddleCenter;
            this.lblSubtitle.ForeColor = System.Drawing.Color.Gray;

            Panel line = new Panel();
            line.BorderStyle = BorderStyle.FixedSingle;
            line.Location    = new Point(10, 70);
            line.Size        = new Size(382, 1);

            // Username
            this.lblUsername.Text     = "Username:";
            this.lblUsername.Font     = new Font("Arial", 9);
            this.lblUsername.Location = new Point(50, 90);
            this.lblUsername.Size     = new Size(80, 20);

            this.txtUsername.Font     = new Font("Arial", 9);
            this.txtUsername.Location = new Point(145, 88);
            this.txtUsername.Size     = new Size(210, 22);

            // Password
            this.lblPassword.Text     = "Password:";
            this.lblPassword.Font     = new Font("Arial", 9);
            this.lblPassword.Location = new Point(50, 125);
            this.lblPassword.Size     = new Size(80, 20);

            this.txtPassword.Font                  = new Font("Arial", 9);
            this.txtPassword.Location              = new Point(145, 123);
            this.txtPassword.Size                  = new Size(210, 22);
            this.txtPassword.UseSystemPasswordChar = true;

            // Show password
            this.chkShowPassword.Text            = "Show Password";
            this.chkShowPassword.Font            = new Font("Arial", 8);
            this.chkShowPassword.Location        = new Point(145, 152);
            this.chkShowPassword.Size            = new Size(120, 20);
            this.chkShowPassword.CheckedChanged += new EventHandler(this.chkShowPassword_CheckedChanged);

            Panel line2 = new Panel();
            line2.BorderStyle = BorderStyle.FixedSingle;
            line2.Location    = new Point(10, 183);
            line2.Size        = new Size(382, 1);

            // Buttons
            this.btnLogin.Text     = "Login";
            this.btnLogin.Font     = new Font("Arial", 9, FontStyle.Bold);
            this.btnLogin.Location = new Point(50, 198);
            this.btnLogin.Size     = new Size(100, 30);
            this.btnLogin.BackColor= System.Drawing.Color.SteelBlue;
            this.btnLogin.ForeColor= System.Drawing.Color.White;
            this.btnLogin.FlatStyle= FlatStyle.Flat;
            this.btnLogin.Click   += new EventHandler(this.btnLogin_Click);

            this.btnClear.Text     = "Clear";
            this.btnClear.Font     = new Font("Arial", 9);
            this.btnClear.Location = new Point(165, 198);
            this.btnClear.Size     = new Size(80, 30);
            this.btnClear.Click   += new EventHandler(this.btnClear_Click);

            this.btnExit.Text     = "Exit";
            this.btnExit.Font     = new Font("Arial", 9);
            this.btnExit.Location = new Point(295, 198);
            this.btnExit.Size     = new Size(70, 30);
            this.btnExit.Click   += new EventHandler(this.btnExit_Click);

            // Demo credentials hint
            this.lblDemo.Text =
                "Demo Accounts:\r\n" +
                "  Admin:    admin01 / admin123\r\n" +
                "  Lecturer: lect01  / lect123\r\n" +
                "  Trainer:  train01 / train123\r\n" +
                "  Student:  tp001   / student123";
            this.lblDemo.Font      = new Font("Courier New", 8);
            this.lblDemo.ForeColor = System.Drawing.Color.DimGray;
            this.lblDemo.Location  = new Point(50, 245);
            this.lblDemo.Size      = new Size(320, 90);
            this.lblDemo.BorderStyle = BorderStyle.FixedSingle;

            this.Controls.Add(this.lblTitle);
            this.Controls.Add(this.lblSubtitle);
            this.Controls.Add(line);
            this.Controls.Add(this.lblUsername);
            this.Controls.Add(this.lblPassword);
            this.Controls.Add(this.txtUsername);
            this.Controls.Add(this.txtPassword);
            this.Controls.Add(this.chkShowPassword);
            this.Controls.Add(line2);
            this.Controls.Add(this.btnLogin);
            this.Controls.Add(this.btnClear);
            this.Controls.Add(this.btnExit);
            this.Controls.Add(this.lblDemo);

            this.AcceptButton = this.btnLogin;
            this.ResumeLayout(false);
        }
    }
}
