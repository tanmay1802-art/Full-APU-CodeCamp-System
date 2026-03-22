using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace APUCodeCamp
{
    // Lecturer Feature B: Approve or reject student coaching requests
    public class frmApproveRequests : Form
    {
        private Label        lblTitle;
        private Label        lblInfo;
        private Label        lblFilter;
        private ComboBox     cboFilter;
        private Label        lblCount;
        private DataGridView dgvRequests;
        private Label        lblNotes;
        private TextBox      txtNotes;
        private Button       btnApprove;
        private Button       btnReject;
        private Button       btnRefresh;
        private Button       btnClose;

        public frmApproveRequests()
        {
            InitializeComponent();
        }

        private void frmApproveRequests_Load(object sender, EventArgs e)
        {
            lblInfo.Text = "Lecturer: " + UserSession.Name + "  |  Staff: " + UserSession.LecturerStaffID;
            LoadRequests();
        }

        private void LoadRequests()
        {
            string filter = cboFilter.SelectedIndex > 0
                ? " AND cr.Status = '" + cboFilter.SelectedItem.ToString() + "'"
                : "";

            string query =
                "SELECT cr.RequestID, " +
                "       u.Name AS Student, s.TPNumber, " +
                "       m.ModuleCode + ' - ' + m.ModuleName AS Module, " +
                "       cr.ClassLevel, cr.Reason, cr.Status, " +
                "       CONVERT(NVARCHAR, cr.RequestDate, 103) AS RequestDate, " +
                "       ISNULL(cr.ReviewNotes,'') AS ReviewNotes " +
                "FROM CoachingRequests cr " +
                "INNER JOIN Students s ON cr.StudentID  = s.StudentID " +
                "INNER JOIN Users    u ON s.UserID      = u.UserID " +
                "INNER JOIN Modules  m ON cr.ModuleID   = m.ModuleID " +
                "WHERE cr.LecturerID = @LecturerID" + filter +
                " ORDER BY cr.RequestDate DESC";

            SqlParameter[] p = { new SqlParameter("@LecturerID", UserSession.LecturerID) };
            DataTable dt     = DatabaseHelper.ExecuteQuery(query, p);

            dgvRequests.DataSource = null;
            dgvRequests.DataSource = dt;

            if (dgvRequests.Columns.Count > 0)
            {
                dgvRequests.Columns["RequestID"].Visible    = false;
                dgvRequests.Columns["ReviewNotes"].Visible  = false;
                dgvRequests.Columns["Student"].HeaderText   = "Student";
                dgvRequests.Columns["TPNumber"].HeaderText  = "TP";
                dgvRequests.Columns["Module"].HeaderText    = "Module";
                dgvRequests.Columns["ClassLevel"].HeaderText= "Level";
                dgvRequests.Columns["Reason"].HeaderText    = "Reason";
                dgvRequests.Columns["Status"].HeaderText    = "Status";
                dgvRequests.Columns["RequestDate"].HeaderText = "Date";
            }

            lblCount.Text = "Total: " + dt.Rows.Count + " request(s)";
            UpdateButtons();
        }

        private void UpdateButtons()
        {
            bool hasPending = false;
            if (dgvRequests.SelectedRows.Count > 0)
                hasPending = dgvRequests.SelectedRows[0].Cells["Status"].Value.ToString() == "Pending";

            btnApprove.Enabled = hasPending;
            btnReject.Enabled  = hasPending;
        }

        private void dgvRequests_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvRequests.SelectedRows.Count > 0)
            {
                string notes = dgvRequests.SelectedRows[0].Cells["ReviewNotes"].Value.ToString();
                txtNotes.Text = notes;
            }
            else txtNotes.Clear();
            UpdateButtons();
        }

        private void btnApprove_Click(object sender, EventArgs e)
        {
            ProcessRequest("Approved");
        }

        private void btnReject_Click(object sender, EventArgs e)
        {
            if (txtNotes.Text.Trim() == "")
            {
                MessageBox.Show("Please enter a reason for rejection in the Notes field.", "Validation",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            ProcessRequest("Rejected");
        }

        private void ProcessRequest(string status)
        {
            if (dgvRequests.SelectedRows.Count == 0) return;

            int    requestID = Convert.ToInt32(dgvRequests.SelectedRows[0].Cells["RequestID"].Value);
            string student   = dgvRequests.SelectedRows[0].Cells["Student"].Value.ToString();

            if (MessageBox.Show(status + " request from " + student + "?", "Confirm",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No) return;

            string upd =
                "UPDATE CoachingRequests " +
                "SET Status = @Status, ReviewDate = GETDATE(), ReviewNotes = @Notes " +
                "WHERE RequestID = @ID AND Status = 'Pending'";
            SqlParameter[] p =
            {
                new SqlParameter("@Status", status),
                new SqlParameter("@Notes",  txtNotes.Text.Trim()),
                new SqlParameter("@ID",     requestID)
            };

            if (DatabaseHelper.ExecuteNonQuery(upd, p) > 0)
            {
                MessageBox.Show("Request " + status.ToLower() + ".", "Done",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                txtNotes.Clear();
                LoadRequests();
            }
        }

        private void cboFilter_SelectedIndexChanged(object sender, EventArgs e) { LoadRequests(); }
        private void btnRefresh_Click(object sender, EventArgs e) { LoadRequests(); }
        private void btnClose_Click(object sender, EventArgs e)   { this.Close(); }

        private void InitializeComponent()
        {
            this.lblTitle    = new Label();
            this.lblInfo     = new Label();
            this.lblFilter   = new Label();
            this.cboFilter   = new ComboBox();
            this.lblCount    = new Label();
            this.dgvRequests = new DataGridView();
            this.lblNotes    = new Label();
            this.txtNotes    = new TextBox();
            this.btnApprove  = new Button();
            this.btnReject   = new Button();
            this.btnRefresh  = new Button();
            this.btnClose    = new Button();

            this.SuspendLayout();

            this.Text            = "Approve / Reject Student Requests";
            this.Size            = new Size(860, 580);
            this.StartPosition   = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox     = false;

            this.lblTitle.Text      = "Student Coaching Enrolment Requests";
            this.lblTitle.Font      = new Font("Arial", 11, FontStyle.Bold);
            this.lblTitle.Location  = new Point(10, 10);
            this.lblTitle.Size      = new Size(820, 22);
            this.lblTitle.TextAlign = ContentAlignment.MiddleCenter;

            Panel line = new Panel();
            line.BorderStyle = BorderStyle.FixedSingle;
            line.Location    = new Point(10, 35);
            line.Size        = new Size(820, 1);

            this.lblInfo.Font     = new Font("Arial", 8);
            this.lblInfo.Location = new Point(10, 42);
            this.lblInfo.Size     = new Size(400, 18);

            this.lblFilter.Text     = "Filter:";
            this.lblFilter.Font     = new Font("Arial", 9);
            this.lblFilter.Location = new Point(550, 42);
            this.lblFilter.Size     = new Size(50, 18);

            this.cboFilter.Font          = new Font("Arial", 9);
            this.cboFilter.DropDownStyle = ComboBoxStyle.DropDownList;
            this.cboFilter.Location      = new Point(605, 40);
            this.cboFilter.Size          = new Size(110, 22);
            this.cboFilter.Items.AddRange(new object[] { "All","Pending","Approved","Rejected" });
            this.cboFilter.SelectedIndex = 0;
            this.cboFilter.SelectedIndexChanged += new EventHandler(this.cboFilter_SelectedIndexChanged);

            this.lblCount.Font      = new Font("Arial", 8);
            this.lblCount.Location  = new Point(720, 42);
            this.lblCount.Size      = new Size(110, 18);
            this.lblCount.TextAlign = ContentAlignment.MiddleRight;

            this.dgvRequests.Location            = new Point(10, 65);
            this.dgvRequests.Size                = new Size(820, 310);
            this.dgvRequests.ReadOnly            = true;
            this.dgvRequests.AllowUserToAddRows  = false;
            this.dgvRequests.SelectionMode       = DataGridViewSelectionMode.FullRowSelect;
            this.dgvRequests.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvRequests.RowHeadersVisible   = false;
            this.dgvRequests.MultiSelect         = false;
            this.dgvRequests.SelectionChanged   += new EventHandler(this.dgvRequests_SelectionChanged);

            this.lblNotes.Text     = "Review Notes (required for rejection):";
            this.lblNotes.Font     = new Font("Arial", 9);
            this.lblNotes.Location = new Point(10, 386);
            this.lblNotes.Size     = new Size(300, 20);

            this.txtNotes.Font       = new Font("Arial", 9);
            this.txtNotes.Location   = new Point(10, 408);
            this.txtNotes.Size       = new Size(820, 65);
            this.txtNotes.Multiline  = true;
            this.txtNotes.ScrollBars = ScrollBars.Vertical;

            this.btnApprove.Text     = "Approve";
            this.btnApprove.Font     = new Font("Arial", 9);
            this.btnApprove.Location = new Point(10, 485);
            this.btnApprove.Size     = new Size(90, 28);
            this.btnApprove.Enabled  = false;
            this.btnApprove.Click   += new EventHandler(this.btnApprove_Click);

            this.btnReject.Text     = "Reject";
            this.btnReject.Font     = new Font("Arial", 9);
            this.btnReject.Location = new Point(115, 485);
            this.btnReject.Size     = new Size(90, 28);
            this.btnReject.Enabled  = false;
            this.btnReject.Click   += new EventHandler(this.btnReject_Click);

            this.btnRefresh.Text     = "Refresh";
            this.btnRefresh.Font     = new Font("Arial", 9);
            this.btnRefresh.Location = new Point(688, 485);
            this.btnRefresh.Size     = new Size(70, 28);
            this.btnRefresh.Click   += new EventHandler(this.btnRefresh_Click);

            this.btnClose.Text     = "Close";
            this.btnClose.Font     = new Font("Arial", 9);
            this.btnClose.Location = new Point(768, 485);
            this.btnClose.Size     = new Size(62, 28);
            this.btnClose.Click   += new EventHandler(this.btnClose_Click);

            this.Controls.Add(this.lblTitle);
            this.Controls.Add(line);
            this.Controls.Add(this.lblInfo);
            this.Controls.Add(this.lblFilter);
            this.Controls.Add(this.cboFilter);
            this.Controls.Add(this.lblCount);
            this.Controls.Add(this.dgvRequests);
            this.Controls.Add(this.lblNotes);
            this.Controls.Add(this.txtNotes);
            this.Controls.Add(this.btnApprove);
            this.Controls.Add(this.btnReject);
            this.Controls.Add(this.btnRefresh);
            this.Controls.Add(this.btnClose);

            this.Load += new EventHandler(this.frmApproveRequests_Load);
            this.ResumeLayout(false);
        }
    }
}
