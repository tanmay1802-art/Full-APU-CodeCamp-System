using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace APUCodeCamp
{
    // ============================================================
    // Trainer Dashboard
    // ============================================================
    public class frmTrainerDashboard : Form
    {
        private Label  lblTitle;
        private Label  lblWelcome;
        private Label  lblDateTime;
        private Button btnManageClasses;
        private Button btnViewEnrolledStudents;
        private Button btnSendFeedback;
        private Button btnUpdateProfile;
        private Button btnLogout;
        private System.Windows.Forms.Timer tmrClock;

        public frmTrainerDashboard()
        {
            InitializeComponent();
        }

        private void frmTrainerDashboard_Load(object sender, EventArgs e)
        {
            lblWelcome.Text  = "Welcome, " + UserSession.Name +
                               "  |  Staff: " + UserSession.TrainerStaffID;
            lblDateTime.Text = DateTime.Now.ToString("dd/MM/yyyy  hh:mm tt");
        }

        private void tmrClock_Tick(object sender, EventArgs e)
        {
            lblDateTime.Text = DateTime.Now.ToString("dd/MM/yyyy  hh:mm tt");
        }

        private void btnManageClasses_Click(object sender, EventArgs e)        { new frmManageClasses().ShowDialog(); }
        private void btnViewEnrolledStudents_Click(object sender, EventArgs e) { new frmViewEnrolledStudents().ShowDialog(); }
        private void btnSendFeedback_Click(object sender, EventArgs e)         { new frmSendFeedback().ShowDialog(); }

        private void btnUpdateProfile_Click(object sender, EventArgs e)
        {
            new frmUpdateProfile().ShowDialog();
            lblWelcome.Text = "Welcome, " + UserSession.Name + "  |  Staff: " + UserSession.TrainerStaffID;
        }

        private void btnLogout_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to logout?", "Confirm Logout",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                UserSession.ClearSession();
                new frmLogin().Show();
                this.Close();
            }
        }

        private void InitializeComponent()
        {
            this.lblTitle                = new Label();
            this.lblWelcome              = new Label();
            this.lblDateTime             = new Label();
            this.btnManageClasses        = new Button();
            this.btnViewEnrolledStudents = new Button();
            this.btnSendFeedback         = new Button();
            this.btnUpdateProfile        = new Button();
            this.btnLogout               = new Button();
            this.tmrClock                = new System.Windows.Forms.Timer();

            this.SuspendLayout();

            this.Text            = "Trainer Dashboard";
            this.Size            = new Size(500, 380);
            this.StartPosition   = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox     = false;

            // Title
            this.lblTitle.Text      = "APU CodeCamp - Trainer Portal";
            this.lblTitle.Font      = new Font("Arial", 12, FontStyle.Bold);
            this.lblTitle.Location  = new Point(10, 15);
            this.lblTitle.Size      = new Size(463, 25);
            this.lblTitle.TextAlign = ContentAlignment.MiddleCenter;

            // Welcome
            this.lblWelcome.Text     = "Welcome";
            this.lblWelcome.Font     = new Font("Arial", 8);
            this.lblWelcome.Location = new Point(10, 48);
            this.lblWelcome.Size     = new Size(350, 18);

            // DateTime
            this.lblDateTime.Font      = new Font("Arial", 8);
            this.lblDateTime.Location  = new Point(330, 48);
            this.lblDateTime.Size      = new Size(143, 18);
            this.lblDateTime.TextAlign = ContentAlignment.MiddleRight;

            // Button 1
            this.btnManageClasses.Text      = "1.  Manage My Classes";
            this.btnManageClasses.Font      = new Font("Arial", 10);
            this.btnManageClasses.TextAlign = ContentAlignment.MiddleLeft;
            this.btnManageClasses.Location  = new Point(80, 80);
            this.btnManageClasses.Size      = new Size(320, 38);
            this.btnManageClasses.Click    += new EventHandler(this.btnManageClasses_Click);

            // Button 2
            this.btnViewEnrolledStudents.Text      = "2.  View Enrolled & Paid Students";
            this.btnViewEnrolledStudents.Font      = new Font("Arial", 10);
            this.btnViewEnrolledStudents.TextAlign = ContentAlignment.MiddleLeft;
            this.btnViewEnrolledStudents.Location  = new Point(80, 128);
            this.btnViewEnrolledStudents.Size      = new Size(320, 38);
            this.btnViewEnrolledStudents.Click    += new EventHandler(this.btnViewEnrolledStudents_Click);

            // Button 3
            this.btnSendFeedback.Text      = "3.  Send Feedback to Administrator";
            this.btnSendFeedback.Font      = new Font("Arial", 10);
            this.btnSendFeedback.TextAlign = ContentAlignment.MiddleLeft;
            this.btnSendFeedback.Location  = new Point(80, 176);
            this.btnSendFeedback.Size      = new Size(320, 38);
            this.btnSendFeedback.Click    += new EventHandler(this.btnSendFeedback_Click);

            // Button 4
            this.btnUpdateProfile.Text      = "4.  Update My Profile";
            this.btnUpdateProfile.Font      = new Font("Arial", 10);
            this.btnUpdateProfile.TextAlign = ContentAlignment.MiddleLeft;
            this.btnUpdateProfile.Location  = new Point(80, 224);
            this.btnUpdateProfile.Size      = new Size(320, 38);
            this.btnUpdateProfile.Click    += new EventHandler(this.btnUpdateProfile_Click);

            // Logout
            this.btnLogout.Text     = "Logout";
            this.btnLogout.Font     = new Font("Arial", 9);
            this.btnLogout.Location = new Point(370, 280);
            this.btnLogout.Size     = new Size(100, 28);
            this.btnLogout.Click   += new EventHandler(this.btnLogout_Click);

            // Timer
            this.tmrClock.Interval = 1000;
            this.tmrClock.Enabled  = true;
            this.tmrClock.Tick    += new EventHandler(this.tmrClock_Tick);

            this.Controls.Add(this.lblTitle);
            this.Controls.Add(this.lblWelcome);
            this.Controls.Add(this.lblDateTime);
            this.Controls.Add(this.btnManageClasses);
            this.Controls.Add(this.btnViewEnrolledStudents);
            this.Controls.Add(this.btnSendFeedback);
            this.Controls.Add(this.btnUpdateProfile);
            this.Controls.Add(this.btnLogout);

            this.Load += new EventHandler(this.frmTrainerDashboard_Load);
            this.ResumeLayout(false);
        }
    }

    // ============================================================
    // Trainer Feature A/B: Manage Classes (Add, Update, Delete)
    // ============================================================
    public class frmManageClasses : Form
    {
        private Label        lblTitle;
        private Label        lblInfo;
        private TabControl   tabClasses;
        private TabPage      tabList;
        private TabPage      tabAddEdit;

        // List tab
        private DataGridView dgvClasses;
        private Button       btnEdit;
        private Button       btnDeleteClass;
        private Button       btnAddNew;
        private Button       btnRefreshList;
        private Label        lblCount;

        // Add/Edit tab
        private Label          lblFormTitle;
        private Label          lblModule;
        private ComboBox       cboModule;
        private Label          lblLevel;
        private ComboBox       cboLevel;
        private Label          lblSchedule;
        private TextBox        txtSchedule;
        private Label          lblVenue;
        private TextBox        txtVenue;
        private Label          lblFee;
        private TextBox        txtFee;
        private Label          lblMax;
        private TextBox        txtMax;
        private Label          lblStart;
        private DateTimePicker dtpStart;
        private Label          lblEnd;
        private DateTimePicker dtpEnd;
        private Button         btnSave;
        private Button         btnClearForm;

        private Button btnClose;
        private int    editingClassID = 0;

        public frmManageClasses()
        {
            InitializeComponent();
        }

        private void frmManageClasses_Load(object sender, EventArgs e)
        {
            lblInfo.Text = "Trainer: " + UserSession.Name + "  |  Staff: " + UserSession.TrainerStaffID;
            LoadModuleCombo();
            LoadClasses();
        }

        private void LoadModuleCombo()
        {
            DataTable dt = DatabaseHelper.ExecuteQuery(
                "SELECT m.ModuleID, m.ModuleCode + ' - ' + m.ModuleName AS Display " +
                "FROM TrainerModules tm " +
                "INNER JOIN Modules m ON tm.ModuleID = m.ModuleID " +
                "WHERE tm.TrainerID = @TrainerID AND tm.IsActive = 1",
                new SqlParameter[] { new SqlParameter("@TrainerID", UserSession.TrainerID) });
            cboModule.DataSource    = dt;
            cboModule.DisplayMember = "Display";
            cboModule.ValueMember   = "ModuleID";
            if (cboModule.Items.Count > 0) cboModule.SelectedIndex = 0;
        }

        private void LoadClasses()
        {
            string query =
                "SELECT c.ClassID, m.ModuleCode, m.ModuleName, c.ClassLevel, " +
                "       c.Schedule, c.Venue, c.Fee, c.StartDate, c.EndDate, " +
                "       c.MaxStudents, c.IsActive " +
                "FROM Classes c INNER JOIN Modules m ON c.ModuleID = m.ModuleID " +
                "WHERE c.TrainerID = @TrainerID ORDER BY c.StartDate DESC";
            SqlParameter[] p = { new SqlParameter("@TrainerID", UserSession.TrainerID) };
            DataTable dt     = DatabaseHelper.ExecuteQuery(query, p);

            dgvClasses.DataSource = null;
            dgvClasses.DataSource = dt;

            if (dgvClasses.Columns.Count > 0)
            {
                dgvClasses.Columns["ClassID"].Visible       = false;
                dgvClasses.Columns["ModuleCode"].HeaderText = "Code";
                dgvClasses.Columns["ModuleName"].HeaderText = "Module Name";
                dgvClasses.Columns["ClassLevel"].HeaderText = "Level";
                dgvClasses.Columns["Schedule"].HeaderText   = "Schedule";
                dgvClasses.Columns["Venue"].HeaderText      = "Venue";
                dgvClasses.Columns["Fee"].HeaderText        = "Fee (RM)";
                dgvClasses.Columns["StartDate"].HeaderText  = "Start";
                dgvClasses.Columns["EndDate"].HeaderText    = "End";
                dgvClasses.Columns["MaxStudents"].HeaderText= "Max";
                dgvClasses.Columns["IsActive"].HeaderText   = "Active";
                dgvClasses.Columns["Fee"].DefaultCellStyle.Format       = "N2";
                dgvClasses.Columns["StartDate"].DefaultCellStyle.Format = "dd/MM/yyyy";
                dgvClasses.Columns["EndDate"].DefaultCellStyle.Format   = "dd/MM/yyyy";
            }

            lblCount.Text          = "Total: " + dt.Rows.Count + " class(es)";
            btnEdit.Enabled        = false;
            btnDeleteClass.Enabled = false;
        }

        private void dgvClasses_SelectionChanged(object sender, EventArgs e)
        {
            btnEdit.Enabled        = dgvClasses.SelectedRows.Count > 0;
            btnDeleteClass.Enabled = dgvClasses.SelectedRows.Count > 0;
        }

        private void btnAddNew_Click(object sender, EventArgs e)
        {
            editingClassID    = 0;
            lblFormTitle.Text = "Add New Class";
            ClearAddForm();
            tabClasses.SelectedTab = tabAddEdit;
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (dgvClasses.SelectedRows.Count == 0) return;
            DataGridViewRow row = dgvClasses.SelectedRows[0];
            editingClassID    = Convert.ToInt32(row.Cells["ClassID"].Value);
            lblFormTitle.Text = "Edit Class (ID: " + editingClassID + ")";

            for (int i = 0; i < cboLevel.Items.Count; i++)
                if (cboLevel.Items[i].ToString() == row.Cells["ClassLevel"].Value.ToString())
                { cboLevel.SelectedIndex = i; break; }

            txtSchedule.Text = row.Cells["Schedule"].Value.ToString();
            txtVenue.Text    = row.Cells["Venue"].Value.ToString();
            txtFee.Text      = string.Format("{0:N2}", row.Cells["Fee"].Value);
            txtMax.Text      = row.Cells["MaxStudents"].Value.ToString();
            dtpStart.Value   = Convert.ToDateTime(row.Cells["StartDate"].Value);
            dtpEnd.Value     = Convert.ToDateTime(row.Cells["EndDate"].Value);

            tabClasses.SelectedTab = tabAddEdit;
        }

        private void btnDeleteClass_Click(object sender, EventArgs e)
        {
            if (dgvClasses.SelectedRows.Count == 0) return;
            int classID = Convert.ToInt32(dgvClasses.SelectedRows[0].Cells["ClassID"].Value);

            if (MessageBox.Show("Delete this class? It will be set as inactive.",
                "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.No) return;

            string upd = "UPDATE Classes SET IsActive = 0 WHERE ClassID = @ID AND TrainerID = @TID";
            SqlParameter[] p =
            {
                new SqlParameter("@ID",  classID),
                new SqlParameter("@TID", UserSession.TrainerID)
            };
            if (DatabaseHelper.ExecuteNonQuery(upd, p) > 0)
            {
                MessageBox.Show("Class deleted.", "Done", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadClasses();
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (cboModule.SelectedIndex < 0 || cboLevel.SelectedIndex < 0 ||
                txtSchedule.Text.Trim() == "" || txtVenue.Text.Trim() == "" || txtFee.Text.Trim() == "")
            {
                MessageBox.Show("Please fill in all required fields.", "Validation",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            decimal fee;
            if (!decimal.TryParse(txtFee.Text.Trim(), out fee) || fee <= 0)
            {
                MessageBox.Show("Please enter a valid fee amount.", "Validation",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int max = 20;
            int.TryParse(txtMax.Text.Trim(), out max);
            if (max <= 0) max = 20;

            if (dtpEnd.Value <= dtpStart.Value)
            {
                MessageBox.Show("End date must be after start date.", "Validation",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int    moduleID = Convert.ToInt32(cboModule.SelectedValue);
            string level    = cboLevel.SelectedItem.ToString();

            if (editingClassID == 0)
            {
                string ins =
                    "INSERT INTO Classes (TrainerID, ModuleID, ClassLevel, Schedule, Venue, Fee, StartDate, EndDate, MaxStudents) " +
                    "VALUES (@TrainerID, @ModuleID, @Level, @Schedule, @Venue, @Fee, @Start, @End, @Max)";
                SqlParameter[] p =
                {
                    new SqlParameter("@TrainerID", UserSession.TrainerID),
                    new SqlParameter("@ModuleID",  moduleID),
                    new SqlParameter("@Level",     level),
                    new SqlParameter("@Schedule",  txtSchedule.Text.Trim()),
                    new SqlParameter("@Venue",     txtVenue.Text.Trim()),
                    new SqlParameter("@Fee",       fee),
                    new SqlParameter("@Start",     dtpStart.Value.Date),
                    new SqlParameter("@End",       dtpEnd.Value.Date),
                    new SqlParameter("@Max",       max)
                };
                if (DatabaseHelper.ExecuteNonQuery(ins, p) > 0)
                {
                    MessageBox.Show("Class added successfully!", "Success",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    ClearAddForm();
                    LoadClasses();
                    tabClasses.SelectedTab = tabList;
                }
            }
            else
            {
                string upd =
                    "UPDATE Classes SET ClassLevel=@Level, Schedule=@Schedule, Venue=@Venue, " +
                    "Fee=@Fee, StartDate=@Start, EndDate=@End, MaxStudents=@Max " +
                    "WHERE ClassID=@ClassID AND TrainerID=@TrainerID";
                SqlParameter[] p =
                {
                    new SqlParameter("@Level",     level),
                    new SqlParameter("@Schedule",  txtSchedule.Text.Trim()),
                    new SqlParameter("@Venue",     txtVenue.Text.Trim()),
                    new SqlParameter("@Fee",       fee),
                    new SqlParameter("@Start",     dtpStart.Value.Date),
                    new SqlParameter("@End",       dtpEnd.Value.Date),
                    new SqlParameter("@Max",       max),
                    new SqlParameter("@ClassID",   editingClassID),
                    new SqlParameter("@TrainerID", UserSession.TrainerID)
                };
                if (DatabaseHelper.ExecuteNonQuery(upd, p) > 0)
                {
                    MessageBox.Show("Class updated successfully!", "Success",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadClasses();
                    tabClasses.SelectedTab = tabList;
                }
            }
        }

        private void ClearAddForm()
        {
            if (cboModule.Items.Count > 0) cboModule.SelectedIndex = 0;
            cboLevel.SelectedIndex = 0;
            txtSchedule.Clear(); txtVenue.Clear();
            txtFee.Clear(); txtMax.Text = "20";
            dtpStart.Value = DateTime.Now.Date;
            dtpEnd.Value   = DateTime.Now.Date.AddDays(28);
        }

        private void btnClearForm_Click(object sender, EventArgs e) { ClearAddForm(); }
        private void btnRefreshList_Click(object sender, EventArgs e) { LoadClasses(); }
        private void btnClose_Click(object sender, EventArgs e)     { this.Close(); }

        private void InitializeComponent()
        {
            this.lblTitle        = new Label();
            this.lblInfo         = new Label();
            this.tabClasses      = new TabControl();
            this.tabList         = new TabPage();
            this.tabAddEdit      = new TabPage();
            this.dgvClasses      = new DataGridView();
            this.btnEdit         = new Button();
            this.btnDeleteClass  = new Button();
            this.btnAddNew       = new Button();
            this.btnRefreshList  = new Button();
            this.lblCount        = new Label();
            this.lblFormTitle    = new Label();
            this.lblModule       = new Label();
            this.cboModule       = new ComboBox();
            this.lblLevel        = new Label();
            this.cboLevel        = new ComboBox();
            this.lblSchedule     = new Label();
            this.txtSchedule     = new TextBox();
            this.lblVenue        = new Label();
            this.txtVenue        = new TextBox();
            this.lblFee          = new Label();
            this.txtFee          = new TextBox();
            this.lblMax          = new Label();
            this.txtMax          = new TextBox();
            this.lblStart        = new Label();
            this.dtpStart        = new DateTimePicker();
            this.lblEnd          = new Label();
            this.dtpEnd          = new DateTimePicker();
            this.btnSave         = new Button();
            this.btnClearForm    = new Button();
            this.btnClose        = new Button();

            this.SuspendLayout();

            this.Text            = "Manage My Classes";
            this.Size            = new Size(820, 570);
            this.StartPosition   = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox     = false;

            // Title
            this.lblTitle.Text      = "Manage My Coaching Classes";
            this.lblTitle.Font      = new Font("Arial", 11, FontStyle.Bold);
            this.lblTitle.Location  = new Point(10, 10);
            this.lblTitle.Size      = new Size(780, 22);
            this.lblTitle.TextAlign = ContentAlignment.MiddleCenter;

            // Info
            this.lblInfo.Font     = new Font("Arial", 8);
            this.lblInfo.Location = new Point(10, 36);
            this.lblInfo.Size     = new Size(400, 18);
            this.lblInfo.Text     = "";

            // TabControl
            this.tabClasses.Location = new Point(10, 58);
            this.tabClasses.Size     = new Size(780, 465);

            // ── LIST TAB ──
            this.tabList.Text = "My Classes";

            this.lblCount.Font     = new Font("Arial", 8);
            this.lblCount.Location = new Point(5, 8);
            this.lblCount.Size     = new Size(200, 18);

            this.btnAddNew.Text     = "Add New Class";
            this.btnAddNew.Font     = new Font("Arial", 9);
            this.btnAddNew.Location = new Point(562, 5);
            this.btnAddNew.Size     = new Size(110, 24);
            this.btnAddNew.Click   += new EventHandler(this.btnAddNew_Click);

            this.btnRefreshList.Text     = "Refresh";
            this.btnRefreshList.Font     = new Font("Arial", 9);
            this.btnRefreshList.Location = new Point(678, 5);
            this.btnRefreshList.Size     = new Size(70, 24);
            this.btnRefreshList.Click   += new EventHandler(this.btnRefreshList_Click);

            this.dgvClasses.Location            = new Point(5, 32);
            this.dgvClasses.Size                = new Size(762, 360);
            this.dgvClasses.ReadOnly            = true;
            this.dgvClasses.AllowUserToAddRows  = false;
            this.dgvClasses.SelectionMode       = DataGridViewSelectionMode.FullRowSelect;
            this.dgvClasses.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvClasses.RowHeadersVisible   = false;
            this.dgvClasses.MultiSelect         = false;
            this.dgvClasses.SelectionChanged   += new EventHandler(this.dgvClasses_SelectionChanged);

            this.btnEdit.Text     = "Edit";
            this.btnEdit.Font     = new Font("Arial", 9);
            this.btnEdit.Location = new Point(5, 398);
            this.btnEdit.Size     = new Size(80, 27);
            this.btnEdit.Enabled  = false;
            this.btnEdit.Click   += new EventHandler(this.btnEdit_Click);

            this.btnDeleteClass.Text     = "Delete";
            this.btnDeleteClass.Font     = new Font("Arial", 9);
            this.btnDeleteClass.Location = new Point(95, 398);
            this.btnDeleteClass.Size     = new Size(80, 27);
            this.btnDeleteClass.Enabled  = false;
            this.btnDeleteClass.Click   += new EventHandler(this.btnDeleteClass_Click);

            this.tabList.Controls.Add(this.lblCount);
            this.tabList.Controls.Add(this.btnAddNew);
            this.tabList.Controls.Add(this.btnRefreshList);
            this.tabList.Controls.Add(this.dgvClasses);
            this.tabList.Controls.Add(this.btnEdit);
            this.tabList.Controls.Add(this.btnDeleteClass);

            // ── ADD/EDIT TAB ──
            this.tabAddEdit.Text = "Add / Edit Class";

            // Form title
            this.lblFormTitle.Text     = "Add New Class";
            this.lblFormTitle.Font     = new Font("Arial", 10, FontStyle.Bold);
            this.lblFormTitle.Location = new Point(10, 10);
            this.lblFormTitle.Size     = new Size(300, 22);

            int lx2 = 15, cx2 = 165, cw2 = 300;

            // Module
            this.lblModule.Text     = "Module:";
            this.lblModule.Font     = new Font("Arial", 9);
            this.lblModule.Location = new Point(lx2, 42);
            this.lblModule.Size     = new Size(140, 22);
            this.cboModule.Font          = new Font("Arial", 9);
            this.cboModule.DropDownStyle = ComboBoxStyle.DropDownList;
            this.cboModule.Location      = new Point(cx2, 40);
            this.cboModule.Size          = new Size(cw2, 22);

            // Level
            this.lblLevel.Text     = "Class Level:";
            this.lblLevel.Font     = new Font("Arial", 9);
            this.lblLevel.Location = new Point(lx2, 77);
            this.lblLevel.Size     = new Size(140, 22);
            this.cboLevel.Font          = new Font("Arial", 9);
            this.cboLevel.DropDownStyle = ComboBoxStyle.DropDownList;
            this.cboLevel.Location      = new Point(cx2, 75);
            this.cboLevel.Size          = new Size(180, 22);
            this.cboLevel.Items.AddRange(new object[] { "Beginner", "Intermediate", "Advance" });
            this.cboLevel.SelectedIndex = 0;

            // Schedule
            this.lblSchedule.Text     = "Schedule:";
            this.lblSchedule.Font     = new Font("Arial", 9);
            this.lblSchedule.Location = new Point(lx2, 112);
            this.lblSchedule.Size     = new Size(140, 22);
            this.txtSchedule.Font     = new Font("Arial", 9);
            this.txtSchedule.Location = new Point(cx2, 110);
            this.txtSchedule.Size     = new Size(cw2, 22);

            // Venue
            this.lblVenue.Text     = "Venue:";
            this.lblVenue.Font     = new Font("Arial", 9);
            this.lblVenue.Location = new Point(lx2, 147);
            this.lblVenue.Size     = new Size(140, 22);
            this.txtVenue.Font     = new Font("Arial", 9);
            this.txtVenue.Location = new Point(cx2, 145);
            this.txtVenue.Size     = new Size(cw2, 22);

            // Fee
            this.lblFee.Text     = "Fee (RM):";
            this.lblFee.Font     = new Font("Arial", 9);
            this.lblFee.Location = new Point(lx2, 182);
            this.lblFee.Size     = new Size(140, 22);
            this.txtFee.Font     = new Font("Arial", 9);
            this.txtFee.Location = new Point(cx2, 180);
            this.txtFee.Size     = new Size(120, 22);

            // Max Students
            this.lblMax.Text     = "Max Students:";
            this.lblMax.Font     = new Font("Arial", 9);
            this.lblMax.Location = new Point(lx2, 217);
            this.lblMax.Size     = new Size(140, 22);
            this.txtMax.Font     = new Font("Arial", 9);
            this.txtMax.Location = new Point(cx2, 215);
            this.txtMax.Size     = new Size(80, 22);
            this.txtMax.Text     = "20";

            // Start Date
            this.lblStart.Text     = "Start Date:";
            this.lblStart.Font     = new Font("Arial", 9);
            this.lblStart.Location = new Point(lx2, 252);
            this.lblStart.Size     = new Size(140, 22);
            this.dtpStart.Format   = DateTimePickerFormat.Short;
            this.dtpStart.Location = new Point(cx2, 250);
            this.dtpStart.Size     = new Size(150, 22);

            // End Date
            this.lblEnd.Text     = "End Date:";
            this.lblEnd.Font     = new Font("Arial", 9);
            this.lblEnd.Location = new Point(lx2, 287);
            this.lblEnd.Size     = new Size(140, 22);
            this.dtpEnd.Format   = DateTimePickerFormat.Short;
            this.dtpEnd.Location = new Point(cx2, 285);
            this.dtpEnd.Size     = new Size(150, 22);
            this.dtpEnd.Value    = DateTime.Now.Date.AddDays(28);

            // Save button
            this.btnSave.Text     = "Save";
            this.btnSave.Font     = new Font("Arial", 9);
            this.btnSave.Location = new Point(lx2, 330);
            this.btnSave.Size     = new Size(90, 28);
            this.btnSave.Click   += new EventHandler(this.btnSave_Click);

            // Clear button
            this.btnClearForm.Text     = "Clear";
            this.btnClearForm.Font     = new Font("Arial", 9);
            this.btnClearForm.Location = new Point(115, 330);
            this.btnClearForm.Size     = new Size(80, 28);
            this.btnClearForm.Click   += new EventHandler(this.btnClearForm_Click);

            this.tabAddEdit.Controls.Add(this.lblFormTitle);
            this.tabAddEdit.Controls.Add(this.lblModule);    this.tabAddEdit.Controls.Add(this.cboModule);
            this.tabAddEdit.Controls.Add(this.lblLevel);     this.tabAddEdit.Controls.Add(this.cboLevel);
            this.tabAddEdit.Controls.Add(this.lblSchedule);  this.tabAddEdit.Controls.Add(this.txtSchedule);
            this.tabAddEdit.Controls.Add(this.lblVenue);     this.tabAddEdit.Controls.Add(this.txtVenue);
            this.tabAddEdit.Controls.Add(this.lblFee);       this.tabAddEdit.Controls.Add(this.txtFee);
            this.tabAddEdit.Controls.Add(this.lblMax);       this.tabAddEdit.Controls.Add(this.txtMax);
            this.tabAddEdit.Controls.Add(this.lblStart);     this.tabAddEdit.Controls.Add(this.dtpStart);
            this.tabAddEdit.Controls.Add(this.lblEnd);       this.tabAddEdit.Controls.Add(this.dtpEnd);
            this.tabAddEdit.Controls.Add(this.btnSave);
            this.tabAddEdit.Controls.Add(this.btnClearForm);

            this.tabClasses.TabPages.Add(this.tabList);
            this.tabClasses.TabPages.Add(this.tabAddEdit);

            // Close button
            this.btnClose.Text     = "Close";
            this.btnClose.Font     = new Font("Arial", 9);
            this.btnClose.Location = new Point(720, 530);
            this.btnClose.Size     = new Size(80, 27);
            this.btnClose.Click   += new EventHandler(this.btnClose_Click);

            this.Controls.Add(this.lblTitle);
            this.Controls.Add(this.lblInfo);
            this.Controls.Add(this.tabClasses);
            this.Controls.Add(this.btnClose);

            this.Load += new EventHandler(this.frmManageClasses_Load);
            this.ResumeLayout(false);
        }
    }

    // ============================================================
    // Trainer Feature C: View enrolled students who have paid
    // ============================================================
    public class frmViewEnrolledStudents : Form
    {
        private Label        lblTitle;
        private Label        lblInfo;
        private Label        lblFilter;
        private ComboBox     cboFilterClass;
        private Label        lblCount;
        private DataGridView dgvStudents;
        private Button       btnRefresh;
        private Button       btnClose;

        public frmViewEnrolledStudents()
        {
            InitializeComponent();
        }

        private void frmViewEnrolledStudents_Load(object sender, EventArgs e)
        {
            lblInfo.Text = "Trainer: " + UserSession.Name;
            LoadClassFilter();
            LoadStudents();
        }

        private void LoadClassFilter()
        {
            cboFilterClass.Items.Add("All My Classes");
            string query =
                "SELECT ClassID, m.ModuleCode + ' - ' + c.ClassLevel AS Display " +
                "FROM Classes c INNER JOIN Modules m ON c.ModuleID = m.ModuleID " +
                "WHERE c.TrainerID = @TrainerID AND c.IsActive = 1";
            SqlParameter[] p = { new SqlParameter("@TrainerID", UserSession.TrainerID) };
            DataTable dt = DatabaseHelper.ExecuteQuery(query, p);
            foreach (DataRow row in dt.Rows)
                cboFilterClass.Items.Add(row["Display"].ToString() + "|" + row["ClassID"].ToString());
            cboFilterClass.SelectedIndex = 0;
        }

        private void LoadStudents()
        {
            string classFilter = "";
            if (cboFilterClass.SelectedIndex > 0)
            {
                string sel      = cboFilterClass.SelectedItem.ToString();
                string classIDStr = sel.Substring(sel.LastIndexOf('|') + 1);
                classFilter = " AND e.ClassID = " + classIDStr;
            }

            string query =
                "SELECT u.Name AS StudentName, s.TPNumber, s.StudyLevel, " +
                "       m.ModuleCode, c.ClassLevel, c.Schedule, " +
                "       e.PaymentStatus, " +
                "       ISNULL(CONVERT(NVARCHAR, p.PaymentDate, 103), '-') AS PaidOn " +
                "FROM Enrolments e " +
                "INNER JOIN Students s  ON e.StudentID   = s.StudentID " +
                "INNER JOIN Users    u  ON s.UserID      = u.UserID " +
                "INNER JOIN Classes  c  ON e.ClassID     = c.ClassID " +
                "INNER JOIN Modules  m  ON c.ModuleID    = m.ModuleID " +
                "LEFT  JOIN Payments p  ON e.EnrolmentID = p.EnrolmentID " +
                "WHERE c.TrainerID = @TrainerID AND e.PaymentStatus = 'Paid'" + classFilter +
                " ORDER BY u.Name";

            SqlParameter[] p2 = { new SqlParameter("@TrainerID", UserSession.TrainerID) };
            DataTable dt      = DatabaseHelper.ExecuteQuery(query, p2);

            dgvStudents.DataSource = null;
            dgvStudents.DataSource = dt;

            if (dgvStudents.Columns.Count > 0)
            {
                dgvStudents.Columns["StudentName"].HeaderText = "Student";
                dgvStudents.Columns["TPNumber"].HeaderText    = "TP";
                dgvStudents.Columns["StudyLevel"].HeaderText  = "Level";
                dgvStudents.Columns["ModuleCode"].HeaderText  = "Module";
                dgvStudents.Columns["ClassLevel"].HeaderText  = "Class";
                dgvStudents.Columns["Schedule"].HeaderText    = "Schedule";
                dgvStudents.Columns["PaymentStatus"].HeaderText = "Payment";
                dgvStudents.Columns["PaidOn"].HeaderText      = "Paid On";
            }

            lblCount.Text = "Paid students: " + dt.Rows.Count;
        }

        private void cboFilterClass_SelectedIndexChanged(object sender, EventArgs e) { LoadStudents(); }
        private void btnRefresh_Click(object sender, EventArgs e) { LoadStudents(); }
        private void btnClose_Click(object sender, EventArgs e)   { this.Close(); }

        private void InitializeComponent()
        {
            this.lblTitle       = new Label();
            this.lblInfo        = new Label();
            this.lblFilter      = new Label();
            this.cboFilterClass = new ComboBox();
            this.lblCount       = new Label();
            this.dgvStudents    = new DataGridView();
            this.btnRefresh     = new Button();
            this.btnClose       = new Button();

            this.SuspendLayout();

            this.Text            = "Enrolled & Paid Students";
            this.Size            = new Size(860, 520);
            this.StartPosition   = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox     = false;

            this.lblTitle.Text      = "Enrolled & Paid Students";
            this.lblTitle.Font      = new Font("Arial", 11, FontStyle.Bold);
            this.lblTitle.Location  = new Point(10, 10);
            this.lblTitle.Size      = new Size(820, 22);
            this.lblTitle.TextAlign = ContentAlignment.MiddleCenter;

            this.lblInfo.Text     = "";
            this.lblInfo.Font     = new Font("Arial", 8);
            this.lblInfo.Location = new Point(10, 40);
            this.lblInfo.Size     = new Size(300, 18);

            this.lblFilter.Text     = "Filter by Class:";
            this.lblFilter.Font     = new Font("Arial", 9);
            this.lblFilter.Location = new Point(420, 40);
            this.lblFilter.Size     = new Size(100, 18);

            this.cboFilterClass.Font          = new Font("Arial", 9);
            this.cboFilterClass.DropDownStyle = ComboBoxStyle.DropDownList;
            this.cboFilterClass.Location      = new Point(525, 38);
            this.cboFilterClass.Size          = new Size(200, 22);
            this.cboFilterClass.SelectedIndexChanged += new EventHandler(this.cboFilterClass_SelectedIndexChanged);

            this.lblCount.Font      = new Font("Arial", 8);
            this.lblCount.Location  = new Point(730, 40);
            this.lblCount.Size      = new Size(110, 18);
            this.lblCount.TextAlign = ContentAlignment.MiddleRight;

            this.dgvStudents.Location            = new Point(10, 65);
            this.dgvStudents.Size                = new Size(820, 395);
            this.dgvStudents.ReadOnly            = true;
            this.dgvStudents.AllowUserToAddRows  = false;
            this.dgvStudents.SelectionMode       = DataGridViewSelectionMode.FullRowSelect;
            this.dgvStudents.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvStudents.RowHeadersVisible   = false;

            this.btnRefresh.Text     = "Refresh";
            this.btnRefresh.Font     = new Font("Arial", 9);
            this.btnRefresh.Location = new Point(700, 468);
            this.btnRefresh.Size     = new Size(70, 27);
            this.btnRefresh.Click   += new EventHandler(this.btnRefresh_Click);

            this.btnClose.Text     = "Close";
            this.btnClose.Font     = new Font("Arial", 9);
            this.btnClose.Location = new Point(780, 468);
            this.btnClose.Size     = new Size(70, 27);
            this.btnClose.Click   += new EventHandler(this.btnClose_Click);

            this.Controls.Add(this.lblTitle);
            this.Controls.Add(this.lblInfo);
            this.Controls.Add(this.lblFilter);
            this.Controls.Add(this.cboFilterClass);
            this.Controls.Add(this.lblCount);
            this.Controls.Add(this.dgvStudents);
            this.Controls.Add(this.btnRefresh);
            this.Controls.Add(this.btnClose);

            this.Load += new EventHandler(this.frmViewEnrolledStudents_Load);
            this.ResumeLayout(false);
        }
    }

    // ============================================================
    // Trainer Feature D: Send Feedback to Administrator
    // ============================================================
    public class frmSendFeedback : Form
    {
        private Label    lblTitle;
        private Label    lblInfo;
        private Label    lblType;
        private ComboBox cboType;
        private Label    lblSubject;
        private TextBox  txtSubject;
        private Label    lblMessage;
        private TextBox  txtMessage;
        private Button   btnSend;
        private Button   btnClear;
        private Button   btnClose;

        public frmSendFeedback()
        {
            InitializeComponent();
        }

        private void frmSendFeedback_Load(object sender, EventArgs e)
        {
            lblInfo.Text = "Trainer: " + UserSession.Name + "  |  Staff: " + UserSession.TrainerStaffID;
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            if (cboType.SelectedIndex < 0)
            { MessageBox.Show("Please select a feedback type.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }
            if (txtSubject.Text.Trim() == "")
            { MessageBox.Show("Please enter a subject.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }
            if (txtMessage.Text.Trim().Length < 10)
            { MessageBox.Show("Please enter a message (at least 10 characters).", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }

            string ins =
                "INSERT INTO Feedbacks (TrainerID, FeedbackType, Subject, Message) " +
                "VALUES (@TrainerID, @Type, @Subject, @Message)";
            SqlParameter[] p =
            {
                new SqlParameter("@TrainerID", UserSession.TrainerID),
                new SqlParameter("@Type",      cboType.SelectedItem.ToString()),
                new SqlParameter("@Subject",   txtSubject.Text.Trim()),
                new SqlParameter("@Message",   txtMessage.Text.Trim())
            };
            if (DatabaseHelper.ExecuteNonQuery(ins, p) > 0)
            {
                MessageBox.Show("Feedback sent to Administrator!", "Success",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                ClearForm();
            }
        }

        private void ClearForm()
        {
            cboType.SelectedIndex = -1;
            txtSubject.Clear();
            txtMessage.Clear();
        }

        private void btnClear_Click(object sender, EventArgs e) { ClearForm(); }
        private void btnClose_Click(object sender, EventArgs e) { this.Close(); }

        private void InitializeComponent()
        {
            this.lblTitle   = new Label();
            this.lblInfo    = new Label();
            this.lblType    = new Label();
            this.cboType    = new ComboBox();
            this.lblSubject = new Label();
            this.txtSubject = new TextBox();
            this.lblMessage = new Label();
            this.txtMessage = new TextBox();
            this.btnSend    = new Button();
            this.btnClear   = new Button();
            this.btnClose   = new Button();

            this.SuspendLayout();

            this.Text            = "Send Feedback";
            this.Size            = new Size(520, 430);
            this.StartPosition   = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox     = false;

            this.lblTitle.Text      = "Send Feedback to Administrator";
            this.lblTitle.Font      = new Font("Arial", 11, FontStyle.Bold);
            this.lblTitle.Location  = new Point(10, 10);
            this.lblTitle.Size      = new Size(485, 22);
            this.lblTitle.TextAlign = ContentAlignment.MiddleCenter;

            this.lblInfo.Text     = "";
            this.lblInfo.Font     = new Font("Arial", 8);
            this.lblInfo.Location = new Point(10, 38);
            this.lblInfo.Size     = new Size(485, 18);

            int lx = 15, cx = 155, cw = 330;

            // Type
            this.lblType.Text     = "Feedback Type:";
            this.lblType.Font     = new Font("Arial", 9);
            this.lblType.Location = new Point(lx, 68);
            this.lblType.Size     = new Size(130, 22);
            this.cboType.Font          = new Font("Arial", 9);
            this.cboType.DropDownStyle = ComboBoxStyle.DropDownList;
            this.cboType.Location      = new Point(cx, 66);
            this.cboType.Size          = new Size(160, 22);
            this.cboType.Items.AddRange(new object[] { "Suggestion", "Complaint", "Other" });

            // Subject
            this.lblSubject.Text     = "Subject:";
            this.lblSubject.Font     = new Font("Arial", 9);
            this.lblSubject.Location = new Point(lx, 104);
            this.lblSubject.Size     = new Size(130, 22);
            this.txtSubject.Font     = new Font("Arial", 9);
            this.txtSubject.Location = new Point(cx, 102);
            this.txtSubject.Size     = new Size(cw, 22);

            // Message
            this.lblMessage.Text     = "Message:";
            this.lblMessage.Font     = new Font("Arial", 9);
            this.lblMessage.Location = new Point(lx, 139);
            this.lblMessage.Size     = new Size(130, 22);
            this.txtMessage.Font       = new Font("Arial", 9);
            this.txtMessage.Location   = new Point(cx, 137);
            this.txtMessage.Size       = new Size(cw, 200);
            this.txtMessage.Multiline  = true;
            this.txtMessage.ScrollBars = ScrollBars.Vertical;

            // Buttons
            this.btnSend.Text     = "Send Feedback";
            this.btnSend.Font     = new Font("Arial", 9);
            this.btnSend.Location = new Point(lx, 355);
            this.btnSend.Size     = new Size(110, 28);
            this.btnSend.Click   += new EventHandler(this.btnSend_Click);

            this.btnClear.Text     = "Clear";
            this.btnClear.Font     = new Font("Arial", 9);
            this.btnClear.Location = new Point(140, 355);
            this.btnClear.Size     = new Size(80, 28);
            this.btnClear.Click   += new EventHandler(this.btnClear_Click);

            this.btnClose.Text     = "Close";
            this.btnClose.Font     = new Font("Arial", 9);
            this.btnClose.Location = new Point(405, 355);
            this.btnClose.Size     = new Size(80, 28);
            this.btnClose.Click   += new EventHandler(this.btnClose_Click);

            this.Controls.Add(this.lblTitle);
            this.Controls.Add(this.lblInfo);
            this.Controls.Add(this.lblType);    this.Controls.Add(this.cboType);
            this.Controls.Add(this.lblSubject); this.Controls.Add(this.txtSubject);
            this.Controls.Add(this.lblMessage); this.Controls.Add(this.txtMessage);
            this.Controls.Add(this.btnSend);
            this.Controls.Add(this.btnClear);
            this.Controls.Add(this.btnClose);

            this.Load += new EventHandler(this.frmSendFeedback_Load);
            this.ResumeLayout(false);
        }
    }
}
