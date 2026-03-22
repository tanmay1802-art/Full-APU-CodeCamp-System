namespace APUCodeCamp
{
    // Stores the currently logged-in user's data.
    // Static so all forms share the same session without passing objects.
    public class UserSession
    {
        public static int    UserID     { get; set; }
        public static string Role       { get; set; }   // Administrator | Lecturer | Trainer | Student
        public static string Name       { get; set; }
        public static string Email      { get; set; }
        public static string Phone      { get; set; }
        public static string Address    { get; set; }
        public static string Username   { get; set; }

        // Role-specific IDs
        public static int    AdminID    { get; set; }
        public static string AdminStaffID { get; set; }

        public static int    LecturerID { get; set; }
        public static string LecturerStaffID { get; set; }

        public static int    TrainerID  { get; set; }
        public static string TrainerStaffID { get; set; }
        public static string TrainerSpecialisation { get; set; }

        public static int    StudentID  { get; set; }
        public static string TPNumber   { get; set; }
        public static string StudyLevel { get; set; }

        // Call this on logout to clear everything
        public static void ClearSession()
        {
            UserID     = 0;
            Role       = "";
            Name       = "";
            Email      = "";
            Phone      = "";
            Address    = "";
            Username   = "";

            AdminID    = 0; AdminStaffID = "";
            LecturerID = 0; LecturerStaffID = "";
            TrainerID  = 0; TrainerStaffID = ""; TrainerSpecialisation = "";
            StudentID  = 0; TPNumber = ""; StudyLevel = "";
        }
    }

    // ── Backward-compatible alias used by the original student forms ──
    // The existing student .cs files reference StudentSession directly.
    public class StudentSession
    {
        public static int    UserID     { get { return UserSession.UserID;    } set { UserSession.UserID    = value; } }
        public static int    StudentID  { get { return UserSession.StudentID; } set { UserSession.StudentID = value; } }
        public static string Name       { get { return UserSession.Name;      } set { UserSession.Name      = value; } }
        public static string TPNumber   { get { return UserSession.TPNumber;  } set { UserSession.TPNumber  = value; } }
        public static string StudyLevel { get { return UserSession.StudyLevel;} set { UserSession.StudyLevel= value; } }
        public static string Email      { get { return UserSession.Email;     } set { UserSession.Email     = value; } }
        public static string Phone      { get { return UserSession.Phone;     } set { UserSession.Phone     = value; } }
        public static string Address    { get { return UserSession.Address;   } set { UserSession.Address   = value; } }
        public static string Username   { get { return UserSession.Username;  } set { UserSession.Username  = value; } }

        public static void ClearSession() { UserSession.ClearSession(); }
    }
}
