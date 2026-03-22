using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace APUCodeCamp
{
    // Admin Feature D: Monthly income report grouped by trainer
    public class frmMonthlyReport : Form
    {
        private Label        lblTitle;
        private Label        lblYear;
        private Label        lblMonth;
        private ComboBox     cboYear;
        private ComboBox     cboMonth;
        private Button       btnGenerate;
        private DataGridView dgvReport;
        private Label        lblTotal;
        private Button       btnClose;

        public frmMonthlyReport()
        {
            InitializeComponent();
        }

        private void frmMonthlyReport_Load(object sender, EventArgs e)
        {
            // Populate year combo (current year ±2)
            int currentYear = DateTime.Now.Year;
            for (int y = currentYear - 2; y <= currentYear + 1; y++)
                cboYear.Items.Add(y.ToString());
            cboYear.SelectedItem = currentYear.ToString();

            // Populate month combo
            string[] months = { "All Months","January","February","March","April","May","June",
                                 "July","August","September","October","November","December" };
            cboMonth.Items.AddRange(months);
            cboMonth.SelectedIndex = 0;

            GenerateReport();
        }

        private void GenerateReport()
        {
            string year  = cboYear.SelectedItem.ToString();
            string month = cboMonth.SelectedItem.ToString();

            string monthFilter = "";
            if (cboMonth.SelectedIndex > 0)
                monthFilter = " AND MONTH(p.PaymentDate) = " + cboMonth.SelectedIndex;

            string query =
                "SELECT " +
                "    u.Name AS Trainer, " +
                "    t.StaffID, " +
                "    m.ModuleCode, " +
                "    c.ClassLevel, " +
                "    COUNT(p.PaymentID) AS PaymentCount, " +
                "    SUM(p.Amount) AS TotalIncome " +
                "FROM Payments p " +
                "INNER JOIN Enrolments e  ON p.EnrolmentID = e.EnrolmentID " +
                "INNER JOIN Classes    c  ON e.ClassID     = c.ClassID " +
                "INNER JOIN Modules    m  ON c.ModuleID    = m.ModuleID " +
                "INNER JOIN Trainers   t  ON c.TrainerID   = t.TrainerID " +
                "INNER JOIN Users      u  ON t.UserID      = u.UserID " +
                "WHERE YEAR(p.PaymentDate) = " + year + monthFilter +
                " GROUP BY u.Name, t.StaffID, m.ModuleCode, c.ClassLevel " +
                " ORDER BY TotalIncome DESC";

            DataTable dt = DatabaseHelper.ExecuteQuery(query);
            dgvReport.DataSource = null;
            dgvReport.DataSource = dt;

            if (dgvReport.Columns.Count > 0)
            {
                dgvReport.Columns["Trainer"].HeaderText      = "Trainer";
                dgvReport.Columns["StaffID"].HeaderText      = "Staff ID";
                dgvReport.Columns["ModuleCode"].HeaderText   = "Module";
                dgvReport.Columns["ClassLevel"].HeaderText   = "Level";
                dgvReport.Columns["PaymentCount"].HeaderText = "Payments";
                dgvReport.Columns["TotalIncome"].HeaderText  = "Income (RM)";
                dgvReport.Columns["TotalIncome"].DefaultCellStyle.Format = "N2";
            }

            // Calculate grand total
            decimal total = 0;
            foreach (DataRow row in dt.Rows)
                total += Convert.ToDecimal(row["TotalIncome"]);

            lblTotal.Text = "Grand Total Income: RM " + total.ToString("N2") +
                            "  |  Records: " + dt.Rows.Count +
                            "  |  Period: " + month + " " + year;
        }

        private void btnGenerate_Click(object sender, EventArgs e) { GenerateReport(); }
        private void btnClose_Click(object sender, EventArgs e)    { this.Close(); }

        private void InitializeComponent()
        {
            this.lblTitle    = new Label();
            this.lblYear     = new Label(); this.cboYear  = new ComboBox();
            this.lblMonth    = new Label(); this.cboMonth = new ComboBox();
            this.btnGenerate = new Button();
            this.dgvReport   = new DataGridView();
            this.lblTotal    = new Label();
            this.btnClose    = new Button();

            this.SuspendLayout();

            this.Text            = "Monthly Income Report";
            this.Size            = new Size(760, 540);
            this.StartPosition   = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox     = false;

            this.lblTitle.Text      = "Monthly Income Report";
            this.lblTitle.Font      = new Font("Arial", 11, FontStyle.Bold);
            this.lblTitle.Location  = new Point(10, 10);
            this.lblTitle.Size      = new Size(720, 22);
            this.lblTitle.TextAlign = ContentAlignment.MiddleCenter;

            Panel line = new Panel();
            line.BorderStyle = BorderStyle.FixedSingle;
            line.Location    = new Point(10, 35);
            line.Size        = new Size(720, 1);

            this.lblYear.Text     = "Year:";
            this.lblYear.Font     = new Font("Arial", 9);
            this.lblYear.Location = new Point(20, 48);
            this.lblYear.Size     = new Size(50, 22);
            this.cboYear.Font          = new Font("Arial", 9);
            this.cboYear.DropDownStyle = ComboBoxStyle.DropDownList;
            this.cboYear.Location      = new Point(72, 46);
            this.cboYear.Size          = new Size(80, 22);

            this.lblMonth.Text     = "Month:";
            this.lblMonth.Font     = new Font("Arial", 9);
            this.lblMonth.Location = new Point(170, 48);
            this.lblMonth.Size     = new Size(55, 22);
            this.cboMonth.Font          = new Font("Arial", 9);
            this.cboMonth.DropDownStyle = ComboBoxStyle.DropDownList;
            this.cboMonth.Location      = new Point(230, 46);
            this.cboMonth.Size          = new Size(130, 22);

            this.btnGenerate.Text     = "Generate Report";
            this.btnGenerate.Font     = new Font("Arial", 9);
            this.btnGenerate.Location = new Point(375, 44);
            this.btnGenerate.Size     = new Size(120, 26);
            this.btnGenerate.Click   += new EventHandler(this.btnGenerate_Click);

            this.dgvReport.Location            = new Point(10, 80);
            this.dgvReport.Size                = new Size(720, 400);
            this.dgvReport.ReadOnly            = true;
            this.dgvReport.AllowUserToAddRows  = false;
            this.dgvReport.SelectionMode       = DataGridViewSelectionMode.FullRowSelect;
            this.dgvReport.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvReport.RowHeadersVisible   = false;

            this.lblTotal.Font      = new Font("Arial", 9, FontStyle.Bold);
            this.lblTotal.Location  = new Point(10, 488);
            this.lblTotal.Size      = new Size(620, 20);

            this.btnClose.Text     = "Close";
            this.btnClose.Font     = new Font("Arial", 9);
            this.btnClose.Location = new Point(660, 485);
            this.btnClose.Size     = new Size(70, 27);
            this.btnClose.Click   += new EventHandler(this.btnClose_Click);

            this.Controls.Add(this.lblTitle);
            this.Controls.Add(line);
            this.Controls.Add(this.lblYear);  this.Controls.Add(this.cboYear);
            this.Controls.Add(this.lblMonth); this.Controls.Add(this.cboMonth);
            this.Controls.Add(this.btnGenerate);
            this.Controls.Add(this.dgvReport);
            this.Controls.Add(this.lblTotal);
            this.Controls.Add(this.btnClose);

            this.Load += new EventHandler(this.frmMonthlyReport_Load);
            this.ResumeLayout(false);
        }
    }
}
