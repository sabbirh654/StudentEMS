using StudentEMS.Models;

namespace StudentEMS
{
    public class Student: User
    {
        public int StudentId { get; set; }
        public string Session { get; set; }
        public string Department { get; set; }
        public new string Password { get; set; }
        public bool IsDeleted { get; set; }
        public string CGPA { get; set; }
        public string LastEnrolledSemester { get; set; }
        public string TotalEarnedCredits { get; set; }
    }
}
