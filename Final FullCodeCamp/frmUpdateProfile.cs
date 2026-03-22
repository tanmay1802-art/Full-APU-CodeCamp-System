using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace APUCodeCamp
{
    // Feature E (all roles): Update personal profile and change password
    public class frmUpdateProfile : Form
    {
        private Label      lblTitle;
        private TabControl tabProfile;
        private TabPage    tabPersonal;
        private TabPage    tabPassword;

        // Personal tab controls
        private Label    lblTPNumber;
        private TextBox  txtTPNumber;
        private Label    lblStaffID;
        private TextBox  txtStaffID;
        private Label    lblUsername;
        private TextBox  txtUsername;
        private Label    lblName;
        private TextBox  txtName;
        private Label    lblEmail;
        private TextBox  txtEmail;
        private Label    lblPhone;
        private TextBox  txtPhone;
        private Label    lblAddress;
        private TextBox  txtAddress;
        private Label    lblStudyLevel;
        private ComboBox cboStudyLevel;
        private Button   btnSave;
        private Button   btnReset;

        // Password tab controls
        private Label   lblCurrentPw;
        private TextBox txtCurrentPw;
        private Label   lblNewPw;
        private TextBox txtNewPw;
        private Label   lblConfirmPw;
        private TextBox txtConfirmPw;
        private Button  btnChangePassword;
        private Label lblHint;
        private Button btnClose;

        public frmUpdateProfile()
        {
            InitializeComponent();
        }

        private void frmUpdateProfile_Load(object sender, EventArgs e)
        {
            LoadProfile();

            // Show/hide fields based on role
            bool isStudent = (UserSession.Role == "Student");
            lblTPNumber.Visible   = isStudent;
            txtTPNumber.Visible   = isStudent;
            lblStudyLevel.Visible = isStudent;
            cboStudyLevel.Visible = isStudent;
            lblStaffID.Visible    = !isStudent;
            txtStaffID.Visible    = !isStudent;
        }

        private void LoadProfile()
        {
            string query =
                "SELECT u.Name, u.Email, u.Phone, u.Address, u.Username " +
                "FROM Users u WHERE u.UserID = @UserID";
            SqlParameter[] p = { new SqlParameter("@UserID", UserSession.UserID) };
            DataTable dt     = DatabaseHelper.ExecuteQuery(query, p);

            if (dt.Rows.Count > 0)
            {
                DataRow row      = dt.Rows[0];
                txtName.Text     = row["Name"].ToString();
                txtEmail.Text    = row["Email"].ToString();
                txtPhone.Text    = row["Phone"].ToString();
                txtAddress.Text  = row["Address"].ToString();
                txtUsername.Text = row["Username"].ToString();
            }

            // Role-specific read-only fields
            if (UserSession.Role == "Student")
            {
                txtTPNumber.Text = UserSession.TPNumber;
                cboStudyLevel.SelectedItem = UserSession.StudyLevel;
                if (cboStudyLevel.SelectedIndex < 0)
                    cboStudyLevel.SelectedIndex = 0;
            }
            else if (UserSession.Role == "Lecturer")
                txtStaffID.Text = UserSession.LecturerStaffID;
            else if (UserSession.Role == "Trainer")
                txtStaffID.Text = UserSession.TrainerStaffID;
            else if (UserSession.Role == "Administrator")
                txtStaffID.Text = UserSession.AdminStaffID;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (txtName.Text.Trim() == "")
            {
                MessageBox.Show("Name cannot be empty.", "Validation",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (!txtEmail.Text.Contains("@") || !txtEmail.Text.Contains("."))
            {
                MessageBox.Show("Please enter a valid email address.", "Validation",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (txtPhone.Text.Trim() == "")
            {
                MessageBox.Show("Phone cannot be empty.", "Validation",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (MessageBox.Show("Save changes to your profile?", "Confirm",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                return;

            // Update Users table
            string updateUser =
                "UPDATE Users SET Name=@Name, Email=@Email, Phone=@Phone, Address=@Address " +
                "WHERE UserID=@UserID";
            SqlParameter[] pUser =
            {
                new SqlParameter("@Name",    txtName.Text.Trim()),
                new SqlParameter("@Email",   txtEmail.Text.Trim()),
                new SqlParameter("@Phone",   txtPhone.Text.Trim()),
                new SqlParameter("@Address", txtAddress.Text.Trim()),
                new SqlParameter("@UserID",  UserSession.UserID)
            };
            DatabaseHelper.ExecuteNonQuery(updateUser, pUser);

            // Student only: update StudyLevel
            if (UserSession.Role == "Student" && cboStudyLevel.SelectedIndex >= 0)
            {
                string updateStudent =
                    "UPDATE Students SET StudyLevel=@StudyLevel WHERE UserID=@UserID";
                SqlParameter[] pStu =
                {
                    new SqlParameter("@StudyLevel", cboStudyLevel.SelectedItem.ToString()),
                    new SqlParameter("@UserID",     UserSession.UserID)
                };
                DatabaseHelper.ExecuteNonQuery(updateStudent, pStu);
                UserSession.StudyLevel = cboStudyLevel.SelectedItem.ToString();
            }

            // Refresh session
            UserSession.Name    = txtName.Text.Trim();
            UserSession.Email   = txtEmail.Text.Trim();
            UserSession.Phone   = txtPhone.Text.Trim();
            UserSession.Address = txtAddress.Text.Trim();

            MessageBox.Show("Profile updated successfully!", "Success",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnChangePassword_Click(object sender, EventArgs e)
        {
            string currentPw = txtCurrentPw.Text.Trim();
            string newPw     = txtNewPw.Text.Trim();
            string confirmPw = txtConfirmPw.Text.Trim();

            if (currentPw == "")
            {
                MessageBox.Show("Please enter your current password.", "Validation",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (newPw.Length < 6)
            {
                MessageBox.Show("New password must be at least 6 characters.", "Validation",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (newPw != confirmPw)
            {
                MessageBox.Show("New passwords do not match.", "Validation",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Verify current password
            string checkQuery =
                "SELECT COUNT(*) FROM Users WHERE UserID=@UserID AND Password=@Password";
            SqlParameter[] pCheck =
            {
                new SqlParameter("@UserID",   UserSession.UserID),
                new SqlParameter("@Password", currentPw)
            };
            if (Convert.ToInt32(DatabaseHelper.ExecuteScalar(checkQuery, pCheck)) == 0)
            {
                MessageBox.Show("Current password is incorrect.", "Wrong Password",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Update password
            string updatePw = "UPDATE Users SET Password=@NewPw WHERE UserID=@UserID";
            SqlParameter[] pUpdate =
            {
                new SqlParameter("@NewPw",  newPw),
                new SqlParameter("@UserID", UserSession.UserID)
            };
            DatabaseHelper.ExecuteNonQuery(updatePw, pUpdate);

            MessageBox.Show("Password changed successfully!", "Success",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            txtCurrentPw.Clear();
            txtNewPw.Clear();
            txtConfirmPw.Clear();
        }

        private void btnReset_Click(object sender, EventArgs e) { LoadProfile(); }
        private void btnClose_Click(object sender, EventArgs e) { this.Close(); }

        private void InitializeComponent()
        {
            this.lblTitle = new System.Windows.Forms.Label();
            this.tabProfile = new System.Windows.Forms.TabControl();
            this.tabPersonal = new System.Windows.Forms.TabPage();
            this.lblTPNumber = new System.Windows.Forms.Label();
            this.txtTPNumber = new System.Windows.Forms.TextBox();
            this.lblStaffID = new System.Windows.Forms.Label();
            this.txtStaffID = new System.Windows.Forms.TextBox();
            this.lblUsername = new System.Windows.Forms.Label();
            this.txtUsername = new System.Windows.Forms.TextBox();
            this.lblName = new System.Windows.Forms.Label();
            this.txtName = new System.Windows.Forms.TextBox();
            this.lblEmail = new System.Windows.Forms.Label();
            this.txtEmail = new System.Windows.Forms.TextBox();
            this.lblPhone = new System.Windows.Forms.Label();
            this.txtPhone = new System.Windows.Forms.TextBox();
            this.lblAddress = new System.Windows.Forms.Label();
            this.txtAddress = new System.Windows.Forms.TextBox();
            this.lblStudyLevel = new System.Windows.Forms.Label();
            this.cboStudyLevel = new System.Windows.Forms.ComboBox();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnReset = new System.Windows.Forms.Button();
            this.tabPassword = new System.Windows.Forms.TabPage();
            this.lblCurrentPw = new System.Windows.Forms.Label();
            this.txtCurrentPw = new System.Windows.Forms.TextBox();
            this.lblNewPw = new System.Windows.Forms.Label();
            this.txtNewPw = new System.Windows.Forms.TextBox();
            this.lblConfirmPw = new System.Windows.Forms.Label();
            this.txtConfirmPw = new System.Windows.Forms.TextBox();
            this.lblHint = new System.Windows.Forms.Label();
            this.btnChangePassword = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.tabProfile.SuspendLayout();
            this.tabPersonal.SuspendLayout();
            this.tabPassword.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblTitle
            // 
            this.lblTitle.Font = new System.Drawing.Font("Arial", 11F, System.Drawing.FontStyle.Bold);
            this.lblTitle.Location = new System.Drawing.Point(10, 10);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(485, 22);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "Update Profile";
            this.lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // tabProfile
            // 
            this.tabProfile.Controls.Add(this.tabPersonal);
            this.tabProfile.Controls.Add(this.tabPassword);
            this.tabProfile.Location = new System.Drawing.Point(10, 38);
            this.tabProfile.Name = "tabProfile";
            this.tabProfile.SelectedIndex = 0;
            this.tabProfile.Size = new System.Drawing.Size(485, 440);
            this.tabProfile.TabIndex = 1;
            // 
            // tabPersonal
            // 
            this.tabPersonal.Controls.Add(this.lblTPNumber);
            this.tabPersonal.Controls.Add(this.txtTPNumber);
            this.tabPersonal.Controls.Add(this.lblStaffID);
            this.tabPersonal.Controls.Add(this.txtStaffID);
            this.tabPersonal.Controls.Add(this.lblUsername);
            this.tabPersonal.Controls.Add(this.txtUsername);
            this.tabPersonal.Controls.Add(this.lblName);
            this.tabPersonal.Controls.Add(this.txtName);
            this.tabPersonal.Controls.Add(this.lblEmail);
            this.tabPersonal.Controls.Add(this.txtEmail);
            this.tabPersonal.Controls.Add(this.lblPhone);
            this.tabPersonal.Controls.Add(this.txtPhone);
            this.tabPersonal.Controls.Add(this.lblAddress);
            this.tabPersonal.Controls.Add(this.txtAddress);
            this.tabPersonal.Controls.Add(this.lblStudyLevel);
            this.tabPersonal.Controls.Add(this.cboStudyLevel);
            this.tabPersonal.Controls.Add(this.btnSave);
            this.tabPersonal.Controls.Add(this.btnReset);
            this.tabPersonal.Location = new System.Drawing.Point(4, 25);
            this.tabPersonal.Name = "tabPersonal";
            this.tabPersonal.Size = new System.Drawing.Size(477, 411);
            this.tabPersonal.TabIndex = 0;
            this.tabPersonal.Text = "Personal Info";
            // 
            // lblTPNumber
            // 
            this.lblTPNumber.Font = new System.Drawing.Font("Arial", 9F);
            this.lblTPNumber.Location = new System.Drawing.Point(15, 18);
            this.lblTPNumber.Name = "lblTPNumber";
            this.lblTPNumber.Size = new System.Drawing.Size(130, 22);
            this.lblTPNumber.TabIndex = 0;
            this.lblTPNumber.Text = "TP Number:";
            // 
            // txtTPNumber
            // 
            this.txtTPNumber.BackColor = System.Drawing.SystemColors.Control;
            this.txtTPNumber.Font = new System.Drawing.Font("Arial", 9F);
            this.txtTPNumber.Location = new System.Drawing.Point(155, 16);
            this.txtTPNumber.Name = "txtTPNumber";
            this.txtTPNumber.ReadOnly = true;
            this.txtTPNumber.Size = new System.Drawing.Size(285, 25);
            this.txtTPNumber.TabIndex = 1;
            // 
            // lblStaffID
            // 
            this.lblStaffID.Font = new System.Drawing.Font("Arial", 9F);
            this.lblStaffID.Location = new System.Drawing.Point(15, 18);
            this.lblStaffID.Name = "lblStaffID";
            this.lblStaffID.Size = new System.Drawing.Size(130, 22);
            this.lblStaffID.TabIndex = 2;
            this.lblStaffID.Text = "Staff ID:";
            // 
            // txtStaffID
            // 
            this.txtStaffID.BackColor = System.Drawing.SystemColors.Control;
            this.txtStaffID.Font = new System.Drawing.Font("Arial", 9F);
            this.txtStaffID.Location = new System.Drawing.Point(155, 16);
            this.txtStaffID.Name = "txtStaffID";
            this.txtStaffID.ReadOnly = true;
            this.txtStaffID.Size = new System.Drawing.Size(285, 25);
            this.txtStaffID.TabIndex = 3;
            // 
            // lblUsername
            // 
            this.lblUsername.Font = new System.Drawing.Font("Arial", 9F);
            this.lblUsername.Location = new System.Drawing.Point(15, 53);
            this.lblUsername.Name = "lblUsername";
            this.lblUsername.Size = new System.Drawing.Size(130, 22);
            this.lblUsername.TabIndex = 4;
            this.lblUsername.Text = "Username:";
            // 
            // txtUsername
            // 
            this.txtUsername.BackColor = System.Drawing.SystemColors.Control;
            this.txtUsername.Font = new System.Drawing.Font("Arial", 9F);
            this.txtUsername.Location = new System.Drawing.Point(155, 51);
            this.txtUsername.Name = "txtUsername";
            this.txtUsername.ReadOnly = true;
            this.txtUsername.Size = new System.Drawing.Size(285, 25);
            this.txtUsername.TabIndex = 5;
            // 
            // lblName
            // 
            this.lblName.Font = new System.Drawing.Font("Arial", 9F);
            this.lblName.Location = new System.Drawing.Point(15, 88);
            this.lblName.Name = "lblName";
            this.lblName.Size = new System.Drawing.Size(130, 22);
            this.lblName.TabIndex = 6;
            this.lblName.Text = "Full Name: *";
            // 
            // txtName
            // 
            this.txtName.Font = new System.Drawing.Font("Arial", 9F);
            this.txtName.Location = new System.Drawing.Point(155, 86);
            this.txtName.Name = "txtName";
            this.txtName.Size = new System.Drawing.Size(285, 25);
            this.txtName.TabIndex = 7;
            this.txtName.Text = "Tanmay";
            this.txtName.TextChanged += new System.EventHandler(this.txtName_TextChanged);
            // 
            // lblEmail
            // 
            this.lblEmail.Font = new System.Drawing.Font("Arial", 9F);
            this.lblEmail.Location = new System.Drawing.Point(15, 123);
            this.lblEmail.Name = "lblEmail";
            this.lblEmail.Size = new System.Drawing.Size(130, 22);
            this.lblEmail.TabIndex = 8;
            this.lblEmail.Text = "Email: *";
            // 
            // txtEmail
            // 
            this.txtEmail.Font = new System.Drawing.Font("Arial", 9F);
            this.txtEmail.Location = new System.Drawing.Point(155, 121);
            this.txtEmail.Name = "txtEmail";
            this.txtEmail.Size = new System.Drawing.Size(285, 25);
            this.txtEmail.TabIndex = 9;
            // 
            // lblPhone
            // 
            this.lblPhone.Font = new System.Drawing.Font("Arial", 9F);
            this.lblPhone.Location = new System.Drawing.Point(15, 158);
            this.lblPhone.Name = "lblPhone";
            this.lblPhone.Size = new System.Drawing.Size(130, 22);
            this.lblPhone.TabIndex = 10;
            this.lblPhone.Text = "Phone: *";
            // 
            // txtPhone
            // 
            this.txtPhone.Font = new System.Drawing.Font("Arial", 9F);
            this.txtPhone.Location = new System.Drawing.Point(155, 156);
            this.txtPhone.Name = "txtPhone";
            this.txtPhone.Size = new System.Drawing.Size(285, 25);
            this.txtPhone.TabIndex = 11;
            // 
            // lblAddress
            // 
            this.lblAddress.Font = new System.Drawing.Font("Arial", 9F);
            this.lblAddress.Location = new System.Drawing.Point(15, 193);
            this.lblAddress.Name = "lblAddress";
            this.lblAddress.Size = new System.Drawing.Size(130, 22);
            this.lblAddress.TabIndex = 12;
            this.lblAddress.Text = "Address:";
            // 
            // txtAddress
            // 
            this.txtAddress.Font = new System.Drawing.Font("Arial", 9F);
            this.txtAddress.Location = new System.Drawing.Point(155, 191);
            this.txtAddress.Multiline = true;
            this.txtAddress.Name = "txtAddress";
            this.txtAddress.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtAddress.Size = new System.Drawing.Size(285, 55);
            this.txtAddress.TabIndex = 13;
            // 
            // lblStudyLevel
            // 
            this.lblStudyLevel.Font = new System.Drawing.Font("Arial", 9F);
            this.lblStudyLevel.Location = new System.Drawing.Point(15, 255);
            this.lblStudyLevel.Name = "lblStudyLevel";
            this.lblStudyLevel.Size = new System.Drawing.Size(130, 22);
            this.lblStudyLevel.TabIndex = 14;
            this.lblStudyLevel.Text = "Study Level:";
            // 
            // cboStudyLevel
            // 
            this.cboStudyLevel.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboStudyLevel.Font = new System.Drawing.Font("Arial", 9F);
            this.cboStudyLevel.Items.AddRange(new object[] {
            "Foundation",
            "Level 1",
            "Level 2",
            "Level 3"});
            this.cboStudyLevel.Location = new System.Drawing.Point(155, 253);
            this.cboStudyLevel.Name = "cboStudyLevel";
            this.cboStudyLevel.Size = new System.Drawing.Size(180, 25);
            this.cboStudyLevel.TabIndex = 15;
            // 
            // btnSave
            // 
            this.btnSave.Font = new System.Drawing.Font("Arial", 9F);
            this.btnSave.Location = new System.Drawing.Point(15, 295);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(105, 27);
            this.btnSave.TabIndex = 16;
            this.btnSave.Text = "Save Changes";
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnReset
            // 
            this.btnReset.Font = new System.Drawing.Font("Arial", 9F);
            this.btnReset.Location = new System.Drawing.Point(130, 295);
            this.btnReset.Name = "btnReset";
            this.btnReset.Size = new System.Drawing.Size(75, 27);
            this.btnReset.TabIndex = 17;
            this.btnReset.Text = "Reset";
            this.btnReset.Click += new System.EventHandler(this.btnReset_Click);
            // 
            // tabPassword
            // 
            this.tabPassword.Controls.Add(this.lblCurrentPw);
            this.tabPassword.Controls.Add(this.txtCurrentPw);
            this.tabPassword.Controls.Add(this.lblNewPw);
            this.tabPassword.Controls.Add(this.txtNewPw);
            this.tabPassword.Controls.Add(this.lblConfirmPw);
            this.tabPassword.Controls.Add(this.txtConfirmPw);
            this.tabPassword.Controls.Add(this.lblHint);
            this.tabPassword.Controls.Add(this.btnChangePassword);
            this.tabPassword.Location = new System.Drawing.Point(4, 25);
            this.tabPassword.Name = "tabPassword";
            this.tabPassword.Size = new System.Drawing.Size(477, 411);
            this.tabPassword.TabIndex = 1;
            this.tabPassword.Text = "Change Password";
            // 
            // lblCurrentPw
            // 
            this.lblCurrentPw.Font = new System.Drawing.Font("Arial", 9F);
            this.lblCurrentPw.Location = new System.Drawing.Point(15, 40);
            this.lblCurrentPw.Name = "lblCurrentPw";
            this.lblCurrentPw.Size = new System.Drawing.Size(140, 22);
            this.lblCurrentPw.TabIndex = 0;
            this.lblCurrentPw.Text = "Current Password:";
            // 
            // txtCurrentPw
            // 
            this.txtCurrentPw.Font = new System.Drawing.Font("Arial", 9F);
            this.txtCurrentPw.Location = new System.Drawing.Point(165, 38);
            this.txtCurrentPw.Name = "txtCurrentPw";
            this.txtCurrentPw.Size = new System.Drawing.Size(270, 25);
            this.txtCurrentPw.TabIndex = 1;
            this.txtCurrentPw.UseSystemPasswordChar = true;
            // 
            // lblNewPw
            // 
            this.lblNewPw.Font = new System.Drawing.Font("Arial", 9F);
            this.lblNewPw.Location = new System.Drawing.Point(15, 80);
            this.lblNewPw.Name = "lblNewPw";
            this.lblNewPw.Size = new System.Drawing.Size(140, 22);
            this.lblNewPw.TabIndex = 2;
            this.lblNewPw.Text = "New Password:";
            // 
            // txtNewPw
            // 
            this.txtNewPw.Font = new System.Drawing.Font("Arial", 9F);
            this.txtNewPw.Location = new System.Drawing.Point(165, 78);
            this.txtNewPw.Name = "txtNewPw";
            this.txtNewPw.Size = new System.Drawing.Size(270, 25);
            this.txtNewPw.TabIndex = 3;
            this.txtNewPw.UseSystemPasswordChar = true;
            // 
            // lblConfirmPw
            // 
            this.lblConfirmPw.Font = new System.Drawing.Font("Arial", 9F);
            this.lblConfirmPw.Location = new System.Drawing.Point(15, 120);
            this.lblConfirmPw.Name = "lblConfirmPw";
            this.lblConfirmPw.Size = new System.Drawing.Size(140, 22);
            this.lblConfirmPw.TabIndex = 4;
            this.lblConfirmPw.Text = "Confirm Password:";
            // 
            // txtConfirmPw
            // 
            this.txtConfirmPw.Font = new System.Drawing.Font("Arial", 9F);
            this.txtConfirmPw.Location = new System.Drawing.Point(165, 118);
            this.txtConfirmPw.Name = "txtConfirmPw";
            this.txtConfirmPw.Size = new System.Drawing.Size(270, 25);
            this.txtConfirmPw.TabIndex = 5;
            this.txtConfirmPw.UseSystemPasswordChar = true;
            // 
            // lblHint
            // 
            this.lblHint.Font = new System.Drawing.Font("Arial", 8F, System.Drawing.FontStyle.Italic);
            this.lblHint.Location = new System.Drawing.Point(165, 145);
            this.lblHint.Name = "lblHint";
            this.lblHint.Size = new System.Drawing.Size(250, 18);
            this.lblHint.TabIndex = 6;
            this.lblHint.Text = "Minimum 6 characters required.";
            // 
            // btnChangePassword
            // 
            this.btnChangePassword.Font = new System.Drawing.Font("Arial", 9F);
            this.btnChangePassword.Location = new System.Drawing.Point(15, 180);
            this.btnChangePassword.Name = "btnChangePassword";
            this.btnChangePassword.Size = new System.Drawing.Size(135, 27);
            this.btnChangePassword.TabIndex = 7;
            this.btnChangePassword.Text = "Change Password";
            this.btnChangePassword.Click += new System.EventHandler(this.btnChangePassword_Click);
            // 
            // btnClose
            // 
            this.btnClose.Font = new System.Drawing.Font("Arial", 9F);
            this.btnClose.Location = new System.Drawing.Point(415, 485);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(80, 27);
            this.btnClose.TabIndex = 2;
            this.btnClose.Text = "Close";
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // frmUpdateProfile
            // 
            this.ClientSize = new System.Drawing.Size(502, 483);
            this.Controls.Add(this.lblTitle);
            this.Controls.Add(this.tabProfile);
            this.Controls.Add(this.btnClose);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Name = "frmUpdateProfile";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Update My Profile - Tanmay";
            this.Load += new System.EventHandler(this.frmUpdateProfile_Load);
            this.tabProfile.ResumeLayout(false);
            this.tabPersonal.ResumeLayout(false);
            this.tabPersonal.PerformLayout();
            this.tabPassword.ResumeLayout(false);
            this.tabPassword.PerformLayout();
            this.ResumeLayout(false);

        }

        private void txtName_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
