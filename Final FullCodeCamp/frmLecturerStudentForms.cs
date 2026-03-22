using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace APUCodeCamp
{
    // Lecturer Feature C: Delete student who completed coaching
    public class frmDeleteStudent : Form
    {
        private Label        lblTitle;
        private Label        lblInfo;
        private Label        lblFilter;
        private ComboBox     cboFilterLevel;
        private Label        lblCount;
        private DataGridView dgvStudents;
        private Button       btnDelete;
        private Button       btnRefresh;
        private Button       btnClose;

        public frmDeleteStudent()
        {
            InitializeComponent();
        }

        private void frmDeleteStudent_Load(object sender, EventArgs e)
        {
            lblInfo.Text = "Lecturer: " + UserSession.Name;
            LoadStudents();
        }

        private void LoadStudents()
        {
            string levelFilter = cboFilterLevel.SelectedIndex > 0
                ? " AND s.StudyLevel = '" + cboFilterLevel.SelectedItem.ToString() + "'"
                : "";

            // Show students enrolled in classes taught by this lecturer's assigned modules
            string query =
                "SELECT DISTINCT " +
                "    s.StudentID, u.Name AS StudentName, s.TPNumber, s.StudyLevel, " +
                "    u.Email, u.Phone " +
                "FROM Students s " +
                "INNER JOIN Users u ON s.UserID = u.UserID " +
                "INNER JOIN Enrolments e ON s.StudentID = e.StudentID " +
                "INNER JOIN Classes c ON e.ClassID = c.ClassID " +
                "WHERE u.IsActive = 1" + levelFilter +
                " ORDER BY u.Name";

            DataTable dt = DatabaseHelper.ExecuteQuery(query);
            dgvStudents.DataSource = null;
            dgvStudents.DataSource = dt;

            if (dgvStudents.Columns.Count > 0)
            {
                dgvStudents.Columns["StudentID"].Visible      = false;
                dgvStudents.Columns["StudentName"].HeaderText = "Student Name";
                dgvStudents.Columns["TPNumber"].HeaderText    = "TP Number";
                dgvStudents.Columns["StudyLevel"].HeaderText  = "Study Level";
                dgvStudents.Columns["Email"].HeaderText       = "Email";
                dgvStudents.Columns["Phone"].HeaderText       = "Phone";
            }

            lblCount.Text    = "Total: " + dt.Rows.Count + " student(s)";
            btnDelete.Enabled = false;
        }

        private void dgvStudents_SelectionChanged(object sender, EventArgs e)
        {
            btnDelete.Enabled = dgvStudents.SelectedRows.Count > 0;
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (dgvStudents.SelectedRows.Count == 0) return;

            DataGridViewRow row       = dgvStudents.SelectedRows[0];
            int             studentID = Convert.ToInt32(row.Cells["StudentID"].Value);
            string          name      = row.Cells["StudentName"].Value.ToString();
            string          tp        = row.Cells["TPNumber"].Value.ToString();

            if (MessageBox.Show(
                "Delete student:\n" + name + " (" + tp + ")?\n\n" +
                "This deactivates the student's account after completing coaching.",
                "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.No)
                return;

            // Soft delete – deactivate the Users record
            string upd =
                "UPDATE Users SET IsActive = 0 WHERE UserID = " +
                "(SELECT UserID FROM Students WHERE StudentID = @StudentID)";
            SqlParameter[] p = { new SqlParameter("@StudentID", studentID) };

            if (DatabaseHelper.ExecuteNonQuery(upd, p) > 0)
            {
                MessageBox.Show("Student " + name + " deleted.", "Done",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadStudents();
            }
        }

        private void cboFilterLevel_SelectedIndexChanged(object sender, EventArgs e) { LoadStudents(); }
        private void btnRefresh_Click(object sender, EventArgs e) { LoadStudents(); }
        private void btnClose_Click(object sender, EventArgs e)   { this.Close(); }

        private void InitializeComponent()
        {
            this.lblTitle       = new Label();
            this.lblInfo        = new Label();
            this.lblFilter      = new Label();
            this.cboFilterLevel = new ComboBox();
            this.lblCount       = new Label();
            this.dgvStudents    = new DataGridView();
            this.btnDelete      = new Button();
            this.btnRefresh     = new Button();
            this.btnClose       = new Button();

            this.SuspendLayout();

            this.Text            = "Delete Student";
            this.Size            = new Size(760, 500);
            this.StartPosition   = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox     = false;

            this.lblTitle.Text      = "Delete Completed Students";
            this.lblTitle.Font      = new Font("Arial", 11, FontStyle.Bold);
            this.lblTitle.Location  = new Point(10, 10);
            this.lblTitle.Size      = new Size(720, 22);
            this.lblTitle.TextAlign = ContentAlignment.MiddleCenter;

            Panel line = new Panel();
            line.BorderStyle = BorderStyle.FixedSingle;
            line.Location    = new Point(10, 35);
            line.Size        = new Size(720, 1);

            this.lblInfo.Font     = new Font("Arial", 8);
            this.lblInfo.Location = new Point(10, 42);
            this.lblInfo.Size     = new Size(300, 18);

            this.lblFilter.Text     = "Filter by Level:";
            this.lblFilter.Font     = new Font("Arial", 9);
            this.lblFilter.Location = new Point(430, 42);
            this.lblFilter.Size     = new Size(100, 18);

            this.cboFilterLevel.Font          = new Font("Arial", 9);
            this.cboFilterLevel.DropDownStyle = ComboBoxStyle.DropDownList;
            this.cboFilterLevel.Location      = new Point(535, 40);
            this.cboFilterLevel.Size          = new Size(120, 22);
            this.cboFilterLevel.Items.AddRange(new object[] { "All","Foundation","Level 1","Level 2","Level 3" });
            this.cboFilterLevel.SelectedIndex = 0;
            this.cboFilterLevel.SelectedIndexChanged += new EventHandler(this.cboFilterLevel_SelectedIndexChanged);

            this.lblCount.Font      = new Font("Arial", 8);
            this.lblCount.Location  = new Point(660, 42);
            this.lblCount.Size      = new Size(70, 18);
            this.lblCount.TextAlign = ContentAlignment.MiddleRight;

            this.dgvStudents.Location            = new Point(10, 65);
            this.dgvStudents.Size                = new Size(720, 360);
            this.dgvStudents.ReadOnly            = true;
            this.dgvStudents.AllowUserToAddRows  = false;
            this.dgvStudents.SelectionMode       = DataGridViewSelectionMode.FullRowSelect;
            this.dgvStudents.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvStudents.RowHeadersVisible   = false;
            this.dgvStudents.MultiSelect         = false;
            this.dgvStudents.SelectionChanged   += new EventHandler(this.dgvStudents_SelectionChanged);

            this.btnDelete.Text     = "Delete Student";
            this.btnDelete.Font     = new Font("Arial", 9);
            this.btnDelete.Location = new Point(10, 433);
            this.btnDelete.Size     = new Size(110, 27);
            this.btnDelete.Enabled  = false;
            this.btnDelete.Click   += new EventHandler(this.btnDelete_Click);

            this.btnRefresh.Text     = "Refresh";
            this.btnRefresh.Font     = new Font("Arial", 9);
            this.btnRefresh.Location = new Point(580, 433);
            this.btnRefresh.Size     = new Size(70, 27);
            this.btnRefresh.Click   += new EventHandler(this.btnRefresh_Click);

            this.btnClose.Text     = "Close";
            this.btnClose.Font     = new Font("Arial", 9);
            this.btnClose.Location = new Point(660, 433);
            this.btnClose.Size     = new Size(70, 27);
            this.btnClose.Click   += new EventHandler(this.btnClose_Click);

            this.Controls.Add(this.lblTitle);
            this.Controls.Add(line);
            this.Controls.Add(this.lblInfo);
            this.Controls.Add(this.lblFilter);
            this.Controls.Add(this.cboFilterLevel);
            this.Controls.Add(this.lblCount);
            this.Controls.Add(this.dgvStudents);
            this.Controls.Add(this.btnDelete);
            this.Controls.Add(this.btnRefresh);
            this.Controls.Add(this.btnClose);

            this.Load += new EventHandler(this.frmDeleteStudent_Load);
            this.ResumeLayout(false);
        }
    }

    // ============================================================
    // Lecturer Feature D: View student list with filters
    // ============================================================
    public class frmLecturerViewStudents : Form
    {
        private Label        lblTitle;
        private Label        lblFilterLevel, lblFilterModule;
        private ComboBox     cboFilterLevel, cboFilterModule;
        private Label        lblCount;
        private DataGridView dgvStudents;
        private Button       btnRefresh;
        private Button       btnClose;

        public frmLecturerViewStudents()
        {
            InitializeComponent();
        }

        private void frmLecturerViewStudents_Load(object sender, EventArgs e)
        {
            LoadModuleCombo();
            LoadStudents();
        }

        private void LoadModuleCombo()
        {
            cboFilterModule.Items.Add("All Modules");
            DataTable dt = DatabaseHelper.ExecuteQuery(
                "SELECT ModuleCode FROM Modules WHERE IsActive = 1 ORDER BY ModuleCode");
            foreach (DataRow r in dt.Rows)
                cboFilterModule.Items.Add(r["ModuleCode"].ToString());
            cboFilterModule.SelectedIndex = 0;
        }

        private void LoadStudents()
        {
            string levelFilter  = cboFilterLevel.SelectedIndex > 0
                ? " AND s.StudyLevel = '" + cboFilterLevel.SelectedItem.ToString() + "'" : "";
            string moduleFilter = cboFilterModule.SelectedIndex > 0
                ? " AND m.ModuleCode = '" + cboFilterModule.SelectedItem.ToString() + "'" : "";

            string query =
                "SELECT DISTINCT u.Name AS StudentName, s.TPNumber, s.StudyLevel, " +
                "       u.Email, u.Phone, " +
                "       m.ModuleCode, c.ClassLevel, " +
                "       CASE e.PaymentStatus WHEN 'Paid' THEN 'Paid' ELSE 'Unpaid' END AS Payment " +
                "FROM Students s " +
                "INNER JOIN Users      u ON s.UserID    = u.UserID " +
                "INNER JOIN Enrolments e ON s.StudentID = e.StudentID " +
                "INNER JOIN Classes    c ON e.ClassID   = c.ClassID " +
                "INNER JOIN Modules    m ON c.ModuleID  = m.ModuleID " +
                "WHERE u.IsActive = 1" + levelFilter + moduleFilter +
                " ORDER BY u.Name";

            DataTable dt = DatabaseHelper.ExecuteQuery(query);
            dgvStudents.DataSource = null;
            dgvStudents.DataSource = dt;

            if (dgvStudents.Columns.Count > 0)
            {
                dgvStudents.Columns["StudentName"].HeaderText = "Name";
                dgvStudents.Columns["TPNumber"].HeaderText    = "TP Number";
                dgvStudents.Columns["StudyLevel"].HeaderText  = "Level";
                dgvStudents.Columns["Email"].HeaderText       = "Email";
                dgvStudents.Columns["Phone"].HeaderText       = "Phone";
                dgvStudents.Columns["ModuleCode"].HeaderText  = "Module";
                dgvStudents.Columns["ClassLevel"].HeaderText  = "Class";
                dgvStudents.Columns["Payment"].HeaderText     = "Payment";
            }

            lblCount.Text = "Total: " + dt.Rows.Count + " record(s)";
        }

        private void cboFilterLevel_SelectedIndexChanged(object sender, EventArgs e)  { LoadStudents(); }
        private void cboFilterModule_SelectedIndexChanged(object sender, EventArgs e) { LoadStudents(); }
        private void btnRefresh_Click(object sender, EventArgs e) { LoadStudents(); }
        private void btnClose_Click(object sender, EventArgs e)   { this.Close(); }

        private void InitializeComponent()
        {
            this.lblTitle       = new Label();
            this.lblFilterLevel = new Label(); this.cboFilterLevel  = new ComboBox();
            this.lblFilterModule= new Label(); this.cboFilterModule = new ComboBox();
            this.lblCount       = new Label();
            this.dgvStudents    = new DataGridView();
            this.btnRefresh     = new Button();
            this.btnClose       = new Button();

            this.SuspendLayout();

            this.Text            = "View Students";
            this.Size            = new Size(900, 520);
            this.StartPosition   = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox     = false;

            this.lblTitle.Text      = "Student List";
            this.lblTitle.Font      = new Font("Arial", 11, FontStyle.Bold);
            this.lblTitle.Location  = new Point(10, 10);
            this.lblTitle.Size      = new Size(860, 22);
            this.lblTitle.TextAlign = ContentAlignment.MiddleCenter;

            Panel line = new Panel();
            line.BorderStyle = BorderStyle.FixedSingle;
            line.Location    = new Point(10, 35);
            line.Size        = new Size(860, 1);

            this.lblFilterLevel.Text     = "Level:";
            this.lblFilterLevel.Font     = new Font("Arial", 9);
            this.lblFilterLevel.Location = new Point(10, 44);
            this.lblFilterLevel.Size     = new Size(45, 22);
            this.cboFilterLevel.Font          = new Font("Arial", 9);
            this.cboFilterLevel.DropDownStyle = ComboBoxStyle.DropDownList;
            this.cboFilterLevel.Location      = new Point(60, 42);
            this.cboFilterLevel.Size          = new Size(130, 22);
            this.cboFilterLevel.Items.AddRange(new object[] { "All","Foundation","Level 1","Level 2","Level 3" });
            this.cboFilterLevel.SelectedIndex = 0;
            this.cboFilterLevel.SelectedIndexChanged += new EventHandler(this.cboFilterLevel_SelectedIndexChanged);

            this.lblFilterModule.Text     = "Module:";
            this.lblFilterModule.Font     = new Font("Arial", 9);
            this.lblFilterModule.Location = new Point(205, 44);
            this.lblFilterModule.Size     = new Size(60, 22);
            this.cboFilterModule.Font          = new Font("Arial", 9);
            this.cboFilterModule.DropDownStyle = ComboBoxStyle.DropDownList;
            this.cboFilterModule.Location      = new Point(270, 42);
            this.cboFilterModule.Size          = new Size(140, 22);
            this.cboFilterModule.SelectedIndexChanged += new EventHandler(this.cboFilterModule_SelectedIndexChanged);

            this.lblCount.Font      = new Font("Arial", 8);
            this.lblCount.Location  = new Point(700, 44);
            this.lblCount.Size      = new Size(170, 18);
            this.lblCount.TextAlign = ContentAlignment.MiddleRight;

            this.dgvStudents.Location            = new Point(10, 68);
            this.dgvStudents.Size                = new Size(860, 390);
            this.dgvStudents.ReadOnly            = true;
            this.dgvStudents.AllowUserToAddRows  = false;
            this.dgvStudents.SelectionMode       = DataGridViewSelectionMode.FullRowSelect;
            this.dgvStudents.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvStudents.RowHeadersVisible   = false;

            this.btnRefresh.Text     = "Refresh";
            this.btnRefresh.Font     = new Font("Arial", 9);
            this.btnRefresh.Location = new Point(720, 466);
            this.btnRefresh.Size     = new Size(70, 27);
            this.btnRefresh.Click   += new EventHandler(this.btnRefresh_Click);

            this.btnClose.Text     = "Close";
            this.btnClose.Font     = new Font("Arial", 9);
            this.btnClose.Location = new Point(800, 466);
            this.btnClose.Size     = new Size(70, 27);
            this.btnClose.Click   += new EventHandler(this.btnClose_Click);

            this.Controls.Add(this.lblTitle);
            this.Controls.Add(line);
            this.Controls.Add(this.lblFilterLevel);  this.Controls.Add(this.cboFilterLevel);
            this.Controls.Add(this.lblFilterModule); this.Controls.Add(this.cboFilterModule);
            this.Controls.Add(this.lblCount);
            this.Controls.Add(this.dgvStudents);
            this.Controls.Add(this.btnRefresh);
            this.Controls.Add(this.btnClose);

            this.Load += new EventHandler(this.frmLecturerViewStudents_Load);
            this.ResumeLayout(false);
        }
    }
}
