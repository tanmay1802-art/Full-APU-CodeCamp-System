using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace APUCodeCamp
{
    // Admin Feature C: View feedback submitted by trainers
    public class frmViewFeedback : Form
    {
        private Label        lblTitle;
        private Label        lblFilter;
        private ComboBox     cboFilterType;
        private Label        lblCount;
        private DataGridView dgvFeedbacks;
        private Label        lblDetailTitle;
        private RichTextBox  rtbDetail;
        private Button       btnMarkRead;
        private Button       btnRefresh;
        private Button       btnClose;

        public frmViewFeedback()
        {
            InitializeComponent();
        }

        private void frmViewFeedback_Load(object sender, EventArgs e)
        {
            LoadFeedbacks();
        }

        private void LoadFeedbacks()
        {
            string typeFilter = "";
            if (cboFilterType.SelectedIndex > 0)
                typeFilter = " AND f.FeedbackType = '" + cboFilterType.SelectedItem.ToString() + "'";

            string query =
                "SELECT f.FeedbackID, u.Name AS Trainer, f.FeedbackType, f.Subject, " +
                "       CONVERT(NVARCHAR, f.FeedbackDate, 103) AS FeedbackDate, " +
                "       CASE WHEN f.IsRead = 1 THEN 'Read' ELSE 'Unread' END AS Status, " +
                "       f.Message " +
                "FROM Feedbacks f " +
                "INNER JOIN Trainers t ON f.TrainerID = t.TrainerID " +
                "INNER JOIN Users    u ON t.UserID    = u.UserID " +
                "WHERE 1=1" + typeFilter +
                " ORDER BY f.FeedbackDate DESC";

            DataTable dt = DatabaseHelper.ExecuteQuery(query);
            dgvFeedbacks.DataSource = null;
            dgvFeedbacks.DataSource = dt;

            if (dgvFeedbacks.Columns.Count > 0)
            {
                dgvFeedbacks.Columns["FeedbackID"].Visible  = false;
                dgvFeedbacks.Columns["Message"].Visible     = false;
                dgvFeedbacks.Columns["Trainer"].HeaderText  = "Trainer";
                dgvFeedbacks.Columns["FeedbackType"].HeaderText = "Type";
                dgvFeedbacks.Columns["Subject"].HeaderText  = "Subject";
                dgvFeedbacks.Columns["FeedbackDate"].HeaderText = "Date";
                dgvFeedbacks.Columns["Status"].HeaderText   = "Status";
            }

            lblCount.Text = "Total: " + dt.Rows.Count + " feedback(s)";
            rtbDetail.Clear();
            btnMarkRead.Enabled = false;
        }

        private void dgvFeedbacks_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvFeedbacks.SelectedRows.Count == 0) return;
            DataGridViewRow row = dgvFeedbacks.SelectedRows[0];

            rtbDetail.Text =
                "From     : " + row.Cells["Trainer"].Value     + "\r\n" +
                "Type     : " + row.Cells["FeedbackType"].Value + "\r\n" +
                "Subject  : " + row.Cells["Subject"].Value     + "\r\n" +
                "Date     : " + row.Cells["FeedbackDate"].Value + "\r\n" +
                "Status   : " + row.Cells["Status"].Value      + "\r\n" +
                "─────────────────────────────────────\r\n" +
                row.Cells["Message"].Value.ToString();

            btnMarkRead.Enabled = (row.Cells["Status"].Value.ToString() == "Unread");
        }

        private void btnMarkRead_Click(object sender, EventArgs e)
        {
            if (dgvFeedbacks.SelectedRows.Count == 0) return;
            int id = Convert.ToInt32(dgvFeedbacks.SelectedRows[0].Cells["FeedbackID"].Value);

            string upd = "UPDATE Feedbacks SET IsRead = 1 WHERE FeedbackID = @ID";
            SqlParameter[] p = { new SqlParameter("@ID", id) };
            if (DatabaseHelper.ExecuteNonQuery(upd, p) > 0)
            {
                MessageBox.Show("Marked as read.", "Done",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadFeedbacks();
            }
        }

        private void cboFilterType_SelectedIndexChanged(object sender, EventArgs e) { LoadFeedbacks(); }
        private void btnRefresh_Click(object sender, EventArgs e) { LoadFeedbacks(); }
        private void btnClose_Click(object sender, EventArgs e)   { this.Close(); }

        private void InitializeComponent()
        {
            this.lblTitle      = new Label();
            this.lblFilter     = new Label();
            this.cboFilterType = new ComboBox();
            this.lblCount      = new Label();
            this.dgvFeedbacks  = new DataGridView();
            this.lblDetailTitle= new Label();
            this.rtbDetail     = new RichTextBox();
            this.btnMarkRead   = new Button();
            this.btnRefresh    = new Button();
            this.btnClose      = new Button();

            this.SuspendLayout();

            this.Text            = "Trainer Feedback";
            this.Size            = new Size(800, 590);
            this.StartPosition   = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox     = false;

            this.lblTitle.Text      = "Trainer Feedback";
            this.lblTitle.Font      = new Font("Arial", 11, FontStyle.Bold);
            this.lblTitle.Location  = new Point(10, 10);
            this.lblTitle.Size      = new Size(760, 22);
            this.lblTitle.TextAlign = ContentAlignment.MiddleCenter;

            Panel line = new Panel();
            line.BorderStyle = BorderStyle.FixedSingle;
            line.Location    = new Point(10, 35);
            line.Size        = new Size(760, 1);

            this.lblFilter.Text     = "Filter by Type:";
            this.lblFilter.Font     = new Font("Arial", 9);
            this.lblFilter.Location = new Point(10, 44);
            this.lblFilter.Size     = new Size(100, 20);

            this.cboFilterType.Font          = new Font("Arial", 9);
            this.cboFilterType.DropDownStyle = ComboBoxStyle.DropDownList;
            this.cboFilterType.Location      = new Point(115, 42);
            this.cboFilterType.Size          = new Size(130, 22);
            this.cboFilterType.Items.AddRange(new object[] { "All", "Suggestion", "Complaint", "Other" });
            this.cboFilterType.SelectedIndex = 0;
            this.cboFilterType.SelectedIndexChanged += new EventHandler(this.cboFilterType_SelectedIndexChanged);

            this.lblCount.Font      = new Font("Arial", 8);
            this.lblCount.Location  = new Point(600, 44);
            this.lblCount.Size      = new Size(170, 18);
            this.lblCount.TextAlign = ContentAlignment.MiddleRight;

            this.dgvFeedbacks.Location            = new Point(10, 68);
            this.dgvFeedbacks.Size                = new Size(760, 220);
            this.dgvFeedbacks.ReadOnly            = true;
            this.dgvFeedbacks.AllowUserToAddRows  = false;
            this.dgvFeedbacks.SelectionMode       = DataGridViewSelectionMode.FullRowSelect;
            this.dgvFeedbacks.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvFeedbacks.RowHeadersVisible   = false;
            this.dgvFeedbacks.MultiSelect         = false;
            this.dgvFeedbacks.SelectionChanged   += new EventHandler(this.dgvFeedbacks_SelectionChanged);

            this.lblDetailTitle.Text     = "Feedback Detail:";
            this.lblDetailTitle.Font     = new Font("Arial", 9, FontStyle.Bold);
            this.lblDetailTitle.Location = new Point(10, 296);
            this.lblDetailTitle.Size     = new Size(200, 18);

            this.rtbDetail.Font        = new Font("Courier New", 9);
            this.rtbDetail.ReadOnly    = true;
            this.rtbDetail.Location    = new Point(10, 318);
            this.rtbDetail.Size        = new Size(760, 190);
            this.rtbDetail.BorderStyle = BorderStyle.FixedSingle;

            this.btnMarkRead.Text     = "Mark as Read";
            this.btnMarkRead.Font     = new Font("Arial", 9);
            this.btnMarkRead.Location = new Point(10, 518);
            this.btnMarkRead.Size     = new Size(110, 27);
            this.btnMarkRead.Enabled  = false;
            this.btnMarkRead.Click   += new EventHandler(this.btnMarkRead_Click);

            this.btnRefresh.Text     = "Refresh";
            this.btnRefresh.Font     = new Font("Arial", 9);
            this.btnRefresh.Location = new Point(615, 518);
            this.btnRefresh.Size     = new Size(70, 27);
            this.btnRefresh.Click   += new EventHandler(this.btnRefresh_Click);

            this.btnClose.Text     = "Close";
            this.btnClose.Font     = new Font("Arial", 9);
            this.btnClose.Location = new Point(700, 518);
            this.btnClose.Size     = new Size(70, 27);
            this.btnClose.Click   += new EventHandler(this.btnClose_Click);

            this.Controls.Add(this.lblTitle);
            this.Controls.Add(line);
            this.Controls.Add(this.lblFilter);
            this.Controls.Add(this.cboFilterType);
            this.Controls.Add(this.lblCount);
            this.Controls.Add(this.dgvFeedbacks);
            this.Controls.Add(this.lblDetailTitle);
            this.Controls.Add(this.rtbDetail);
            this.Controls.Add(this.btnMarkRead);
            this.Controls.Add(this.btnRefresh);
            this.Controls.Add(this.btnClose);

            this.Load += new EventHandler(this.frmViewFeedback_Load);
            this.ResumeLayout(false);
        }
    }
}
