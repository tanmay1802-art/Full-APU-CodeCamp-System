using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace APUCodeCamp
{
    // Feature A: View enrolled class schedule
    public class frmViewSchedule : Form
    {
        private Label        lblTitle;
        private Label        lblStudentInfo;
        private Label        lblRecordCount;
        private DataGridView dgvSchedule;
        private Button       btnViewDetail;
        private Button       btnRefresh;
        private Button       btnClose;

        public frmViewSchedule()
        {
            InitializeComponent();
        }

        private void frmViewSchedule_Load(object sender, EventArgs e)
        {
            lblStudentInfo.Text = "Student: " + StudentSession.Name +
                                  "   TP: " + StudentSession.TPNumber;
            LoadSchedule();
        }

        private void LoadSchedule()
        {
            string query =
                "SELECT " +
                "    e.EnrolmentID, " +
                "    m.ModuleCode, " +
                "    m.ModuleName, " +
                "    c.ClassLevel, " +
                "    c.Schedule, " +
                "    c.Venue, " +
                "    c.Fee, " +
                "    c.StartDate, " +
                "    c.EndDate, " +
                "    u.Name AS TrainerName, " +
                "    e.MonthOfEnrolment, " +
                "    e.PaymentStatus " +
                "FROM Enrolments e " +
                "INNER JOIN Classes  c ON e.ClassID   = c.ClassID " +
                "INNER JOIN Modules  m ON c.ModuleID  = m.ModuleID " +
                "INNER JOIN Trainers t ON c.TrainerID = t.TrainerID " +
                "INNER JOIN Users    u ON t.UserID    = u.UserID " +
                "WHERE e.StudentID = @StudentID " +
                "ORDER BY c.StartDate";

            SqlParameter[] p = { new SqlParameter("@StudentID", StudentSession.StudentID) };
            DataTable dt     = DatabaseHelper.ExecuteQuery(query, p);

            dgvSchedule.DataSource = null;
            dgvSchedule.DataSource = dt;

            if (dgvSchedule.Columns.Count > 0)
            {
                dgvSchedule.Columns["EnrolmentID"].Visible    = false;
                dgvSchedule.Columns["ModuleCode"].HeaderText  = "Code";
                dgvSchedule.Columns["ModuleName"].HeaderText  = "Module Name";
                dgvSchedule.Columns["ClassLevel"].HeaderText  = "Level";
                dgvSchedule.Columns["Schedule"].HeaderText    = "Schedule";
                dgvSchedule.Columns["Venue"].HeaderText       = "Venue";
                dgvSchedule.Columns["Fee"].HeaderText         = "Fee (RM)";
                dgvSchedule.Columns["StartDate"].HeaderText   = "Start Date";
                dgvSchedule.Columns["EndDate"].HeaderText     = "End Date";
                dgvSchedule.Columns["TrainerName"].HeaderText = "Trainer";
                dgvSchedule.Columns["MonthOfEnrolment"].HeaderText = "Month";
                dgvSchedule.Columns["PaymentStatus"].HeaderText    = "Payment";

                dgvSchedule.Columns["Fee"].DefaultCellStyle.Format      = "N2";
                dgvSchedule.Columns["StartDate"].DefaultCellStyle.Format = "dd/MM/yyyy";
                dgvSchedule.Columns["EndDate"].DefaultCellStyle.Format   = "dd/MM/yyyy";
            }

            lblRecordCount.Text = "Total: " + dt.Rows.Count + " class(es)";
        }

        private void btnViewDetail_Click(object sender, EventArgs e)
        {
            if (dgvSchedule.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a row to view details.", "No Selection",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DataGridViewRow row = dgvSchedule.SelectedRows[0];

            string detail =
                "Module Code  : " + row.Cells["ModuleCode"].Value + "\n" +
                "Module Name  : " + row.Cells["ModuleName"].Value + "\n" +
                "Class Level  : " + row.Cells["ClassLevel"].Value + "\n" +
                "Schedule     : " + row.Cells["Schedule"].Value   + "\n" +
                "Venue        : " + row.Cells["Venue"].Value      + "\n" +
                "Trainer      : " + row.Cells["TrainerName"].Value + "\n" +
                "Start Date   : " + Convert.ToDateTime(row.Cells["StartDate"].Value).ToString("dd/MM/yyyy") + "\n" +
                "End Date     : " + Convert.ToDateTime(row.Cells["EndDate"].Value).ToString("dd/MM/yyyy")   + "\n" +
                "Fee          : RM " + string.Format("{0:N2}", row.Cells["Fee"].Value) + "\n" +
                "Payment      : " + row.Cells["PaymentStatus"].Value + "\n" +
                "Month        : " + row.Cells["MonthOfEnrolment"].Value;

            MessageBox.Show(detail, "Session Detail", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnRefresh_Click(object sender, EventArgs e) { LoadSchedule(); }
        private void btnClose_Click(object sender, EventArgs e)   { this.Close(); }

        private void InitializeComponent()
        {
            this.lblTitle       = new Label();
            this.lblStudentInfo = new Label();
            this.lblRecordCount = new Label();
            this.dgvSchedule    = new DataGridView();
            this.btnViewDetail  = new Button();
            this.btnRefresh     = new Button();
            this.btnClose       = new Button();

            this.SuspendLayout();

            // Form
            this.Text            = "My Class Schedule";
            this.Size            = new Size(960, 530);
            this.StartPosition   = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox     = false;

            // Title
            this.lblTitle.Text      = "My Enrolled Class Schedule";
            this.lblTitle.Font      = new Font("Arial", 11, FontStyle.Bold);
            this.lblTitle.Location  = new Point(10, 10);
            this.lblTitle.Size      = new Size(925, 22);
            this.lblTitle.TextAlign = ContentAlignment.MiddleCenter;

            // Separator
            Panel line = new Panel();
            line.BorderStyle = BorderStyle.FixedSingle;
            line.Location    = new Point(10, 35);
            line.Size        = new Size(922, 1);

            // Student info
            this.lblStudentInfo.Font     = new Font("Arial", 8);
            this.lblStudentInfo.Location = new Point(10, 40);
            this.lblStudentInfo.Size     = new Size(500, 18);

            // Record count
            this.lblRecordCount.Font      = new Font("Arial", 8);
            this.lblRecordCount.Location  = new Point(700, 40);
            this.lblRecordCount.Size      = new Size(232, 18);
            this.lblRecordCount.TextAlign = ContentAlignment.MiddleRight;

            // DataGridView
            this.dgvSchedule.Location            = new Point(10, 62);
            this.dgvSchedule.Size                = new Size(922, 380);
            this.dgvSchedule.ReadOnly            = true;
            this.dgvSchedule.AllowUserToAddRows  = false;
            this.dgvSchedule.SelectionMode       = DataGridViewSelectionMode.FullRowSelect;
            this.dgvSchedule.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvSchedule.RowHeadersVisible   = false;
            this.dgvSchedule.MultiSelect         = false;

            // Buttons
            this.btnViewDetail.Text     = "View Detail";
            this.btnViewDetail.Font     = new Font("Arial", 9);
            this.btnViewDetail.Location = new Point(10, 455);
            this.btnViewDetail.Size     = new Size(90, 27);
            this.btnViewDetail.Click   += new EventHandler(this.btnViewDetail_Click);

            this.btnRefresh.Text     = "Refresh";
            this.btnRefresh.Font     = new Font("Arial", 9);
            this.btnRefresh.Location = new Point(847, 455);
            this.btnRefresh.Size     = new Size(70, 27);
            this.btnRefresh.Click   += new EventHandler(this.btnRefresh_Click);

            this.btnClose.Text     = "Close";
            this.btnClose.Font     = new Font("Arial", 9);
            this.btnClose.Location = new Point(854, 455);
            this.btnClose.Size     = new Size(78, 27);
            this.btnClose.Click   += new EventHandler(this.btnClose_Click);

            // Fix button positions so they don't overlap
            this.btnRefresh.Location = new Point(770, 455);
            this.btnClose.Location   = new Point(854, 455);

            this.Controls.Add(this.lblTitle);
            this.Controls.Add(line);
            this.Controls.Add(this.lblStudentInfo);
            this.Controls.Add(this.lblRecordCount);
            this.Controls.Add(this.dgvSchedule);
            this.Controls.Add(this.btnViewDetail);
            this.Controls.Add(this.btnRefresh);
            this.Controls.Add(this.btnClose);

            this.Load += new EventHandler(this.frmViewSchedule_Load);
            this.ResumeLayout(false);
        }
    }
}
