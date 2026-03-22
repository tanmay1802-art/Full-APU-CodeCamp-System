using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace APUCodeCamp
{
    // Feature D: View invoices and make payments
    public class frmInvoicePayment : Form
    {
        private int     selectedEnrolmentID = 0;
        private decimal selectedFee         = 0;

        private Label        lblTitle;
        private Label        lblStudentInfo;
        private Label        lblFilterLabel;
        private ComboBox     cboFilterPayment;
        private DataGridView dgvInvoices;
        private Label        lblTotalFees;
        private Label        lblTotalPaid;
        private Label        lblTotalUnpaid;
        private Button       btnMakePayment;
        private Button       btnViewInvoice;
        private Button       btnRefresh;
        private Button       btnClose;

        public frmInvoicePayment()
        {
            InitializeComponent();
        }

        private void frmInvoicePayment_Load(object sender, EventArgs e)
        {
            lblStudentInfo.Text = "Student: " + StudentSession.Name +
                                  "   TP: " + StudentSession.TPNumber;
            LoadInvoices();
        }

        private void LoadInvoices()
        {
            string statusFilter = "";
            if (cboFilterPayment.SelectedIndex > 0)
                statusFilter = " AND e.PaymentStatus = '" + cboFilterPayment.SelectedItem.ToString() + "'";

            string query =
                "SELECT " +
                "    e.EnrolmentID, " +
                "    m.ModuleCode, " +
                "    m.ModuleName, " +
                "    c.ClassLevel, " +
                "    c.Fee, " +
                "    e.MonthOfEnrolment, " +
                "    e.PaymentStatus, " +
                "    e.EnrolmentDate, " +
                "    ISNULL(CONVERT(NVARCHAR, p.PaymentDate, 103), '-') AS PaymentDate, " +
                "    ISNULL(p.PaymentMethod, '-') AS PaymentMethod " +
                "FROM Enrolments e " +
                "INNER JOIN Classes c ON e.ClassID  = c.ClassID " +
                "INNER JOIN Modules m ON c.ModuleID = m.ModuleID " +
                "LEFT  JOIN Payments p ON e.EnrolmentID = p.EnrolmentID " +
                "WHERE e.StudentID = @StudentID" + statusFilter +
                " ORDER BY e.EnrolmentDate DESC";

            SqlParameter[] p2 = { new SqlParameter("@StudentID", StudentSession.StudentID) };
            DataTable dt      = DatabaseHelper.ExecuteQuery(query, p2);

            dgvInvoices.DataSource = null;
            dgvInvoices.DataSource = dt;

            if (dgvInvoices.Columns.Count > 0)
            {
                dgvInvoices.Columns["EnrolmentID"].Visible        = false;
                dgvInvoices.Columns["ModuleCode"].HeaderText      = "Code";
                dgvInvoices.Columns["ModuleName"].HeaderText      = "Module Name";
                dgvInvoices.Columns["ClassLevel"].HeaderText      = "Level";
                dgvInvoices.Columns["Fee"].HeaderText             = "Fee (RM)";
                dgvInvoices.Columns["MonthOfEnrolment"].HeaderText= "Month";
                dgvInvoices.Columns["PaymentStatus"].HeaderText   = "Status";
                dgvInvoices.Columns["EnrolmentDate"].HeaderText   = "Enrolled On";
                dgvInvoices.Columns["PaymentDate"].HeaderText     = "Paid On";
                dgvInvoices.Columns["PaymentMethod"].HeaderText   = "Method";

                dgvInvoices.Columns["Fee"].DefaultCellStyle.Format          = "N2";
                dgvInvoices.Columns["EnrolmentDate"].DefaultCellStyle.Format = "dd/MM/yyyy";
            }

            CalculateTotals(dt);
            UpdateButtonState();
        }

        private void CalculateTotals(DataTable dt)
        {
            decimal totalFees = 0, totalPaid = 0, totalUnpaid = 0;
            foreach (DataRow row in dt.Rows)
            {
                decimal fee = Convert.ToDecimal(row["Fee"]);
                totalFees += fee;
                if (row["PaymentStatus"].ToString() == "Paid") totalPaid   += fee;
                else                                           totalUnpaid += fee;
            }
            lblTotalFees.Text   = "Total Fees: RM "   + totalFees.ToString("N2");
            lblTotalPaid.Text   = "Paid: RM "         + totalPaid.ToString("N2");
            lblTotalUnpaid.Text = "Unpaid: RM "       + totalUnpaid.ToString("N2");
        }

        private void UpdateButtonState()
        {
            btnMakePayment.Enabled = false;
            btnViewInvoice.Enabled = false;
            selectedEnrolmentID    = 0;
            selectedFee            = 0;

            if (dgvInvoices.SelectedRows.Count > 0)
            {
                string status = dgvInvoices.SelectedRows[0].Cells["PaymentStatus"].Value.ToString();
                selectedEnrolmentID    = Convert.ToInt32(dgvInvoices.SelectedRows[0].Cells["EnrolmentID"].Value);
                selectedFee            = Convert.ToDecimal(dgvInvoices.SelectedRows[0].Cells["Fee"].Value);
                btnMakePayment.Enabled = (status == "Unpaid");
                btnViewInvoice.Enabled = true;
            }
        }

        private void dgvInvoices_SelectionChanged(object sender, EventArgs e)
        {
            UpdateButtonState();
        }

        private void btnMakePayment_Click(object sender, EventArgs e)
        {
            if (selectedEnrolmentID == 0)
            {
                MessageBox.Show("Please select an unpaid row.", "No Selection",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            new frmPaymentDialog(selectedEnrolmentID, selectedFee).ShowDialog();
            LoadInvoices();
        }

        private void btnViewInvoice_Click(object sender, EventArgs e)
        {
            if (dgvInvoices.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a row.", "No Selection",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DataGridViewRow row = dgvInvoices.SelectedRows[0];

            string invoice =
                "========================================\n" +
                "    APU CODECAMP MANAGEMENT SYSTEM\n" +
                "           PAYMENT INVOICE\n" +
                "========================================\n" +
                "Printed On  : " + DateTime.Now.ToString("dd/MM/yyyy hh:mm tt") + "\n" +
                "----------------------------------------\n" +
                "Student     : " + StudentSession.Name      + "\n" +
                "TP Number   : " + StudentSession.TPNumber   + "\n" +
                "Study Level : " + StudentSession.StudyLevel + "\n" +
                "----------------------------------------\n" +
                "Module Code : " + row.Cells["ModuleCode"].Value  + "\n" +
                "Module Name : " + row.Cells["ModuleName"].Value  + "\n" +
                "Class Level : " + row.Cells["ClassLevel"].Value  + "\n" +
                "Month       : " + row.Cells["MonthOfEnrolment"].Value + "\n" +
                "Enrolled On : " + Convert.ToDateTime(row.Cells["EnrolmentDate"].Value).ToString("dd/MM/yyyy") + "\n" +
                "----------------------------------------\n" +
                "Fee         : RM " + string.Format("{0:N2}", row.Cells["Fee"].Value) + "\n" +
                "Status      : " + row.Cells["PaymentStatus"].Value + "\n" +
                "Paid On     : " + row.Cells["PaymentDate"].Value + "\n" +
                "Method      : " + row.Cells["PaymentMethod"].Value + "\n" +
                "========================================\n";

            new frmInvoiceViewer(invoice).ShowDialog();
        }

        private void cboFilterPayment_SelectedIndexChanged(object sender, EventArgs e) { LoadInvoices(); }
        private void btnRefresh_Click(object sender, EventArgs e) { LoadInvoices(); }
        private void btnClose_Click(object sender, EventArgs e)   { this.Close(); }

        private void InitializeComponent()
        {
            this.lblTitle         = new Label();
            this.lblStudentInfo   = new Label();
            this.lblFilterLabel   = new Label();
            this.cboFilterPayment = new ComboBox();
            this.dgvInvoices      = new DataGridView();
            this.lblTotalFees     = new Label();
            this.lblTotalPaid     = new Label();
            this.lblTotalUnpaid   = new Label();
            this.btnMakePayment   = new Button();
            this.btnViewInvoice   = new Button();
            this.btnRefresh       = new Button();
            this.btnClose         = new Button();

            this.SuspendLayout();

            // Form
            this.Text            = "Invoice & Payment";
            this.Size            = new Size(870, 540);
            this.StartPosition   = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox     = false;

            // Title
            this.lblTitle.Text      = "Invoice & Payment";
            this.lblTitle.Font      = new Font("Arial", 11, FontStyle.Bold);
            this.lblTitle.Location  = new Point(10, 10);
            this.lblTitle.Size      = new Size(832, 22);
            this.lblTitle.TextAlign = ContentAlignment.MiddleCenter;

            Panel line = new Panel();
            line.BorderStyle = BorderStyle.FixedSingle;
            line.Location    = new Point(10, 35);
            line.Size        = new Size(832, 1);

            // Student info
            this.lblStudentInfo.Font     = new Font("Arial", 8);
            this.lblStudentInfo.Location = new Point(10, 40);
            this.lblStudentInfo.Size     = new Size(350, 18);

            // Filter
            this.lblFilterLabel.Text     = "Filter:";
            this.lblFilterLabel.Font     = new Font("Arial", 8);
            this.lblFilterLabel.Location = new Point(570, 40);
            this.lblFilterLabel.Size     = new Size(45, 18);

            this.cboFilterPayment.Font          = new Font("Arial", 8);
            this.cboFilterPayment.DropDownStyle = ComboBoxStyle.DropDownList;
            this.cboFilterPayment.Location      = new Point(620, 38);
            this.cboFilterPayment.Size          = new Size(90, 20);
            this.cboFilterPayment.Items.AddRange(new object[] { "All", "Paid", "Unpaid" });
            this.cboFilterPayment.SelectedIndex = 0;
            this.cboFilterPayment.SelectedIndexChanged += new EventHandler(this.cboFilterPayment_SelectedIndexChanged);

            // DataGridView
            this.dgvInvoices.Location            = new Point(10, 62);
            this.dgvInvoices.Size                = new Size(832, 340);
            this.dgvInvoices.ReadOnly            = true;
            this.dgvInvoices.AllowUserToAddRows  = false;
            this.dgvInvoices.SelectionMode       = DataGridViewSelectionMode.FullRowSelect;
            this.dgvInvoices.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvInvoices.RowHeadersVisible   = false;
            this.dgvInvoices.MultiSelect         = false;
            this.dgvInvoices.SelectionChanged   += new EventHandler(this.dgvInvoices_SelectionChanged);

            // Total labels
            this.lblTotalFees.Font     = new Font("Arial", 8, FontStyle.Bold);
            this.lblTotalFees.Location = new Point(10, 410);
            this.lblTotalFees.Size     = new Size(200, 18);

            this.lblTotalPaid.Font     = new Font("Arial", 8, FontStyle.Bold);
            this.lblTotalPaid.Location = new Point(220, 410);
            this.lblTotalPaid.Size     = new Size(160, 18);

            this.lblTotalUnpaid.Font     = new Font("Arial", 8, FontStyle.Bold);
            this.lblTotalUnpaid.Location = new Point(390, 410);
            this.lblTotalUnpaid.Size     = new Size(160, 18);

            Panel line2 = new Panel();
            line2.BorderStyle = BorderStyle.FixedSingle;
            line2.Location    = new Point(10, 405);
            line2.Size        = new Size(832, 1);

            // Buttons
            this.btnMakePayment.Text     = "Make Payment";
            this.btnMakePayment.Font     = new Font("Arial", 9);
            this.btnMakePayment.Location = new Point(10, 440);
            this.btnMakePayment.Size     = new Size(110, 27);
            this.btnMakePayment.Enabled  = false;
            this.btnMakePayment.Click   += new EventHandler(this.btnMakePayment_Click);

            this.btnViewInvoice.Text     = "View Invoice";
            this.btnViewInvoice.Font     = new Font("Arial", 9);
            this.btnViewInvoice.Location = new Point(130, 440);
            this.btnViewInvoice.Size     = new Size(100, 27);
            this.btnViewInvoice.Enabled  = false;
            this.btnViewInvoice.Click   += new EventHandler(this.btnViewInvoice_Click);

            this.btnRefresh.Text     = "Refresh";
            this.btnRefresh.Font     = new Font("Arial", 9);
            this.btnRefresh.Location = new Point(688, 440);
            this.btnRefresh.Size     = new Size(70, 27);
            this.btnRefresh.Click   += new EventHandler(this.btnRefresh_Click);

            this.btnClose.Text     = "Close";
            this.btnClose.Font     = new Font("Arial", 9);
            this.btnClose.Location = new Point(762, 440);
            this.btnClose.Size     = new Size(80, 27);
            this.btnClose.Click   += new EventHandler(this.btnClose_Click);

            this.Controls.Add(this.lblTitle);
            this.Controls.Add(line);
            this.Controls.Add(this.lblStudentInfo);
            this.Controls.Add(this.lblFilterLabel);
            this.Controls.Add(this.cboFilterPayment);
            this.Controls.Add(this.dgvInvoices);
            this.Controls.Add(line2);
            this.Controls.Add(this.lblTotalFees);
            this.Controls.Add(this.lblTotalPaid);
            this.Controls.Add(this.lblTotalUnpaid);
            this.Controls.Add(this.btnMakePayment);
            this.Controls.Add(this.btnViewInvoice);
            this.Controls.Add(this.btnRefresh);
            this.Controls.Add(this.btnClose);

            this.Load += new EventHandler(this.frmInvoicePayment_Load);
            this.ResumeLayout(false);
        }
    }

    // ============================================================
    // Payment Dialog - small popup to pick payment method
    // ============================================================
    public class frmPaymentDialog : Form
    {
        private int     enrolmentID;
        private decimal feeAmount;

        private Label    lblTitle;
        private Label    lblAmountLabel;
        private Label    lblAmountValue;
        private Label    lblMethod;
        private ComboBox cboMethod;
        private Button   btnPay;
        private Button   btnCancel;

        public frmPaymentDialog(int enrolID, decimal fee)
        {
            enrolmentID = enrolID;
            feeAmount   = fee;
            InitializePaymentForm();
        }

        private void InitializePaymentForm()
        {
            this.Text            = "Make Payment";
            this.Size            = new Size(320, 220);
            this.StartPosition   = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox     = false;

            lblTitle = new Label();
            lblTitle.Text      = "Confirm Payment";
            lblTitle.Font      = new Font("Arial", 10, FontStyle.Bold);
            lblTitle.Location  = new Point(10, 12);
            lblTitle.Size      = new Size(285, 22);
            lblTitle.TextAlign = ContentAlignment.MiddleCenter;

            Panel line = new Panel();
            line.BorderStyle = BorderStyle.FixedSingle;
            line.Location    = new Point(10, 37);
            line.Size        = new Size(282, 1);

            lblAmountLabel = new Label();
            lblAmountLabel.Text     = "Amount Due:";
            lblAmountLabel.Font     = new Font("Arial", 9);
            lblAmountLabel.Location = new Point(20, 48);
            lblAmountLabel.Size     = new Size(100, 22);

            lblAmountValue = new Label();
            lblAmountValue.Text      = "RM " + feeAmount.ToString("N2");
            lblAmountValue.Font      = new Font("Arial", 10, FontStyle.Bold);
            lblAmountValue.Location  = new Point(130, 46);
            lblAmountValue.Size      = new Size(150, 24);

            lblMethod = new Label();
            lblMethod.Text     = "Method:";
            lblMethod.Font     = new Font("Arial", 9);
            lblMethod.Location = new Point(20, 85);
            lblMethod.Size     = new Size(100, 22);

            cboMethod = new ComboBox();
            cboMethod.Font          = new Font("Arial", 9);
            cboMethod.DropDownStyle = ComboBoxStyle.DropDownList;
            cboMethod.Location      = new Point(130, 83);
            cboMethod.Size          = new Size(155, 22);
            cboMethod.Items.AddRange(new object[] { "Cash", "Online Transfer", "Debit Card", "Credit Card" });
            cboMethod.SelectedIndex = 0;

            btnPay = new Button();
            btnPay.Text     = "Confirm Pay";
            btnPay.Font     = new Font("Arial", 9);
            btnPay.Location = new Point(20, 135);
            btnPay.Size     = new Size(110, 28);
            btnPay.Click   += new EventHandler(btnPay_Click);

            btnCancel = new Button();
            btnCancel.Text     = "Cancel";
            btnCancel.Font     = new Font("Arial", 9);
            btnCancel.Location = new Point(175, 135);
            btnCancel.Size     = new Size(110, 28);
            btnCancel.Click   += new EventHandler(btnCancel_Click);

            this.Controls.Add(lblTitle);
            this.Controls.Add(line);
            this.Controls.Add(lblAmountLabel);
            this.Controls.Add(lblAmountValue);
            this.Controls.Add(lblMethod);
            this.Controls.Add(cboMethod);
            this.Controls.Add(btnPay);
            this.Controls.Add(btnCancel);
        }

        private void btnPay_Click(object sender, EventArgs e)
        {
            string method = cboMethod.SelectedItem.ToString();

            string insPayment =
                "INSERT INTO Payments (EnrolmentID, Amount, PaymentMethod) " +
                "VALUES (@EnrolmentID, @Amount, @Method)";
            SqlParameter[] p1 =
            {
                new SqlParameter("@EnrolmentID", enrolmentID),
                new SqlParameter("@Amount",      feeAmount),
                new SqlParameter("@Method",      method)
            };

            string updEnrolment =
                "UPDATE Enrolments SET PaymentStatus = 'Paid' WHERE EnrolmentID = @EnrolmentID";
            SqlParameter[] p2 = { new SqlParameter("@EnrolmentID", enrolmentID) };

            DatabaseHelper.ExecuteNonQuery(insPayment, p1);
            DatabaseHelper.ExecuteNonQuery(updEnrolment, p2);

            MessageBox.Show(
                "Payment of RM " + feeAmount.ToString("N2") + " confirmed!\nMethod: " + method,
                "Payment Successful", MessageBoxButtons.OK, MessageBoxIcon.Information);

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e) { this.Close(); }
    }

    // ============================================================
    // Invoice Viewer - shows invoice text in a popup window
    // ============================================================
    public class frmInvoiceViewer : Form
    {
        private string      invoiceText;
        private RichTextBox rtbInvoice;
        private Button      btnClose;

        public frmInvoiceViewer(string text)
        {
            invoiceText = text;
            InitializeInvoiceForm();
        }

        private void InitializeInvoiceForm()
        {
            this.Text            = "Invoice";
            this.Size            = new Size(420, 460);
            this.StartPosition   = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox     = false;

            rtbInvoice = new RichTextBox();
            rtbInvoice.Text        = invoiceText;
            rtbInvoice.Font        = new Font("Courier New", 9);  // monospace keeps alignment
            rtbInvoice.ReadOnly    = true;
            rtbInvoice.Location    = new Point(10, 10);
            rtbInvoice.Size        = new Size(382, 380);
            rtbInvoice.BorderStyle = BorderStyle.FixedSingle;

            btnClose = new Button();
            btnClose.Text     = "Close";
            btnClose.Font     = new Font("Arial", 9);
            btnClose.Location = new Point(315, 398);
            btnClose.Size     = new Size(77, 27);
            btnClose.Click   += (s, ev) => this.Close();

            this.Controls.Add(rtbInvoice);
            this.Controls.Add(btnClose);
        }
    }
}
