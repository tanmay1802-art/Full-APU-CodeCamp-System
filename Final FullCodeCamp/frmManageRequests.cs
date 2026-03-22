using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace APUCodeCamp
{
    // Feature C: View all requests, cancel pending ones
    public class frmManageRequests : Form
    {
        private Label        lblTitle;
        private Label        lblStudentInfo;
        private Label        lblRecordCount;
        private Label        lblFilter;
        private ComboBox     cboFilterStatus;
        private DataGridView dgvRequests;
        private Button       btnCancelRequest;
        private Button       btnRefresh;
        private Button       btnClose;

        public frmManageRequests()
        {
            InitializeComponent();
        }

        private void frmManageRequests_Load(object sender, EventArgs e)
        {
            lblStudentInfo.Text = "Student: " + StudentSession.Name +
                                  "   TP: " + StudentSession.TPNumber;
            LoadRequests();
        }

        private void LoadRequests()
        {
            string statusFilter = "";
            if (cboFilterStatus.SelectedIndex > 0)
                statusFilter = " AND cr.Status = '" + cboFilterStatus.SelectedItem.ToString() + "'";

            string query =
                "SELECT " +
                "    cr.RequestID, " +
                "    m.ModuleCode + ' - ' + m.ModuleName AS Module, " +
                "    cr.ClassLevel, " +
                "    u.Name AS Lecturer, " +
                "    cr.Reason, " +
                "    cr.Status, " +
                "    CONVERT(NVARCHAR, cr.RequestDate, 103) + ' ' + " +
                "    CONVERT(NVARCHAR, cr.RequestDate, 108) AS RequestDate " +
                "FROM CoachingRequests cr " +
                "INNER JOIN Modules   m ON cr.ModuleID   = m.ModuleID " +
                "INNER JOIN Lecturers l ON cr.LecturerID = l.LecturerID " +
                "INNER JOIN Users     u ON l.UserID      = u.UserID " +
                "WHERE cr.StudentID = @StudentID" + statusFilter +
                " ORDER BY cr.RequestDate DESC";

            SqlParameter[] p = { new SqlParameter("@StudentID", StudentSession.StudentID) };
            DataTable dt     = DatabaseHelper.ExecuteQuery(query, p);

            dgvRequests.DataSource = null;
            dgvRequests.DataSource = dt;

            if (dgvRequests.Columns.Count > 0)
            {
                dgvRequests.Columns["RequestID"].Visible     = false;
                dgvRequests.Columns["Module"].HeaderText     = "Module";
                dgvRequests.Columns["ClassLevel"].HeaderText = "Level";
                dgvRequests.Columns["Lecturer"].HeaderText   = "Lecturer";
                dgvRequests.Columns["Reason"].HeaderText     = "Reason";
                dgvRequests.Columns["Status"].HeaderText     = "Status";
                dgvRequests.Columns["RequestDate"].HeaderText= "Date";
            }

            lblRecordCount.Text = "Total: " + dt.Rows.Count + " request(s)";
            UpdateButtonState();
        }

        private void UpdateButtonState()
        {
            btnCancelRequest.Enabled = false;
            if (dgvRequests.SelectedRows.Count > 0)
            {
                string status = dgvRequests.SelectedRows[0].Cells["Status"].Value.ToString();
                btnCancelRequest.Enabled = (status == "Pending");
            }
        }

        private void dgvRequests_SelectionChanged(object sender, EventArgs e)
        {
            UpdateButtonState();
        }

        private void btnCancelRequest_Click(object sender, EventArgs e)
        {
            if (dgvRequests.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a request first.", "No Selection",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DataGridViewRow row    = dgvRequests.SelectedRows[0];
            string          status = row.Cells["Status"].Value.ToString();

            if (status != "Pending")
            {
                MessageBox.Show("Only Pending requests can be cancelled.", "Cannot Cancel",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int    requestID = Convert.ToInt32(row.Cells["RequestID"].Value);
            string module    = row.Cells["Module"].Value.ToString();

            if (MessageBox.Show("Cancel the request for:\n\n" + module + "?",
                "Confirm Cancel", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                return;

            string deleteQuery =
                "DELETE FROM CoachingRequests WHERE RequestID = @RequestID AND Status = 'Pending'";
            SqlParameter[] p = { new SqlParameter("@RequestID", requestID) };

            int rows = DatabaseHelper.ExecuteNonQuery(deleteQuery, p);
            if (rows > 0)
            {
                MessageBox.Show("Request cancelled.", "Done",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadRequests();
            }
            else
            {
                MessageBox.Show("Could not cancel. It may already be processed.", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void cboFilterStatus_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadRequests();
        }

        private void btnRefresh_Click(object sender, EventArgs e) { LoadRequests(); }
        private void btnClose_Click(object sender, EventArgs e)   { this.Close(); }

        private void InitializeComponent()
        {
            this.lblTitle        = new Label();
            this.lblStudentInfo  = new Label();
            this.lblRecordCount  = new Label();
            this.lblFilter       = new Label();
            this.cboFilterStatus = new ComboBox();
            this.dgvRequests     = new DataGridView();
            this.btnCancelRequest= new Button();
            this.btnRefresh      = new Button();
            this.btnClose        = new Button();

            this.SuspendLayout();

            // Form
            this.Text            = "My Coaching Requests";
            this.Size            = new Size(860, 510);
            this.StartPosition   = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox     = false;

            // Title
            this.lblTitle.Text      = "My Coaching Requests";
            this.lblTitle.Font      = new Font("Arial", 11, FontStyle.Bold);
            this.lblTitle.Location  = new Point(10, 10);
            this.lblTitle.Size      = new Size(822, 22);
            this.lblTitle.TextAlign = ContentAlignment.MiddleCenter;

            Panel line = new Panel();
            line.BorderStyle = BorderStyle.FixedSingle;
            line.Location    = new Point(10, 35);
            line.Size        = new Size(822, 1);

            // Student info
            this.lblStudentInfo.Font     = new Font("Arial", 8);
            this.lblStudentInfo.Location = new Point(10, 40);
            this.lblStudentInfo.Size     = new Size(400, 18);

            // Filter
            this.lblFilter.Text     = "Filter by Status:";
            this.lblFilter.Font     = new Font("Arial", 8);
            this.lblFilter.Location = new Point(520, 40);
            this.lblFilter.Size     = new Size(100, 18);

            this.cboFilterStatus.Font          = new Font("Arial", 8);
            this.cboFilterStatus.DropDownStyle = ComboBoxStyle.DropDownList;
            this.cboFilterStatus.Location      = new Point(625, 38);
            this.cboFilterStatus.Size          = new Size(100, 20);
            this.cboFilterStatus.Items.AddRange(new object[] { "All", "Pending", "Approved", "Rejected" });
            this.cboFilterStatus.SelectedIndex = 0;
            this.cboFilterStatus.SelectedIndexChanged += new EventHandler(this.cboFilterStatus_SelectedIndexChanged);

            // Record count
            this.lblRecordCount.Font      = new Font("Arial", 8);
            this.lblRecordCount.Location  = new Point(730, 40);
            this.lblRecordCount.Size      = new Size(102, 18);
            this.lblRecordCount.TextAlign = ContentAlignment.MiddleRight;

            // DataGridView
            this.dgvRequests.Location            = new Point(10, 62);
            this.dgvRequests.Size                = new Size(822, 360);
            this.dgvRequests.ReadOnly            = true;
            this.dgvRequests.AllowUserToAddRows  = false;
            this.dgvRequests.SelectionMode       = DataGridViewSelectionMode.FullRowSelect;
            this.dgvRequests.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvRequests.RowHeadersVisible   = false;
            this.dgvRequests.MultiSelect         = false;
            this.dgvRequests.SelectionChanged   += new EventHandler(this.dgvRequests_SelectionChanged);

            // Buttons
            this.btnCancelRequest.Text     = "Cancel Request";
            this.btnCancelRequest.Font     = new Font("Arial", 9);
            this.btnCancelRequest.Location = new Point(10, 435);
            this.btnCancelRequest.Size     = new Size(115, 27);
            this.btnCancelRequest.Enabled  = false;
            this.btnCancelRequest.Click   += new EventHandler(this.btnCancelRequest_Click);

            this.btnRefresh.Text     = "Refresh";
            this.btnRefresh.Font     = new Font("Arial", 9);
            this.btnRefresh.Location = new Point(748, 435);
            this.btnRefresh.Size     = new Size(70, 27);
            this.btnRefresh.Click   += new EventHandler(this.btnRefresh_Click);

            this.btnClose.Text     = "Close";
            this.btnClose.Font     = new Font("Arial", 9);
            this.btnClose.Location = new Point(762, 435);
            this.btnClose.Size     = new Size(70, 27);
            this.btnClose.Click   += new EventHandler(this.btnClose_Click);

            // Fix overlapping buttons
            this.btnRefresh.Location = new Point(680, 435);
            this.btnClose.Location   = new Point(762, 435);

            this.Controls.Add(this.lblTitle);
            this.Controls.Add(line);
            this.Controls.Add(this.lblStudentInfo);
            this.Controls.Add(this.lblFilter);
            this.Controls.Add(this.cboFilterStatus);
            this.Controls.Add(this.lblRecordCount);
            this.Controls.Add(this.dgvRequests);
            this.Controls.Add(this.btnCancelRequest);
            this.Controls.Add(this.btnRefresh);
            this.Controls.Add(this.btnClose);

            this.Load += new EventHandler(this.frmManageRequests_Load);
            this.ResumeLayout(false);
        }
    }
}
