namespace APUCodeCamp
{
    // Model class – holds a student's data
    public class Student
    {
        public int    StudentID  { get; set; }
        public int    UserID     { get; set; }
        public string TPNumber   { get; set; }
        public string Name       { get; set; }
        public string Email      { get; set; }
        public string Phone      { get; set; }
        public string Address    { get; set; }
        public string StudyLevel { get; set; }
        public string Username   { get; set; }

        public Student()
        {
            StudentID  = 0; UserID = 0;
            TPNumber   = ""; Name = ""; Email = "";
            Phone      = ""; Address = ""; StudyLevel = ""; Username = "";
        }

        // Returns "Name (TP Number)" display string
        public string GetDisplayName()
        {
            return Name + " (" + TPNumber + ")";
        }
    }
}
