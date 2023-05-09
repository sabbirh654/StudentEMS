namespace StudentEMS.Models
{
    public class Subject
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string CreditHour { get; set; }
        public string Department { get; set; }
        public string Semester { get; set; }
        public string PrerequisiteSubjectList { get; set; }
    }
}
