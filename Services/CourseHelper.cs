using Microsoft.Extensions.DependencyInjection;

using StudentEMS.AppData;
using StudentEMS.Models;
using StudentEMS.Services.Interfaces;

using System;
using System.Collections.Generic;
using System.Data;
using System.Windows;

namespace StudentEMS.Services
{
    public class CourseHelper : ICourseHelper
    {
        private IDatabaseAccess _databaseAccess = App.ServiceProvider.GetService<IDatabaseAccess>();
        private ISubjectHelper _subjectHelper = App.ServiceProvider.GetService<ISubjectHelper>();

        private static string AddCourseQuery =>
            @"
            INSERT INTO studentsubjects
                (
                    StudentId,
                    SubjectId,
                    SubjectGrade,
                    CreatedBy,
                    CreatedDate,
                    UpdatedBy,
                    UpdatedDate
                )
            VALUES 
                (
                    @studentId,
                    @subjectId,
                    @subjectGrade,
                    @createdBy,
                    @createdDate,
                    @updatedBy,
                    @updatedDate
                )";

        private static string IsSameCourseExistsQuery =>
            @"SELECT COUNT(Id) FROM studentsubjects WHERE StudentId = @studentId AND SubjectId = @subjectId";

        private static string GetCourseDetailsForAStudentQuery =>
            @"
            SELECT 
                s.SubjectId AS subjectId,
                s.Name AS subjectName,
                s.CreditHour AS creditHour,
                d.Name AS department,
                ss.SubjectGrade AS subjectGrade,
                sm.Name AS semester
                
            FROM 
                subjects s
            JOIN 
                departments d ON d.Id = s.DepartmentId
            JOIN
                semesters sm ON sm.Id = s. SemesterId
            JOIN 
                studentsubjects ss ON ss.SubjectId = s.Id
            WHERE 
                ss.StudentId = @studentId
            LIMIT @offset,@limit";

        private static string CountCourseForAStudentQuery =>
            @"
            SELECT 
                COUNT(ss.Id) 
            FROM 
                subjects s
            JOIN 
                studentsubjects ss ON ss.SubjectId = s.Id
            WHERE 
                ss.StudentId = @studentId AND s.SemesterId = @semesterId";

        private static string DeleteCoursesQuery =>
            @"DELETE FROM studentsubjects WHERE SubjectId = @subjectId AND studentId = @studentId";

        private static string UpdateCourseQuery =>
            @"
            UPDATE 
                studentsubjects 
            SET
                SubjectGrade = @subjectGrade,
                UpdatedBy = @updatedBy,
                UpdatedDate = @updatedDate
            WHERE
                StudentId = @studentId AND
                SubjectId = @subjectId";

        private static string DeleteCourseQueryOnSubjectDelete =>
            @"DELETE FROM studentsubjects WHERE SubjectId = @subjectId";

        private static string DeleteCourseQueryOnStudentDelete =>
            @"DELETE FROM studentsubjects WHERE StudentId = @studentId";

        public bool AddCourse(Student student, Course course)
        {
            string subject = course.SubjectCode;
            string[] parts = subject.Split('-');

            var parameters = new Dictionary<string, object>()
            {
                {"studentId", student.StudentId },
                {"subjectId", _subjectHelper.GetSubjectId(Convert.ToInt32(parts[1]), parts[0]) },
                {"subjectGrade", course.Grade },
                {"createdBy", "sabbir" },
                {"createdDate", DateTime.Now.ToString("yyyy-MM-dd")},
                {"updatedBy", "sabbir" },
                {"updatedDate", DateTime.Now.ToString("yyyy-MM-dd") }
            };

            try
            {
                _databaseAccess.ExecuteQuery(AddCourseQuery, parameters);
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
        }

        public bool IsSameCourseExists(Student student, Course course)
        {
            string subject = course.SubjectCode;
            string[] parts = subject.Split('-');

            var parameters = new Dictionary<string, object>()
            {
                {"studentId", student.StudentId },
                {"subjectId", _subjectHelper.GetSubjectId(Convert.ToInt32(parts[1]), parts[0]) },
            };

            try
            {
                string count = _databaseAccess.GetSingleData(IsSameCourseExistsQuery, parameters);

                if (count == null || count == "0")
                {
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
        }

        public List<Course> GetCourseDetailsForAStudent(int limit, int offset, Student student)
        {
            List<Course> courseList = new List<Course>();

            var parameters = new Dictionary<string, object>()
            {
                {"limit" , limit},
                {"offset", offset },
                {"studentId", student.StudentId },
            };

            DataTable dataTable = _databaseAccess.GetData(GetCourseDetailsForAStudentQuery, parameters);

            foreach (DataRow row in dataTable.Rows)
            {
                Course course = new Course();

                course.SubjectCode = $"{row["department"].ToString()}-{row["subjectId"].ToString()}";
                course.SubjectName = row["subjectName"].ToString();
                course.Grade = Convert.ToDouble(row["subjectGrade"]).ToString("F2");
                course.CreditHour = Convert.ToDouble(row["creditHour"]).ToString("F2");
                course.Semester = row["semester"].ToString();

                courseList.Add(course);
            }

            return courseList;
        }

        public int CountCourseForAStudent(Student student, string semester)
        {
            var parameters = new Dictionary<string, object>()
            {
                {"studentId", student.StudentId },
                {"semesterId", _subjectHelper.GetSemesterId(semester) }
            };

            int count = Convert.ToInt32(_databaseAccess.GetSingleData(CountCourseForAStudentQuery, parameters));

            return count;
        }

        public bool DeleteCourse(Student student, Course course)
        {
            string subject = course.SubjectCode;
            string[] parts = subject.Split('-');

            var parameters = new Dictionary<string, object>()
            {
                {"studentId", student.StudentId },
                {"subjectId", _subjectHelper.GetSubjectId(Convert.ToInt32(parts[1]), parts[0]) },
            };

            try
            {
                _databaseAccess.ExecuteQuery(DeleteCoursesQuery, parameters);
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
        }

        public bool UpdateCourse(Student student, Course course)
        {
            string subject = course.SubjectCode;
            string[] parts = subject.Split('-');

            var parameters = new Dictionary<string, object>()
            {
                {"studentId", student.StudentId },
                {"subjectId", _subjectHelper.GetSubjectId(Convert.ToInt32(parts[1]), parts[0]) },
                {"subjectGrade", course.Grade },
                {"updatedBy", "sabbir" },
                {"updatedDate", DateTime.Now.ToString("yyyy-MM-dd") }
            };

            try
            {
                _databaseAccess.ExecuteQuery(UpdateCourseQuery, parameters);
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
        }

        public bool DeleteCoursesWhenASubjectDeleted(int subjectId)
        {
            var parameters = new Dictionary<string, object>()
            {
                {"subjectId", @subjectId},
            };

            try
            {
                _databaseAccess.ExecuteQuery(DeleteCourseQueryOnSubjectDelete, parameters);
                return true;
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
        }

        public bool DeleteCoursesWhenAStudentDeleted(int studentId)
        {
            var parameters = new Dictionary<string, object>()
            {
                {"studentId", studentId},
            };

            try
            {
                _databaseAccess.ExecuteQuery(DeleteCourseQueryOnStudentDelete, parameters);
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
        }
    }
}
