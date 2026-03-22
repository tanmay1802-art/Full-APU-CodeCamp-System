using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace APUCodeCamp
{
    // Lecturer Feature A: Register and enrol student into a coaching class
    public class frmEnrolStudent : Form
    {
        private Label    lblTitle;
        private Label    lblSec1;
        private Label    lblSec2;
        private Label    lblTPNumber;
        private Label    lblStudentName;
        private Label    lblEmail;
        private Label    lblPhone;
        private Label    lblAddress;
        private Label    lblStudyLevel;
        private Label    lblUsername;
        private Label    lblPassword;
        private Label    lblModule;
        private Label    lblClassLevel;
        private Label    lblMonth;
        private TextBox  txtTPNumber;
        private TextBox  txtStudentName;
        private TextBox  txtEmail;
        private TextBox  txtPhone;
        private TextBox  txtAddress;
        private TextBox  txtUsername;
        private TextBox  txtPassword;
        private ComboBox cboStudyLevel;
        private ComboBox cboModule;
        private ComboBox cboClassLevel;
        private ComboBox cboMonth;
        private Button   btnEnrol;
        private Button   btnClear;
        private Button   btnClose;

        public frmEnrolStudent()
        {
            InitializeComponent();
        }

        private void frmEnrolStudent_Load(object sender, EventArgs e)
        {
            LoadModules();
            LoadMonths();
        }

        private void LoadModules()
        {
            DataTable dt = DatabaseHelper.ExecuteQuery(
                "SELECT ModuleID, ModuleCode + ' - ' + ModuleName AS Display " +
                "FROM Modules WHERE IsActive = 1 ORDER BY ModuleCode");
            cboModule.DataSource    = dt;
            cboModule.DisplayMember = "Display";
            cboModule.ValueMember   = "ModuleID";
            cboModule.SelectedIndex = -1;
        }

        private void LoadMonths()
        {
            string[] months = { "January","February","March","April","May","June",
                                 "July","August","September","October","November","December" };
            int curMonth = DateTime.Now.Month;
            int curYear  = DateTime.Now.Year;
            for (int i = 0; i < 6; i++)
            {
                int m = ((curMonth - 1 + i) % 12) + 1;
                int y = curYear + ((curMonth - 1 + i) / 12);
                cboMonth.Items.Add(months[m - 1] + " " + y);
            }
            cboMonth.SelectedIndex = 0;
        }

        private void btnEnrol_Click(object sender, EventArgs e)
        {
            // Validate required student fields
            if (txtTPNumber.Text.Trim() == "" || txtStudentName.Text.Trim() == "" ||
                txtEmail.Text.Trim() == ""    || txtPhone.Text.Trim() == "" ||
                txtUsername.Text.Trim() == "" || txtPassword.Text.Trim() == "")
            {
                MessageBox.Show("Please fill in all required student fields.", "Validation",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (!txtEmail.Text.Contains("@"))
            {
                MessageBox.Show("Please enter a valid email address.", "Validation",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (cboModule.SelectedIndex < 0 || cboClassLevel.SelectedIndex < 0)
            {
                MessageBox.Show("Please select Module and Class Level.", "Validation",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string tp       = txtTPNumber.Text.Trim().ToUpper();
            string username = txtUsername.Text.Trim();

            // Check if student TP already exists
            string chkTP = "SELECT COUNT(*) FROM Students WHERE TPNumber = @TP";
            SqlParameter[] pTP = { new SqlParameter("@TP", tp) };
            bool studentExists = Convert.ToInt32(DatabaseHelper.ExecuteScalar(chkTP, pTP)) > 0;

            int studentID = 0;

            if (studentExists)
            {
                // Get existing StudentID
                string getID = "SELECT StudentID FROM Students WHERE TPNumber = @TP";
                object result = DatabaseHelper.ExecuteScalar(getID, pTP);
                studentID = Convert.ToInt32(result);
                MessageBox.Show("Student " + tp + " already exists. Proceeding to enrolment.",
                    "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                // Check username uniqueness
                string chkUser = "SELECT COUNT(*) FROM Users WHERE Username = @U";
                SqlParameter[] pU = { new SqlParameter("@U", username) };
                if (Convert.ToInt32(DatabaseHelper.ExecuteScalar(chkUser, pU)) > 0)
                {
                    MessageBox.Show("Username already taken. Please choose another.", "Duplicate",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Insert into Users
                string insUser =
                    "INSERT INTO Users (Username, Password, Role, Name, Email, Phone, Address) " +
                    "OUTPUT INSERTED.UserID " +
                    "VALUES (@Username, @Password, 'Student', @Name, @Email, @Phone, @Address)";
                SqlParameter[] pUser =
                {
                    new SqlParameter("@Username", username),
                    new SqlParameter("@Password", txtPassword.Text.Trim()),
                    new SqlParameter("@Name",     txtStudentName.Text.Trim()),
                    new SqlParameter("@Email",    txtEmail.Text.Trim()),
                    new SqlParameter("@Phone",    txtPhone.Text.Trim()),
                    new SqlParameter("@Address",  txtAddress.Text.Trim() == "" ? "N/A" : txtAddress.Text.Trim())
                };
                object newUID = DatabaseHelper.ExecuteScalar(insUser, pUser);
                if (newUID == null) { MessageBox.Show("Failed to create user.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

                // Insert into Students
                string insStudent =
                    "INSERT INTO Students (UserID, TPNumber, StudyLevel) " +
                    "OUTPUT INSERTED.StudentID " +
                    "VALUES (@UserID, @TP, @Level)";
                SqlParameter[] pStu =
                {
                    new SqlParameter("@UserID", Convert.ToInt32(newUID)),
                    new SqlParameter("@TP",     tp),
                    new SqlParameter("@Level",  cboStudyLevel.SelectedItem.ToString())
                };
                object newSID = DatabaseHelper.ExecuteScalar(insStudent, pStu);
                if (newSID == null) { MessageBox.Show("Failed to create student.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }
                studentID = Convert.ToInt32(newSID);
            }

            // Find matching active class
            int    moduleID = Convert.ToInt32(cboModule.SelectedValue);
            string level    = cboClassLevel.SelectedItem.ToString();

            string findClass =
                "SELECT TOP 1 ClassID FROM Classes " +
                "WHERE ModuleID = @ModuleID AND ClassLevel = @Level AND IsActive = 1 " +
                "ORDER BY StartDate";
            SqlParameter[] pClass =
            {
                new SqlParameter("@ModuleID", moduleID),
                new SqlParameter("@Level",    level)
            };
            object classObj = DatabaseHelper.ExecuteScalar(findClass, pClass);

            if (classObj == null)
            {
                MessageBox.Show("No active class found for this module and level.\n" +
                    "Please ask a trainer to create the class first.", "No Class Found",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            int classID = Convert.ToInt32(classObj);

            // Check duplicate enrolment
            string chkEnrol = "SELECT COUNT(*) FROM Enrolments WHERE StudentID = @S AND ClassID = @C";
            SqlParameter[] pEnrol =
            {
                new SqlParameter("@S", studentID),
                new SqlParameter("@C", classID)
            };
            if (Convert.ToInt32(DatabaseHelper.ExecuteScalar(chkEnrol, pEnrol)) > 0)
            {
                MessageBox.Show("Student is already enrolled in this class.", "Duplicate",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Insert Enrolment
            string insEnrol =
                "INSERT INTO Enrolments (StudentID, ClassID, MonthOfEnrolment) " +
                "VALUES (@StudentID, @ClassID, @Month)";
            SqlParameter[] pIns =
            {
                new SqlParameter("@StudentID", studentID),
                new SqlParameter("@ClassID",   classID),
                new SqlParameter("@Month",     cboMonth.SelectedItem.ToString())
            };

            if (DatabaseHelper.ExecuteNonQuery(insEnrol, pIns) > 0)
            {
                MessageBox.Show(
                    "Student enrolled successfully!\n\n" +
                    "TP Number : " + tp + "\n" +
                    "Module    : " + cboModule.Text + "\n" +
                    "Level     : " + level + "\n" +
                    "Month     : " + cboMonth.SelectedItem.ToString(),
                    "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ClearForm();
            }
            else
            {
                MessageBox.Show("Enrolment failed. Please try again.", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ClearForm()
        {
            txtTPNumber.Clear();
            txtStudentName.Clear();
            txtEmail.Clear();
            txtPhone.Clear();
            txtAddress.Clear();
            txtUsername.Clear();
            txtPassword.Clear();
            cboStudyLevel.SelectedIndex = 0;
            cboModule.SelectedIndex     = -1;
            cboClassLevel.SelectedIndex = -1;
        }

        private void btnClear_Click(object sender, EventArgs e) { ClearForm(); }
        private void btnClose_Click(object sender, EventArgs e) { this.Close(); }

        private void InitializeComponent()
        {
            this.lblTitle       = new Label();
            this.lblSec1        = new Label();
            this.lblSec2        = new Label();
            this.lblTPNumber    = new Label();
            this.txtTPNumber    = new TextBox();
            this.lblStudentName = new Label();
            this.txtStudentName = new TextBox();
            this.lblEmail       = new Label();
            this.txtEmail       = new TextBox();
            this.lblPhone       = new Label();
            this.txtPhone       = new TextBox();
            this.lblAddress     = new Label();
            this.txtAddress     = new TextBox();
            this.lblStudyLevel  = new Label();
            this.cboStudyLevel  = new ComboBox();
            this.lblUsername    = new Label();
            this.txtUsername    = new TextBox();
            this.lblPassword    = new Label();
            this.txtPassword    = new TextBox();
            this.lblModule      = new Label();
            this.cboModule      = new ComboBox();
            this.lblClassLevel  = new Label();
            this.cboClassLevel  = new ComboBox();
            this.lblMonth       = new Label();
            this.cboMonth       = new ComboBox();
            this.btnEnrol       = new Button();
            this.btnClear       = new Button();
            this.btnClose       = new Button();

            this.SuspendLayout();

            // ── Form ──
            this.Text            = "Register & Enrol Student";
            this.Size            = new Size(560, 620);
            this.StartPosition   = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox     = false;

            // ── Title ──
            this.lblTitle.Text      = "Register & Enrol Student into Coaching Class";
            this.lblTitle.Font      = new Font("Arial", 11, FontStyle.Bold);
            this.lblTitle.Location  = new Point(10, 10);
            this.lblTitle.Size      = new Size(525, 22);
            this.lblTitle.TextAlign = ContentAlignment.MiddleCenter;

            // ── Section 1 header ──
            this.lblSec1.Text      = "Student Information";
            this.lblSec1.Font      = new Font("Arial", 9, FontStyle.Bold);
            this.lblSec1.Location  = new Point(15, 40);
            this.lblSec1.Size      = new Size(200, 18);

            // ── TP Number ──
            this.lblTPNumber.Text     = "TP Number: *";
            this.lblTPNumber.Font     = new Font("Arial", 9);
            this.lblTPNumber.Location = new Point(15, 65);
            this.lblTPNumber.Size     = new Size(140, 22);
            this.txtTPNumber.Font     = new Font("Arial", 9);
            this.txtTPNumber.Location = new Point(165, 63);
            this.txtTPNumber.Size     = new Size(350, 22);

            // ── Full Name ──
            this.lblStudentName.Text     = "Full Name: *";
            this.lblStudentName.Font     = new Font("Arial", 9);
            this.lblStudentName.Location = new Point(15, 97);
            this.lblStudentName.Size     = new Size(140, 22);
            this.txtStudentName.Font     = new Font("Arial", 9);
            this.txtStudentName.Location = new Point(165, 95);
            this.txtStudentName.Size     = new Size(350, 22);

            // ── Email ──
            this.lblEmail.Text     = "Email: *";
            this.lblEmail.Font     = new Font("Arial", 9);
            this.lblEmail.Location = new Point(15, 129);
            this.lblEmail.Size     = new Size(140, 22);
            this.txtEmail.Font     = new Font("Arial", 9);
            this.txtEmail.Location = new Point(165, 127);
            this.txtEmail.Size     = new Size(350, 22);

            // ── Phone ──
            this.lblPhone.Text     = "Phone: *";
            this.lblPhone.Font     = new Font("Arial", 9);
            this.lblPhone.Location = new Point(15, 161);
            this.lblPhone.Size     = new Size(140, 22);
            this.txtPhone.Font     = new Font("Arial", 9);
            this.txtPhone.Location = new Point(165, 159);
            this.txtPhone.Size     = new Size(350, 22);

            // ── Address ──
            this.lblAddress.Text     = "Address:";
            this.lblAddress.Font     = new Font("Arial", 9);
            this.lblAddress.Location = new Point(15, 193);
            this.lblAddress.Size     = new Size(140, 22);
            this.txtAddress.Font     = new Font("Arial", 9);
            this.txtAddress.Location = new Point(165, 191);
            this.txtAddress.Size     = new Size(350, 22);

            // ── Username ──
            this.lblUsername.Text     = "Username: *";
            this.lblUsername.Font     = new Font("Arial", 9);
            this.lblUsername.Location = new Point(15, 225);
            this.lblUsername.Size     = new Size(140, 22);
            this.txtUsername.Font     = new Font("Arial", 9);
            this.txtUsername.Location = new Point(165, 223);
            this.txtUsername.Size     = new Size(350, 22);

            // ── Password ──
            this.lblPassword.Text     = "Password: *";
            this.lblPassword.Font     = new Font("Arial", 9);
            this.lblPassword.Location = new Point(15, 257);
            this.lblPassword.Size     = new Size(140, 22);
            this.txtPassword.Font                  = new Font("Arial", 9);
            this.txtPassword.Location              = new Point(165, 255);
            this.txtPassword.Size                  = new Size(350, 22);
            this.txtPassword.UseSystemPasswordChar = true;

            // ── Study Level ──
            this.lblStudyLevel.Text          = "Study Level:";
            this.lblStudyLevel.Font          = new Font("Arial", 9);
            this.lblStudyLevel.Location      = new Point(15, 289);
            this.lblStudyLevel.Size          = new Size(140, 22);
            this.cboStudyLevel.Font          = new Font("Arial", 9);
            this.cboStudyLevel.DropDownStyle = ComboBoxStyle.DropDownList;
            this.cboStudyLevel.Location      = new Point(165, 287);
            this.cboStudyLevel.Size          = new Size(180, 22);
            this.cboStudyLevel.Items.AddRange(new object[] { "Foundation", "Level 1", "Level 2", "Level 3" });
            this.cboStudyLevel.SelectedIndex = 0;

            // ── Section 2 header ──
            this.lblSec2.Text      = "Enrolment Details";
            this.lblSec2.Font      = new Font("Arial", 9, FontStyle.Bold);
            this.lblSec2.Location  = new Point(15, 325);
            this.lblSec2.Size      = new Size(200, 18);

            // ── Module ──
            this.lblModule.Text          = "Module: *";
            this.lblModule.Font          = new Font("Arial", 9);
            this.lblModule.Location      = new Point(15, 350);
            this.lblModule.Size          = new Size(140, 22);
            this.cboModule.Font          = new Font("Arial", 9);
            this.cboModule.DropDownStyle = ComboBoxStyle.DropDownList;
            this.cboModule.Location      = new Point(165, 348);
            this.cboModule.Size          = new Size(350, 22);

            // ── Class Level ──
            this.lblClassLevel.Text          = "Class Level: *";
            this.lblClassLevel.Font          = new Font("Arial", 9);
            this.lblClassLevel.Location      = new Point(15, 382);
            this.lblClassLevel.Size          = new Size(140, 22);
            this.cboClassLevel.Font          = new Font("Arial", 9);
            this.cboClassLevel.DropDownStyle = ComboBoxStyle.DropDownList;
            this.cboClassLevel.Location      = new Point(165, 380);
            this.cboClassLevel.Size          = new Size(180, 22);
            this.cboClassLevel.Items.AddRange(new object[] { "Beginner", "Intermediate", "Advance" });

            // ── Month ──
            this.lblMonth.Text          = "Month of Enrolment:";
            this.lblMonth.Font          = new Font("Arial", 9);
            this.lblMonth.Location      = new Point(15, 414);
            this.lblMonth.Size          = new Size(140, 22);
            this.cboMonth.Font          = new Font("Arial", 9);
            this.cboMonth.DropDownStyle = ComboBoxStyle.DropDownList;
            this.cboMonth.Location      = new Point(165, 412);
            this.cboMonth.Size          = new Size(180, 22);

            // ── Buttons ──
            this.btnEnrol.Text     = "Enrol Student";
            this.btnEnrol.Font     = new Font("Arial", 9);
            this.btnEnrol.Location = new Point(15, 455);
            this.btnEnrol.Size     = new Size(110, 28);
            this.btnEnrol.Click   += new EventHandler(this.btnEnrol_Click);

            this.btnClear.Text     = "Clear";
            this.btnClear.Font     = new Font("Arial", 9);
            this.btnClear.Location = new Point(140, 455);
            this.btnClear.Size     = new Size(80, 28);
            this.btnClear.Click   += new EventHandler(this.btnClear_Click);

            this.btnClose.Text     = "Close";
            this.btnClose.Font     = new Font("Arial", 9);
            this.btnClose.Location = new Point(455, 455);
            this.btnClose.Size     = new Size(80, 28);
            this.btnClose.Click   += new EventHandler(this.btnClose_Click);

            // ── Add controls ──
            this.Controls.Add(this.lblTitle);
            this.Controls.Add(this.lblSec1);
            this.Controls.Add(this.lblTPNumber);
            this.Controls.Add(this.txtTPNumber);
            this.Controls.Add(this.lblStudentName);
            this.Controls.Add(this.txtStudentName);
            this.Controls.Add(this.lblEmail);
            this.Controls.Add(this.txtEmail);
            this.Controls.Add(this.lblPhone);
            this.Controls.Add(this.txtPhone);
            this.Controls.Add(this.lblAddress);
            this.Controls.Add(this.txtAddress);
            this.Controls.Add(this.lblUsername);
            this.Controls.Add(this.txtUsername);
            this.Controls.Add(this.lblPassword);
            this.Controls.Add(this.txtPassword);
            this.Controls.Add(this.lblStudyLevel);
            this.Controls.Add(this.cboStudyLevel);
            this.Controls.Add(this.lblSec2);
            this.Controls.Add(this.lblModule);
            this.Controls.Add(this.cboModule);
            this.Controls.Add(this.lblClassLevel);
            this.Controls.Add(this.cboClassLevel);
            this.Controls.Add(this.lblMonth);
            this.Controls.Add(this.cboMonth);
            this.Controls.Add(this.btnEnrol);
            this.Controls.Add(this.btnClear);
            this.Controls.Add(this.btnClose);

            this.Load += new EventHandler(this.frmEnrolStudent_Load);
            this.ResumeLayout(false);
        }
    }
}
