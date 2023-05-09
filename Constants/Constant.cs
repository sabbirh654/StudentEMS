namespace StudentEMS.Constants
{
    public static class Constant
    {
        public const int DefaultPage = 1;
        public const int DefaultPageSize = 25;
        public const int LogoutTimeInMinutes = 10;
        public const int RememberUserTimeInDays = 7;
        public static readonly int[] PageSizeList = { 10, 25, 50, 100 };
        public static readonly double[] creditHourList = { 0.5, 0.75, 1.00, 1.5, 2.0, 3.0 };
        public static readonly double[] GradeList = { 4.0, 3.75, 3.50, 3.25, 3.00, 2.75, 2.50, 2.25, 2.00, 0.00 };
        public const string DefaultDateFormat = "MM-dd-yyyy";
        public const string RegistryPath = @"Computer\HKEY_CURRENT_USER\SOFTWARE\Classes\StudentEMS";
        public const string DummyImagePath = @"..\..\Assets\dummy image.png";
        public const string ImageExtensionFilter = @"Image files (*.jpg, *.jpeg, *.png) | *.jpg; *.jpeg; *.png";

        public enum UserRole
        {
            Student,
            Staff
        }
        public enum AuditLogAction
        {
            Add = 1,
            Update,
            Delete,
            Login,
            Logout
        }
        public enum NagivationMenuItem
        {
            StudentHome,
            StudentDetails,
            StudentUpdateProfile,

            StaffHome,
            StaffDetails,
            StaffUpdateProfile,

            StaffList,
            Subject,
            AddStudent,
            AuditLog,

            Course
        }

        public enum SubjectColumnIndex
        {
            Code,
            Name,
            CreditHour,
            Department,
            Semester,
            Prerequisites
        }

        public enum StudentColumnIndex
        {
            FirstName,
            LastName,
            Email,
            Session,
            Department,
            CGPA,
            LastEnrolledSemester,
            TotalEarnedCredits
        }

        public enum CourseColumnIndex
        {
            SubjectCode,
            SubjectName,
            CreditHour,
            Grade
        }

        public enum Operation
        {
            Add,
            Update,
            Delete,
            None
        }

        public enum NavigationItem
        {
            Home,
            UpdateProfile,
            Subject,
            Exit
        }
    }
}
