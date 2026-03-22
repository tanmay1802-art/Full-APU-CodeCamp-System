using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace APUCodeCamp
{
    // Admin Feature B: Assign trainer to a module+level
    public class frmAssignTrainer : Form
    {
        private Label        lblTitle;
        private Label        lblTrainer;
        private Label        lblModule;
        private Label        lblLevel;
        private ComboBox     cboTrainer;
        private ComboBox     cboModule;
        private ComboBox     cboLevel;
        private Button       btnAssign;
        private Button       btnRemoveAssign;
        private Button       btnClear;
        private Label        lblCurrentTitle;
        private DataGridView dgvCurrent;
        private Label        lblCount;
        private Button       btnClose;

        public frmAssignTrainer()
        {
            InitializeComponent();
        }

        private void frmAssignTrainer_Load(object sender, EventArgs e)
        {
            LoadTrainerCombo();
            LoadModuleCombo();
            LoadCurrentAssignments();
        }

        private void LoadTrainerCombo()
        {
            DataTable dt = DatabaseHelper.ExecuteQuery(
                "SELECT t.TrainerID, u.Name + ' (' + t.StaffID + ')' AS Display " +
                "FROM Trainers t INNER JOIN Users u ON t.UserID = u.UserID " +
                "WHERE u.IsActive = 1 ORDER BY u.Name");
            cboTrainer.DataSource    = dt;
            cboTrainer.DisplayMember = "Display";
            cboTrainer.ValueMember   = "TrainerID";
            cboTrainer.SelectedIndex = -1;
        }

        private void LoadModuleCombo()
        {
            DataTable dt = DatabaseHelper.ExecuteQuery(
                "SELECT ModuleID, ModuleCode + ' - ' + ModuleName AS Display " +
                "FROM Modules WHERE IsActive = 1 ORDER BY ModuleCode");
            cboModule.DataSource    = dt;
            cboModule.DisplayMember = "Display";
            cboModule.ValueMember   = "ModuleID";
            cboModule.SelectedIndex = -1;
        }

        private void LoadCurrentAssignments()
        {
            string query =
                "SELECT tm.TrainerModuleID, u.Name AS Trainer, m.ModuleCode, m.ModuleName, " +
                "       tm.ClassLevel, tm.AssignedDate, tm.IsActive " +
                "FROM TrainerModules tm " +
                "INNER JOIN Trainers t ON tm.TrainerID = t.TrainerID " +
                "INNER JOIN Users    u ON t.UserID     = u.UserID " +
                "INNER JOIN Modules  m ON tm.ModuleID  = m.ModuleID " +
                "ORDER BY u.Name, m.ModuleCode";
            DataTable dt = DatabaseHelper.ExecuteQuery(query);
            dgvCurrent.DataSource = null;
            dgvCurrent.DataSource = dt;

            if (dgvCurrent.Columns.Count > 0)
            {
                dgvCurrent.Columns["TrainerModuleID"].Visible   = false;
                dgvCurrent.Columns["Trainer"].HeaderText        = "Trainer";
                dgvCurrent.Columns["ModuleCode"].HeaderText     = "Code";
                dgvCurrent.Columns["ModuleName"].HeaderText     = "Module Name";
                dgvCurrent.Columns["ClassLevel"].HeaderText     = "Level";
                dgvCurrent.Columns["AssignedDate"].HeaderText   = "Assigned On";
                dgvCurrent.Columns["IsActive"].HeaderText       = "Active";
                dgvCurrent.Columns["AssignedDate"].DefaultCellStyle.Format = "dd/MM/yyyy";
            }
            lblCount.Text = "Current assignments: " + dt.Rows.Count;
        }

        private void btnAssign_Click(object sender, EventArgs e)
        {
            if (cboTrainer.SelectedIndex < 0 || cboModule.SelectedIndex < 0 || cboLevel.SelectedIndex < 0)
            {
                MessageBox.Show("Please select Trainer, Module and Level.", "Validation",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int    trainerID = Convert.ToInt32(cboTrainer.SelectedValue);
            int    moduleID  = Convert.ToInt32(cboModule.SelectedValue);
            string level     = cboLevel.SelectedItem.ToString();

            // One trainer can only teach one module at a time
            string checkActive =
                "SELECT COUNT(*) FROM TrainerModules WHERE TrainerID = @TrainerID AND IsActive = 1";
            SqlParameter[] pCheck = { new SqlParameter("@TrainerID", trainerID) };
            int activeCount = Convert.ToInt32(DatabaseHelper.ExecuteScalar(checkActive, pCheck));
            if (activeCount > 0)
            {
                MessageBox.Show("This trainer already has an active module assignment.\n" +
                    "Remove the existing assignment first.", "Conflict",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Check duplicate
            string checkDup =
                "SELECT COUNT(*) FROM TrainerModules WHERE TrainerID=@T AND ModuleID=@M AND ClassLevel=@L";
            SqlParameter[] pDup =
            {
                new SqlParameter("@T", trainerID),
                new SqlParameter("@M", moduleID),
                new SqlParameter("@L", level)
            };
            if (Convert.ToInt32(DatabaseHelper.ExecuteScalar(checkDup, pDup)) > 0)
            {
                MessageBox.Show("This exact assignment already exists.", "Duplicate",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string ins =
                "INSERT INTO TrainerModules (TrainerID, ModuleID, ClassLevel) " +
                "VALUES (@TrainerID, @ModuleID, @Level)";
            SqlParameter[] pIns =
            {
                new SqlParameter("@TrainerID", trainerID),
                new SqlParameter("@ModuleID",  moduleID),
                new SqlParameter("@Level",     level)
            };
            if (DatabaseHelper.ExecuteNonQuery(ins, pIns) > 0)
            {
                MessageBox.Show("Trainer assigned successfully!", "Success",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                ClearForm();
                LoadCurrentAssignments();
            }
        }

        private void btnRemoveAssign_Click(object sender, EventArgs e)
        {
            if (dgvCurrent.SelectedRows.Count == 0) return;
            int id = Convert.ToInt32(dgvCurrent.SelectedRows[0].Cells["TrainerModuleID"].Value);

            if (MessageBox.Show("Remove this assignment?", "Confirm",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No) return;

            string upd = "UPDATE TrainerModules SET IsActive = 0 WHERE TrainerModuleID = @ID";
            SqlParameter[] p = { new SqlParameter("@ID", id) };
            if (DatabaseHelper.ExecuteNonQuery(upd, p) > 0)
            {
                MessageBox.Show("Assignment removed.", "Done",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadCurrentAssignments();
            }
        }

        private void ClearForm()
        {
            cboTrainer.SelectedIndex = -1;
            cboModule.SelectedIndex  = -1;
            cboLevel.SelectedIndex   = -1;
        }

        private void btnClear_Click(object sender, EventArgs e) { ClearForm(); }
        private void btnClose_Click(object sender, EventArgs e) { this.Close(); }

        private void InitializeComponent()
        {
            this.lblTitle        = new Label();
            this.lblTrainer      = new Label();
            this.cboTrainer      = new ComboBox();
            this.lblModule       = new Label();
            this.cboModule       = new ComboBox();
            this.lblLevel        = new Label();
            this.cboLevel        = new ComboBox();
            this.btnAssign       = new Button();
            this.btnRemoveAssign = new Button();
            this.btnClear        = new Button();
            this.lblCurrentTitle = new Label();
            this.dgvCurrent      = new DataGridView();
            this.lblCount        = new Label();
            this.btnClose        = new Button();

            this.SuspendLayout();

            this.Text            = "Assign Trainer to Module";
            this.Size            = new Size(760, 580);
            this.StartPosition   = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox     = false;

            // Title
            this.lblTitle.Text      = "Assign Trainer to Module & Level";
            this.lblTitle.Font      = new Font("Arial", 11, FontStyle.Bold);
            this.lblTitle.Location  = new Point(10, 10);
            this.lblTitle.Size      = new Size(720, 22);
            this.lblTitle.TextAlign = ContentAlignment.MiddleCenter;

            // Trainer label + combo
            this.lblTrainer.Text     = "Select Trainer:";
            this.lblTrainer.Font     = new Font("Arial", 9);
            this.lblTrainer.Location = new Point(20, 50);
            this.lblTrainer.Size     = new Size(130, 22);

            this.cboTrainer.Font          = new Font("Arial", 9);
            this.cboTrainer.DropDownStyle = ComboBoxStyle.DropDownList;
            this.cboTrainer.Location      = new Point(160, 48);
            this.cboTrainer.Size          = new Size(280, 22);

            // Module label + combo
            this.lblModule.Text     = "Select Module:";
            this.lblModule.Font     = new Font("Arial", 9);
            this.lblModule.Location = new Point(20, 85);
            this.lblModule.Size     = new Size(130, 22);

            this.cboModule.Font          = new Font("Arial", 9);
            this.cboModule.DropDownStyle = ComboBoxStyle.DropDownList;
            this.cboModule.Location      = new Point(160, 83);
            this.cboModule.Size          = new Size(280, 22);

            // Level label + combo
            this.lblLevel.Text     = "Class Level:";
            this.lblLevel.Font     = new Font("Arial", 9);
            this.lblLevel.Location = new Point(20, 120);
            this.lblLevel.Size     = new Size(130, 22);

            this.cboLevel.Font          = new Font("Arial", 9);
            this.cboLevel.DropDownStyle = ComboBoxStyle.DropDownList;
            this.cboLevel.Location      = new Point(160, 118);
            this.cboLevel.Size          = new Size(180, 22);
            this.cboLevel.Items.AddRange(new object[] { "Beginner", "Intermediate", "Advance" });

            // Rule note
            Label lblRule = new Label();
            lblRule.Text      = "* One trainer can only hold one active assignment at a time.";
            lblRule.Font      = new Font("Arial", 8, FontStyle.Italic);
            lblRule.ForeColor = System.Drawing.Color.DarkRed;
            lblRule.Location  = new Point(20, 148);
            lblRule.Size      = new Size(450, 18);

            // Assign button
            this.btnAssign.Text     = "Assign";
            this.btnAssign.Font     = new Font("Arial", 9);
            this.btnAssign.Location = new Point(20, 175);
            this.btnAssign.Size     = new Size(90, 28);
            this.btnAssign.Click   += new EventHandler(this.btnAssign_Click);

            // Clear button
            this.btnClear.Text     = "Clear";
            this.btnClear.Font     = new Font("Arial", 9);
            this.btnClear.Location = new Point(120, 175);
            this.btnClear.Size     = new Size(75, 28);
            this.btnClear.Click   += new EventHandler(this.btnClear_Click);

            // Current assignments section
            this.lblCurrentTitle.Text     = "Current Assignments";
            this.lblCurrentTitle.Font     = new Font("Arial", 9, FontStyle.Bold);
            this.lblCurrentTitle.Location = new Point(20, 222);
            this.lblCurrentTitle.Size     = new Size(200, 18);

            this.lblCount.Font      = new Font("Arial", 8);
            this.lblCount.Location  = new Point(500, 222);
            this.lblCount.Size      = new Size(230, 18);
            this.lblCount.TextAlign = ContentAlignment.MiddleRight;

            // DataGridView
            this.dgvCurrent.Location            = new Point(10, 244);
            this.dgvCurrent.Size                = new Size(720, 255);
            this.dgvCurrent.ReadOnly            = true;
            this.dgvCurrent.AllowUserToAddRows  = false;
            this.dgvCurrent.SelectionMode       = DataGridViewSelectionMode.FullRowSelect;
            this.dgvCurrent.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvCurrent.RowHeadersVisible   = false;
            this.dgvCurrent.MultiSelect         = false;

            // Remove Assignment button
            this.btnRemoveAssign.Text     = "Remove Assignment";
            this.btnRemoveAssign.Font     = new Font("Arial", 9);
            this.btnRemoveAssign.Location = new Point(20, 506);
            this.btnRemoveAssign.Size     = new Size(145, 27);
            this.btnRemoveAssign.Click   += new EventHandler(this.btnRemoveAssign_Click);

            // Close button
            this.btnClose.Text     = "Close";
            this.btnClose.Font     = new Font("Arial", 9);
            this.btnClose.Location = new Point(660, 506);
            this.btnClose.Size     = new Size(70, 27);
            this.btnClose.Click   += new EventHandler(this.btnClose_Click);

            this.Controls.Add(this.lblTitle);
            this.Controls.Add(this.lblTrainer);
            this.Controls.Add(this.cboTrainer);
            this.Controls.Add(this.lblModule);
            this.Controls.Add(this.cboModule);
            this.Controls.Add(this.lblLevel);
            this.Controls.Add(this.cboLevel);
            this.Controls.Add(lblRule);
            this.Controls.Add(this.btnAssign);
            this.Controls.Add(this.btnClear);
            this.Controls.Add(this.lblCurrentTitle);
            this.Controls.Add(this.lblCount);
            this.Controls.Add(this.dgvCurrent);
            this.Controls.Add(this.btnRemoveAssign);
            this.Controls.Add(this.btnClose);

            this.Load += new EventHandler(this.frmAssignTrainer_Load);
            this.ResumeLayout(false);
        }
    }
}
