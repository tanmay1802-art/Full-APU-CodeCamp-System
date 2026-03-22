using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace APUCodeCamp
{
    // Feature B: Send a coaching enrolment request
    public class frmSendRequest : Form
    {
        private Label    lblTitle;
        private Label    lblStudentInfo;
        private Label    lblModule;
        private Label    lblClassLevel;
        private Label    lblLecturer;
        private Label    lblReason;
        private Label    lblNote;
        private ComboBox cboModule;
        private ComboBox cboClassLevel;
        private ComboBox cboLecturer;
        private TextBox  txtReason;
        private Button   btnSubmit;
        private Button   btnClear;
        private Button   btnClose;

        public frmSendRequest()
        {
            InitializeComponent();
        }

        private void frmSendRequest_Load(object sender, EventArgs e)
        {
            lblStudentInfo.Text = "Student: " + StudentSession.Name +
                                  "   TP: " + StudentSession.TPNumber;
            LoadModules();
            LoadLecturers();
        }

        private void LoadModules()
        {
            DataTable dt = DatabaseHelper.ExecuteQuery(
                "SELECT ModuleID, ModuleCode + ' - ' + ModuleName AS ModuleDisplay " +
                "FROM Modules ORDER BY ModuleCode");
            cboModule.DataSource    = dt;
            cboModule.DisplayMember = "ModuleDisplay";
            cboModule.ValueMember   = "ModuleID";
            cboModule.SelectedIndex = -1;
        }

        private void LoadLecturers()
        {
            DataTable dt = DatabaseHelper.ExecuteQuery(
                "SELECT l.LecturerID, u.Name + ' (' + l.StaffID + ')' AS LecturerDisplay " +
                "FROM Lecturers l INNER JOIN Users u ON l.UserID = u.UserID ORDER BY u.Name");
            cboLecturer.DataSource    = dt;
            cboLecturer.DisplayMember = "LecturerDisplay";
            cboLecturer.ValueMember   = "LecturerID";
            cboLecturer.SelectedIndex = -1;
        }

        private void btnSubmit_Click(object sender, EventArgs e)
        {
            // Validation
            if (cboModule.SelectedIndex < 0)
            { MessageBox.Show("Please select a module.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }
            if (cboClassLevel.SelectedIndex < 0)
            { MessageBox.Show("Please select a class level.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }
            if (cboLecturer.SelectedIndex < 0)
            { MessageBox.Show("Please select a lecturer.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }
            if (txtReason.Text.Trim().Length < 10)
            { MessageBox.Show("Please enter a reason (at least 10 characters).", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }

            int    moduleID   = Convert.ToInt32(cboModule.SelectedValue);
            int    lecturerID = Convert.ToInt32(cboLecturer.SelectedValue);
            string level      = cboClassLevel.SelectedItem.ToString();

            // Check for duplicate pending request
            string checkQuery =
                "SELECT COUNT(*) FROM CoachingRequests " +
                "WHERE StudentID = @StudentID AND ModuleID = @ModuleID AND Status = 'Pending'";
            SqlParameter[] checkParams =
            {
                new SqlParameter("@StudentID", StudentSession.StudentID),
                new SqlParameter("@ModuleID",  moduleID)
            };
            int existing = Convert.ToInt32(DatabaseHelper.ExecuteScalar(checkQuery, checkParams));
            if (existing > 0)
            { MessageBox.Show("You already have a pending request for this module.", "Duplicate", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }

            if (MessageBox.Show(
                "Submit this request?\n\nModule  : " + cboModule.Text +
                "\nLevel   : " + level +
                "\nLecturer: " + cboLecturer.Text,
                "Confirm Submit", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                return;

            string insertQuery =
                "INSERT INTO CoachingRequests (StudentID, LecturerID, ModuleID, ClassLevel, Reason) " +
                "VALUES (@StudentID, @LecturerID, @ModuleID, @ClassLevel, @Reason)";
            SqlParameter[] insertParams =
            {
                new SqlParameter("@StudentID",  StudentSession.StudentID),
                new SqlParameter("@LecturerID", lecturerID),
                new SqlParameter("@ModuleID",   moduleID),
                new SqlParameter("@ClassLevel", level),
                new SqlParameter("@Reason",     txtReason.Text.Trim())
            };

            int rows = DatabaseHelper.ExecuteNonQuery(insertQuery, insertParams);
            if (rows > 0)
            {
                MessageBox.Show("Request submitted successfully!", "Success",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                ClearForm();
            }
            else
            {
                MessageBox.Show("Failed to submit. Please try again.", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnClear_Click(object sender, EventArgs e) { ClearForm(); }
        private void btnClose_Click(object sender, EventArgs e) { this.Close(); }

        private void ClearForm()
        {
            cboModule.SelectedIndex     = -1;
            cboClassLevel.SelectedIndex = -1;
            cboLecturer.SelectedIndex   = -1;
            txtReason.Clear();
        }

        private void InitializeComponent()
        {
            this.lblTitle       = new Label();
            this.lblStudentInfo = new Label();
            this.lblModule      = new Label();
            this.lblClassLevel  = new Label();
            this.lblLecturer    = new Label();
            this.lblReason      = new Label();
            this.lblNote        = new Label();
            this.cboModule      = new ComboBox();
            this.cboClassLevel  = new ComboBox();
            this.cboLecturer    = new ComboBox();
            this.txtReason      = new TextBox();
            this.btnSubmit      = new Button();
            this.btnClear       = new Button();
            this.btnClose       = new Button();

            this.SuspendLayout();

            // Form
            this.Text            = "Send Enrolment Request";
            this.Size            = new Size(500, 430);
            this.StartPosition   = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox     = false;

            // Title
            this.lblTitle.Text      = "Send Coaching Enrolment Request";
            this.lblTitle.Font      = new Font("Arial", 11, FontStyle.Bold);
            this.lblTitle.Location  = new Point(10, 10);
            this.lblTitle.Size      = new Size(465, 22);
            this.lblTitle.TextAlign = ContentAlignment.MiddleCenter;

            Panel line = new Panel();
            line.BorderStyle = BorderStyle.FixedSingle;
            line.Location    = new Point(10, 35);
            line.Size        = new Size(462, 1);

            // Student info
            this.lblStudentInfo.Font     = new Font("Arial", 8);
            this.lblStudentInfo.Location = new Point(10, 40);
            this.lblStudentInfo.Size     = new Size(465, 18);

            // Row layout: label on left, control on right
            int lblLeft  = 15;
            int ctrlLeft = 140;
            int ctrlW    = 325;

            // Module
            this.lblModule.Text     = "Module:";
            this.lblModule.Font     = new Font("Arial", 9);
            this.lblModule.Location = new Point(lblLeft, 75);
            this.lblModule.Size     = new Size(120, 22);

            this.cboModule.Font          = new Font("Arial", 9);
            this.cboModule.Location      = new Point(ctrlLeft, 73);
            this.cboModule.Size          = new Size(ctrlW, 22);
            this.cboModule.DropDownStyle = ComboBoxStyle.DropDownList;

            // Class Level
            this.lblClassLevel.Text     = "Class Level:";
            this.lblClassLevel.Font     = new Font("Arial", 9);
            this.lblClassLevel.Location = new Point(lblLeft, 110);
            this.lblClassLevel.Size     = new Size(120, 22);

            this.cboClassLevel.Font          = new Font("Arial", 9);
            this.cboClassLevel.Location      = new Point(ctrlLeft, 108);
            this.cboClassLevel.Size          = new Size(ctrlW, 22);
            this.cboClassLevel.DropDownStyle = ComboBoxStyle.DropDownList;
            this.cboClassLevel.Items.AddRange(new object[] { "Beginner", "Intermediate", "Advance" });

            // Lecturer
            this.lblLecturer.Text     = "Lecturer:";
            this.lblLecturer.Font     = new Font("Arial", 9);
            this.lblLecturer.Location = new Point(lblLeft, 145);
            this.lblLecturer.Size     = new Size(120, 22);

            this.cboLecturer.Font          = new Font("Arial", 9);
            this.cboLecturer.Location      = new Point(ctrlLeft, 143);
            this.cboLecturer.Size          = new Size(ctrlW, 22);
            this.cboLecturer.DropDownStyle = ComboBoxStyle.DropDownList;

            // Reason
            this.lblReason.Text     = "Reason:";
            this.lblReason.Font     = new Font("Arial", 9);
            this.lblReason.Location = new Point(lblLeft, 180);
            this.lblReason.Size     = new Size(120, 22);

            this.txtReason.Font       = new Font("Arial", 9);
            this.txtReason.Location   = new Point(ctrlLeft, 178);
            this.txtReason.Size       = new Size(ctrlW, 80);
            this.txtReason.Multiline  = true;
            this.txtReason.ScrollBars = ScrollBars.Vertical;

            // Note
            this.lblNote.Text      = "Note: Only one pending request per module is allowed at a time.";
            this.lblNote.Font      = new Font("Arial", 8, FontStyle.Italic);
            this.lblNote.Location  = new Point(15, 270);
            this.lblNote.Size      = new Size(460, 18);

            Panel line2 = new Panel();
            line2.BorderStyle = BorderStyle.FixedSingle;
            line2.Location    = new Point(10, 293);
            line2.Size        = new Size(462, 1);

            // Buttons
            this.btnSubmit.Text     = "Submit Request";
            this.btnSubmit.Font     = new Font("Arial", 9);
            this.btnSubmit.Location = new Point(15, 305);
            this.btnSubmit.Size     = new Size(120, 28);
            this.btnSubmit.Click   += new EventHandler(this.btnSubmit_Click);

            this.btnClear.Text     = "Clear";
            this.btnClear.Font     = new Font("Arial", 9);
            this.btnClear.Location = new Point(150, 305);
            this.btnClear.Size     = new Size(80, 28);
            this.btnClear.Click   += new EventHandler(this.btnClear_Click);

            this.btnClose.Text     = "Close";
            this.btnClose.Font     = new Font("Arial", 9);
            this.btnClose.Location = new Point(385, 305);
            this.btnClose.Size     = new Size(80, 28);
            this.btnClose.Click   += new EventHandler(this.btnClose_Click);

            this.Controls.Add(this.lblTitle);
            this.Controls.Add(line);
            this.Controls.Add(this.lblStudentInfo);
            this.Controls.Add(this.lblModule);
            this.Controls.Add(this.cboModule);
            this.Controls.Add(this.lblClassLevel);
            this.Controls.Add(this.cboClassLevel);
            this.Controls.Add(this.lblLecturer);
            this.Controls.Add(this.cboLecturer);
            this.Controls.Add(this.lblReason);
            this.Controls.Add(this.txtReason);
            this.Controls.Add(this.lblNote);
            this.Controls.Add(line2);
            this.Controls.Add(this.btnSubmit);
            this.Controls.Add(this.btnClear);
            this.Controls.Add(this.btnClose);

            this.Load += new EventHandler(this.frmSendRequest_Load);
            this.ResumeLayout(false);
        }
    }
}
