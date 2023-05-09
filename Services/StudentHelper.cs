using System.Collections.Generic;
using System.Data;
using System;
using StudentEMS.Services.Interfaces;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;

namespace StudentEMS.Services
{
    public class StudentHelper : IStudentHelper
    {
        private IDatabaseAccess _databaseAccess = App.ServiceProvider.GetService<IDatabaseAccess>();

        public static string GetAllStudentsInfoQuery =>
            @"
            SELECT
                s.Id as StudentId,
                u.FirstName AS FirstName,
                u.LastName AS LastName,
                u.Email AS Email,
                IFNULL(u.ContactNumber,'N/A') as ContactNumber,
                u.DOB AS DOB,
                s.Session AS Session,
                d.Name as Department
            FROM users u
            INNER JOIN students s
                ON u.Id = s.UserId
            INNER JOIN departments d 
                ON s.DepartmentId = d.Id
            WHERE
                s.IsDeleted = FALSE AND
                (
                @searchText = '' OR
                (
                    u.FirstName LIKE @searchText OR
                    u.LastName LIKE @searchText OR
                    u.Email LIKE @searchText OR
                    u.ContactNumber LIKE @searchText OR
                    d.Name LIKE @searchText
                )
                )
            LIMIT @offset, @limit";

        public static string GetStudentCountQuery =>
            @"
            SELECT
                COUNT(u.Id)
            FROM users u
            INNER JOIN students s
                ON u.Id = s.UserId
            INNER JOIN departments d 
                ON s.DepartmentId = d.Id
            WHERE 
                s.IsDeleted = FALSE
                AND(
                        @searchText = '' OR
                        (
                            u.FirstName LIKE @searchText OR
                            u.LastName LIKE @searchText OR
                            u.Email LIKE @searchText OR
                            u.ContactNumber LIKE @searchText OR
                            d.Name LIKE @searchText
                        )
                    )";

        private static string GetSingleStudentInfoQuery =>
            @"
            SELECT 
                s.Id AS studentId,
                s.Password AS password,
                u.DOB AS birthDate,
                u.ImagePath AS ImagePath,
                IFNULL(u.ContactNumber,'N/A') as ContactNumber
            FROM 
                students s
            JOIN 
                users u ON s.UserId = u.Id
            WHERE 
                u.Email = @email";

        private static string GetUserIdOfStudentQuery =>
            @"
            SELECT 
                UserId
            FROM 
                students
            WHERE 
                Id = @studentId";

        private static string DeleteStudentQuery =>
            @"UPDATE students SET IsDeleted = TRUE where Id = @studentId";

        private static string UpdateStudentQuery =>
            @"
            UPDATE 
                users u
            SET 
                u.FirstName = @firstName,
                u.LastName = @lastName,
                u.ContactNumber = @contactNumber,
                u.DOB = @dateOfBirth,
                u.ImagePath = @imagePath
            WHERE 
                u.Email = @email";

        private static string GetStudentCGPAQuery =>
            @"
            SELECT 
                SUM(f.weightedSum) / SUM(f.credits) AS cgpa
            FROM
            (
                SELECT 
                    SUM(ss.SubjectGrade * s.CreditHour) AS weightedSum, 
                    SUM(s.CreditHour) AS credits
                FROM 
                    studentsubjects ss
                JOIN 
                    students st ON st.Id = ss.StudentId 
                JOIN 
                    subjects s ON s.Id = ss.SubjectId 
                JOIN 
                    semesters sm ON sm.Id = s.SemesterId
                WHERE 
                    ss.StudentId = @studentId
                GROUP BY 
                    sm.Id
            ) AS f";

        private static string GetLastEnrolledSemesterQuery =>
            @"
            SELECT Name
                FROM semesters
            WHERE 
                Id = 
                (
                    SELECT 
                        MAX(sm.Id) AS semesterId
                    FROM 
                        studentsubjects ss
                    JOIN 
                        students st ON st.Id = ss.StudentId 
                    JOIN 
                        subjects s ON s.Id = ss.SubjectId 
                    JOIN 
                        semesters sm ON sm.Id = s.SemesterId
                    WHERE 
                        ss.StudentId = @studentId
                )";

        private static string GetTotalEarnedCreditsQuery =>
           @" 
            SELECT 
                SUM(s.CreditHour) AS totalCreditHour
            FROM 
                studentsubjects ss
            JOIN 
                students st ON st.Id = ss.StudentId 
            JOIN 
                subjects s ON s.Id = ss.SubjectId 
            JOIN 
                semesters sm ON sm.Id = s.SemesterId
            WHERE 
                ss.StudentId = @studentId";

        private static string GetImagePathQuery =>
            "SELECT ImagePath FROM users WHERE Email = @email";

        public bool UpdateStudent(Student student)
        {
            var parameters = new Dictionary<string, object>
            {
                {"firstName", student.FirstName},
                {"lastName", student.LastName},
                {"contactNumber", student.ContactNumber},
                {"dateOfBirth", student.DateOfBirth},
                {"email", student.Email},
                {"imagePath", student.ProfilePicturePath }
            };

            try
            {
                _databaseAccess.ExecuteQuery(UpdateStudentQuery, parameters);
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
        }



        public Student GetSingleStudentInfo(string email)
        {
            var parameters = new Dictionary<string, object>
            {
                {"email", email }
            };

            Student student = new Student();
            DataTable dataTable = _databaseAccess.GetData(GetSingleStudentInfoQuery, parameters);
            DataRow row = dataTable.Rows[0];

            student.StudentId = Convert.ToInt32(row["studentId"].ToString());
            student.DateOfBirth = Convert.ToDateTime(row["birthDate"]);
            student.DateOfBirth.ToString("yyyy-MM-dd");
            student.ContactNumber = row["ContactNumber"].ToString();
            student.Password = row["password"].ToString();
            student.ProfilePicturePath = row["ImagePath"].ToString();

            return student;
        }
        public List<Student> GetAllStudentInfo(int limit, int offset, string filterText)
        {
            List<Student> students = new List<Student>();

            var parameters = new Dictionary<string, object>
            {
                {"limit", limit },
                {"offset", offset },
                { "searchText", "%" + filterText + "%"}
            };

            DataTable dataTable = _databaseAccess.GetData(GetAllStudentsInfoQuery, parameters);

            foreach (DataRow row in dataTable.Rows)
            {
                Student student = new Student();

                student.FirstName = row["FirstName"].ToString();
                student.LastName = row["LastName"].ToString();
                student.Email = row["Email"].ToString();
                student.Session = row["Session"].ToString();
                student.Department = row["Department"].ToString();

                int studentId = Convert.ToInt32(row["StudentId"]);

                student.CGPA = GetStudentCGPA(studentId);
                student.LastEnrolledSemester = GetLastEnrolledSemester(studentId);
                student.TotalEarnedCredits = GetTotalEarnedCredits(studentId);

                students.Add(student);
            }

            return students;
        }

        public int GetStudentCount(string filterText)
        {
            var parameters = new Dictionary<string, object>
            {
                { "searchText", "%" + filterText + "%"},
            };

            int count = Convert.ToInt32(_databaseAccess.GetSingleData(GetStudentCountQuery, parameters));

            return count;
        }

        public int GetUserIdOfStudent(int studentId)
        {
            var parameters = new Dictionary<string, object>
            {
                {"studentId", studentId }
            };

            int userId = Convert.ToInt32(_databaseAccess.GetSingleData(GetUserIdOfStudentQuery, parameters));
            return userId;
        }

        public bool DeleteStudent(int studentId)
        {
            var parameters = new Dictionary<string, object>()
            {
                { "studentId", studentId },
            };

            try
            {
                _databaseAccess.ExecuteQuery(DeleteStudentQuery, parameters);
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
        }

        public string GetStudentCGPA(int studentId)
        {
            var parameters = new Dictionary<string, object>()
            {
                { "studentId", studentId },
            };

            string cgpa = _databaseAccess.GetSingleData(GetStudentCGPAQuery, parameters);

            if(string.IsNullOrEmpty(cgpa))
            {
                return "N\\A";
            }

            return Convert.ToDouble(cgpa).ToString("F2");
        }

        public string GetLastEnrolledSemester(int studentId)
        {
            var parameters = new Dictionary<string, object>()
            {
                { "studentId", studentId },
            };

            string lastEnrolledSemester = _databaseAccess.GetSingleData(GetLastEnrolledSemesterQuery, parameters);

            if(string.IsNullOrEmpty(lastEnrolledSemester))
            {
                return "N\\A";
            }

            return lastEnrolledSemester;
        }

        public string GetTotalEarnedCredits(int studentId)
        {
            var parameters = new Dictionary<string, object>()
            {
                { "studentId", studentId },
            };

            string totalEarnedCredits = _databaseAccess.GetSingleData(GetTotalEarnedCreditsQuery, parameters);

            if (string.IsNullOrEmpty(totalEarnedCredits))
            {
                return "N\\A";
            }

            return Convert.ToDouble(totalEarnedCredits).ToString("F2");
        }

        public string GetStudentImagePath(string email)
        {
            var parameters = new Dictionary<string, object>()
            {
                { "email", email },
            };

            string imagePath = _databaseAccess.GetSingleData(GetImagePathQuery, parameters);

            if (string.IsNullOrEmpty(imagePath))
            {
                return "";
            }

            return imagePath;
        }
    }
}
