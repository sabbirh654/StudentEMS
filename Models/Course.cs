using StudentEMS.Constants;


namespace StudentEMS.Models
{
    public class Course
    {
        public string SubjectCode { get; set; }
        public string SubjectName { get; set; }
        public string CreditHour { get; set; }
        public string Semester { get; set; }
        public string Grade { get; set; }
        public Constant.Operation DbStatus { get; set; }
        public bool IsExistsInDatabase { get; set; }

    }
}
