using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace APUCodeCamp
{
    // Admin Feature A: Register new trainer and remove trainer
    public class frmManageTrainers : Form
    {
        private Label        lblTitle;
        private TabControl   tabTrainers;
        private TabPage      tabList;
        private TabPage      tabRegister;

        // List tab
        private DataGridView dgvTrainers;
        private Button       btnRemoveTrainer;
        private Button       btnRefresh;
        private Label        lblCount;

        // Register tab
        private Label    lblFullName;
        private Label    lblUsername;
        private Label    lblPassword;
        private Label    lblEmail;
        private Label    lblPhone;
        private Label    lblAddress;
        private Label    lblStaffID;
        private Label    lblSpecialisation;
        private TextBox  txtFullName;
        private TextBox  txtUsername;
        private TextBox  txtPassword;
        private TextBox  txtEmail;
        private TextBox  txtPhone;
        private TextBox  txtAddress;
        private TextBox  txtStaffID;
        private TextBox  txtSpecialisation;
        private Button   btnRegister;
        private Button   btnClearReg;

        private Button btnClose;

        public frmManageTrainers()
        {
            InitializeComponent();
        }

        private void frmManageTrainers_Load(object sender, EventArgs e)
        {
            LoadTrainers();
        }

        private void LoadTrainers()
        {
            string query =
                "SELECT t.TrainerID, t.StaffID, u.Name, u.Email, u.Phone, " +
                "       t.Specialisation, u.IsActive " +
                "FROM Trainers t INNER JOIN Users u ON t.UserID = u.UserID " +
                "ORDER BY u.Name";
            DataTable dt = DatabaseHelper.ExecuteQuery(query);

            dgvTrainers.DataSource = null;
            dgvTrainers.DataSource = dt;

            if (dgvTrainers.Columns.Count > 0)
            {
                dgvTrainers.Columns["TrainerID"].Visible         = false;
                dgvTrainers.Columns["StaffID"].HeaderText        = "Staff ID";
                dgvTrainers.Columns["Name"].HeaderText           = "Full Name";
                dgvTrainers.Columns["Email"].HeaderText          = "Email";
                dgvTrainers.Columns["Phone"].HeaderText          = "Phone";
                dgvTrainers.Columns["Specialisation"].HeaderText = "Specialisation";
                dgvTrainers.Columns["IsActive"].HeaderText       = "Active";
            }

            lblCount.Text = "Total: " + dt.Rows.Count + " trainer(s)";
            btnRemoveTrainer.Enabled = false;
        }

        private void dgvTrainers_SelectionChanged(object sender, EventArgs e)
        {
            btnRemoveTrainer.Enabled = dgvTrainers.SelectedRows.Count > 0;
        }

        private void btnRemoveTrainer_Click(object sender, EventArgs e)
        {
            if (dgvTrainers.SelectedRows.Count == 0) return;

            DataGridViewRow row       = dgvTrainers.SelectedRows[0];
            int             trainerID = Convert.ToInt32(row.Cells["TrainerID"].Value);
            string          name      = row.Cells["Name"].Value.ToString();

            if (MessageBox.Show("Remove trainer: " + name + "?\n\nThis will deactivate the account.",
                "Confirm Remove", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.No)
                return;

            string query =
                "UPDATE Users SET IsActive = 0 WHERE UserID = " +
                "(SELECT UserID FROM Trainers WHERE TrainerID = @TrainerID)";
            SqlParameter[] p = { new SqlParameter("@TrainerID", trainerID) };
            if (DatabaseHelper.ExecuteNonQuery(query, p) > 0)
            {
                MessageBox.Show(name + " has been removed.", "Done",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadTrainers();
            }
        }

        private void btnRegister_Click(object sender, EventArgs e)
        {
            if (txtFullName.Text.Trim() == "" || txtUsername.Text.Trim() == "" ||
                txtPassword.Text.Trim() == "" || txtEmail.Text.Trim() == "" ||
                txtPhone.Text.Trim() == ""    || txtStaffID.Text.Trim() == "")
            {
                MessageBox.Show("Please fill in all required fields.", "Validation",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (!txtEmail.Text.Contains("@"))
            {
                MessageBox.Show("Please enter a valid email address.", "Validation",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string checkUser = "SELECT COUNT(*) FROM Users WHERE Username = @Username";
            SqlParameter[] pCheck = { new SqlParameter("@Username", txtUsername.Text.Trim()) };
            if (Convert.ToInt32(DatabaseHelper.ExecuteScalar(checkUser, pCheck)) > 0)
            {
                MessageBox.Show("Username already exists.", "Duplicate",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string checkStaff = "SELECT COUNT(*) FROM Trainers WHERE StaffID = @StaffID";
            SqlParameter[] pStaff = { new SqlParameter("@StaffID", txtStaffID.Text.Trim()) };
            if (Convert.ToInt32(DatabaseHelper.ExecuteScalar(checkStaff, pStaff)) > 0)
            {
                MessageBox.Show("Staff ID already exists.", "Duplicate",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string insUser =
                "INSERT INTO Users (Username, Password, Role, Name, Email, Phone, Address) " +
                "OUTPUT INSERTED.UserID " +
                "VALUES (@Username, @Password, 'Trainer', @Name, @Email, @Phone, @Address)";
            SqlParameter[] pUser =
            {
                new SqlParameter("@Username", txtUsername.Text.Trim()),
                new SqlParameter("@Password", txtPassword.Text.Trim()),
                new SqlParameter("@Name",     txtFullName.Text.Trim()),
                new SqlParameter("@Email",    txtEmail.Text.Trim()),
                new SqlParameter("@Phone",    txtPhone.Text.Trim()),
                new SqlParameter("@Address",  txtAddress.Text.Trim() == "" ? "N/A" : txtAddress.Text.Trim())
            };
            object newUserID = DatabaseHelper.ExecuteScalar(insUser, pUser);
            if (newUserID == null) return;

            string insTrainer =
                "INSERT INTO Trainers (UserID, StaffID, Specialisation) VALUES (@UserID, @StaffID, @Spec)";
            SqlParameter[] pTrainer =
            {
                new SqlParameter("@UserID",  Convert.ToInt32(newUserID)),
                new SqlParameter("@StaffID", txtStaffID.Text.Trim()),
                new SqlParameter("@Spec",    txtSpecialisation.Text.Trim())
            };
            DatabaseHelper.ExecuteNonQuery(insTrainer, pTrainer);

            MessageBox.Show("Trainer registered successfully!", "Success",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            ClearRegForm();
            LoadTrainers();
            tabTrainers.SelectedTab = tabList;
        }

        private void ClearRegForm()
        {
            txtFullName.Clear(); txtUsername.Clear(); txtPassword.Clear();
            txtEmail.Clear();    txtPhone.Clear();    txtAddress.Clear();
            txtStaffID.Clear();  txtSpecialisation.Clear();
        }

        private void btnClearReg_Click(object sender, EventArgs e) { ClearRegForm(); }
        private void btnRefresh_Click(object sender, EventArgs e)  { LoadTrainers(); }
        private void btnClose_Click(object sender, EventArgs e)    { this.Close(); }

        private void InitializeComponent()
        {
            this.lblTitle            = new Label();
            this.tabTrainers         = new TabControl();
            this.tabList             = new TabPage();
            this.tabRegister         = new TabPage();
            this.dgvTrainers         = new DataGridView();
            this.btnRemoveTrainer    = new Button();
            this.btnRefresh          = new Button();
            this.lblCount            = new Label();
            this.lblFullName         = new Label();
            this.txtFullName         = new TextBox();
            this.lblUsername         = new Label();
            this.txtUsername         = new TextBox();
            this.lblPassword         = new Label();
            this.txtPassword         = new TextBox();
            this.lblEmail            = new Label();
            this.txtEmail            = new TextBox();
            this.lblPhone            = new Label();
            this.txtPhone            = new TextBox();
            this.lblAddress          = new Label();
            this.txtAddress          = new TextBox();
            this.lblStaffID          = new Label();
            this.txtStaffID          = new TextBox();
            this.lblSpecialisation   = new Label();
            this.txtSpecialisation   = new TextBox();
            this.btnRegister         = new Button();
            this.btnClearReg         = new Button();
            this.btnClose            = new Button();

            this.SuspendLayout();

            this.Text            = "Manage Trainers";
            this.Size            = new Size(760, 540);
            this.StartPosition   = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox     = false;

            // Title
            this.lblTitle.Text      = "Manage Trainers";
            this.lblTitle.Font      = new Font("Arial", 11, FontStyle.Bold);
            this.lblTitle.Location  = new Point(10, 10);
            this.lblTitle.Size      = new Size(720, 22);
            this.lblTitle.TextAlign = ContentAlignment.MiddleCenter;

            // TabControl
            this.tabTrainers.Location = new Point(10, 40);
            this.tabTrainers.Size     = new Size(720, 450);

            // ── LIST TAB ──
            this.tabList.Text = "Trainer List";

            this.lblCount.Font     = new Font("Arial", 8);
            this.lblCount.Location = new Point(5, 10);
            this.lblCount.Size     = new Size(200, 18);

            this.btnRefresh.Text     = "Refresh";
            this.btnRefresh.Font     = new Font("Arial", 8);
            this.btnRefresh.Location = new Point(540, 8);
            this.btnRefresh.Size     = new Size(70, 24);
            this.btnRefresh.Click   += new EventHandler(this.btnRefresh_Click);

            this.dgvTrainers.Location            = new Point(5, 35);
            this.dgvTrainers.Size                = new Size(700, 340);
            this.dgvTrainers.ReadOnly            = true;
            this.dgvTrainers.AllowUserToAddRows  = false;
            this.dgvTrainers.SelectionMode       = DataGridViewSelectionMode.FullRowSelect;
            this.dgvTrainers.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvTrainers.RowHeadersVisible   = false;
            this.dgvTrainers.MultiSelect         = false;
            this.dgvTrainers.SelectionChanged   += new EventHandler(this.dgvTrainers_SelectionChanged);

            this.btnRemoveTrainer.Text     = "Remove Trainer";
            this.btnRemoveTrainer.Font     = new Font("Arial", 9);
            this.btnRemoveTrainer.Location = new Point(5, 385);
            this.btnRemoveTrainer.Size     = new Size(120, 27);
            this.btnRemoveTrainer.Enabled  = false;
            this.btnRemoveTrainer.Click   += new EventHandler(this.btnRemoveTrainer_Click);

            this.tabList.Controls.Add(this.lblCount);
            this.tabList.Controls.Add(this.btnRefresh);
            this.tabList.Controls.Add(this.dgvTrainers);
            this.tabList.Controls.Add(this.btnRemoveTrainer);

            // ── REGISTER TAB ──
            this.tabRegister.Text = "Register Trainer";

            // Full Name
            this.lblFullName.Text     = "Full Name: *";
            this.lblFullName.Font     = new Font("Arial", 9);
            this.lblFullName.Location = new Point(15, 20);
            this.lblFullName.Size     = new Size(140, 22);
            this.txtFullName.Font     = new Font("Arial", 9);
            this.txtFullName.Location = new Point(165, 18);
            this.txtFullName.Size     = new Size(250, 22);

            // Username
            this.lblUsername.Text     = "Username: *";
            this.lblUsername.Font     = new Font("Arial", 9);
            this.lblUsername.Location = new Point(15, 58);
            this.lblUsername.Size     = new Size(140, 22);
            this.txtUsername.Font     = new Font("Arial", 9);
            this.txtUsername.Location = new Point(165, 56);
            this.txtUsername.Size     = new Size(250, 22);

            // Password
            this.lblPassword.Text     = "Password: *";
            this.lblPassword.Font     = new Font("Arial", 9);
            this.lblPassword.Location = new Point(15, 96);
            this.lblPassword.Size     = new Size(140, 22);
            this.txtPassword.Font                  = new Font("Arial", 9);
            this.txtPassword.Location              = new Point(165, 94);
            this.txtPassword.Size                  = new Size(250, 22);
            this.txtPassword.UseSystemPasswordChar = true;

            // Staff ID
            this.lblStaffID.Text     = "Staff ID: *";
            this.lblStaffID.Font     = new Font("Arial", 9);
            this.lblStaffID.Location = new Point(15, 134);
            this.lblStaffID.Size     = new Size(140, 22);
            this.txtStaffID.Font     = new Font("Arial", 9);
            this.txtStaffID.Location = new Point(165, 132);
            this.txtStaffID.Size     = new Size(250, 22);

            // Email
            this.lblEmail.Text     = "Email: *";
            this.lblEmail.Font     = new Font("Arial", 9);
            this.lblEmail.Location = new Point(15, 172);
            this.lblEmail.Size     = new Size(140, 22);
            this.txtEmail.Font     = new Font("Arial", 9);
            this.txtEmail.Location = new Point(165, 170);
            this.txtEmail.Size     = new Size(250, 22);

            // Phone
            this.lblPhone.Text     = "Phone: *";
            this.lblPhone.Font     = new Font("Arial", 9);
            this.lblPhone.Location = new Point(15, 210);
            this.lblPhone.Size     = new Size(140, 22);
            this.txtPhone.Font     = new Font("Arial", 9);
            this.txtPhone.Location = new Point(165, 208);
            this.txtPhone.Size     = new Size(250, 22);

            // Specialisation
            this.lblSpecialisation.Text     = "Specialisation:";
            this.lblSpecialisation.Font     = new Font("Arial", 9);
            this.lblSpecialisation.Location = new Point(15, 248);
            this.lblSpecialisation.Size     = new Size(140, 22);
            this.txtSpecialisation.Font     = new Font("Arial", 9);
            this.txtSpecialisation.Location = new Point(165, 246);
            this.txtSpecialisation.Size     = new Size(250, 22);

            // Address
            this.lblAddress.Text     = "Address:";
            this.lblAddress.Font     = new Font("Arial", 9);
            this.lblAddress.Location = new Point(15, 286);
            this.lblAddress.Size     = new Size(140, 22);
            this.txtAddress.Font     = new Font("Arial", 9);
            this.txtAddress.Location = new Point(165, 284);
            this.txtAddress.Size     = new Size(250, 22);

            // Register button
            this.btnRegister.Text     = "Register Trainer";
            this.btnRegister.Font     = new Font("Arial", 9);
            this.btnRegister.Location = new Point(15, 325);
            this.btnRegister.Size     = new Size(130, 27);
            this.btnRegister.Click   += new EventHandler(this.btnRegister_Click);

            // Clear button
            this.btnClearReg.Text     = "Clear";
            this.btnClearReg.Font     = new Font("Arial", 9);
            this.btnClearReg.Location = new Point(155, 325);
            this.btnClearReg.Size     = new Size(80, 27);
            this.btnClearReg.Click   += new EventHandler(this.btnClearReg_Click);

            // Required note
            Label lblReq = new Label();
            lblReq.Text     = "* Required fields";
            lblReq.Font     = new Font("Arial", 8, FontStyle.Italic);
            lblReq.Location = new Point(15, 360);
            lblReq.Size     = new Size(200, 18);

            this.tabRegister.Controls.Add(this.lblFullName);
            this.tabRegister.Controls.Add(this.txtFullName);
            this.tabRegister.Controls.Add(this.lblUsername);
            this.tabRegister.Controls.Add(this.txtUsername);
            this.tabRegister.Controls.Add(this.lblPassword);
            this.tabRegister.Controls.Add(this.txtPassword);
            this.tabRegister.Controls.Add(this.lblStaffID);
            this.tabRegister.Controls.Add(this.txtStaffID);
            this.tabRegister.Controls.Add(this.lblEmail);
            this.tabRegister.Controls.Add(this.txtEmail);
            this.tabRegister.Controls.Add(this.lblPhone);
            this.tabRegister.Controls.Add(this.txtPhone);
            this.tabRegister.Controls.Add(this.lblSpecialisation);
            this.tabRegister.Controls.Add(this.txtSpecialisation);
            this.tabRegister.Controls.Add(this.lblAddress);
            this.tabRegister.Controls.Add(this.txtAddress);
            this.tabRegister.Controls.Add(this.btnRegister);
            this.tabRegister.Controls.Add(this.btnClearReg);
            this.tabRegister.Controls.Add(lblReq);

            this.tabTrainers.TabPages.Add(this.tabList);
            this.tabTrainers.TabPages.Add(this.tabRegister);

            // Close button
            this.btnClose.Text     = "Close";
            this.btnClose.Font     = new Font("Arial", 9);
            this.btnClose.Location = new Point(660, 498);
            this.btnClose.Size     = new Size(70, 27);
            this.btnClose.Click   += new EventHandler(this.btnClose_Click);

            this.Controls.Add(this.lblTitle);
            this.Controls.Add(this.tabTrainers);
            this.Controls.Add(this.btnClose);

            this.Load += new EventHandler(this.frmManageTrainers_Load);
            this.ResumeLayout(false);
        }
    }
}
