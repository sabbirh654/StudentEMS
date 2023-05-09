using Microsoft.Extensions.DependencyInjection;

using StudentEMS.Models;
using StudentEMS.Services.Interfaces;

using System;
using System.Collections.Generic;
using System.Data;
using System.Windows;

namespace StudentEMS.Services
{
    public class SubjectHelper : ISubjectHelper
    {
        private IDatabaseAccess _databaseAccess = App.ServiceProvider.GetService<IDatabaseAccess>();

        #region Queries

        private static string GetSemestersQuery =>
            @"SELECT Name FROM semesters";
        private static string GetDepartmentsQuery =>
            @"SELECT Name FROM departments";
        private static string GetSemesterIdQuery =>
            @"SELECT Id FROM semesters WHERE Name = @semesterName";
        private static string GetDepartmentIdQuery =>
            @"SELECT Id FROM departments WHERE Name = @departmentName";
        private static string GetSemesterWiseSubjectPrerequisitesQuery =>
            @"
            SELECT 
                SubjectId, Name
            FROM 
                subjects 
            WHERE 
                (SemesterId < @semesterId) AND 
                (DepartmentId = @departmentId)";

        private static string GetSemesterWiseSubjectsQuery =>
            @"
            SELECT 
                SubjectId, Name
            FROM 
                subjects 
            WHERE 
                (SemesterId = @semesterId) AND 
                (DepartmentId = @departmentId)";

        private static string IsSubjectBelongsToDepartmentQuery =>
           @"
            SELECT 
                COUNT(Id) 
            FROM 
                subjects 
            WHERE 
                Name = @subjectName AND 
                DepartmentId = @departmentId 
            GROUP BY 
                DepartmentId";
        private static string IsSubjectCodeSameQuery =>
            @"
            SELECT 
                COUNT(Id) 
            FROM 
                subjects 
            WHERE 
                SubjectId = @subjectId AND
                DepartmentId = @departmentId";
        private static string AddSubjectQuery =>
            @"
            INSERT INTO Subjects  
                (SubjectId, 
                Name, 
                CreditHour, 
                DepartmentId, 
                SemesterId, 
                CreatedBy, 
                CreatedDate, 
                UpdatedBy, 
                UpdatedDate
                ) 
            VALUES  
                (@SubjectId, 
                @Name, 
                @CreditHour, 
                @DepartmentId, 
                @SemesterId, 
                @CreatedBy, 
                @CreatedDate, 
                @UpdatedBy, 
                @UpdatedDate
                )";
        private static string UpdateSubjectQuery =>
            @"
            UPDATE 
                subjects 
            SET 
                Name = @Name, 
                CreditHour = @CreditHour,
                SemesterId = @SemesterId, 
                UpdatedBy = @UpdatedBy, 
                UpdatedDate = @UpdatedDate 
            WHERE 
                SubjectId = @subjectId AND
                DepartmentId = @DepartmentId";
        private static string AddPrerequisiteSubjectsQuery =>
            @" 
            INSERT INTO subjectprerequisites
                (SubjectId, 
                PrerequisiteSubjectId
                )
            VALUES 
                (@subjectId, 
                @prerequisiteId
                )";
        private static string IsSubjectBelongsToPrerequisiteQuery =>
            @"
            SELECT 
                COUNT(PrerequisiteSubjectId) 
            FROM 
                subjectprerequisites 
            WHERE 
                PrerequisiteSubjectId = @subjectId";
        private static string DeletePrerequisiteSubjectsQuery =>
            @"
            DELETE FROM 
                subjectprerequisites 
            WHERE 
                subjectId = @subjectId";
        private static string DeleteSubjectQuery =>
            @"
            DELETE FROM 
                subjects 
            WHERE 
                Id = @subjectId";
        private static string DeleteSubjectBelongsToPrerequisiteQuery =>
            @"
            DELETE FROM 
                subjectprerequisites 
            WHERE 
                PrerequisiteSubjectId = @subjectId";
        private static string GetSubjectIdQuery =>
            @"SELECT Id FROM subjects WHERE SubjectId = @subjectId AND DepartmentId = @departmentId";
        private static string GetAllSubjectInformationQuery =>
            @"
            SELECT 
                s.SubjectId AS subjectId, 
                s.Name AS subjectName, 
                s.CreditHour AS creditHour, 
                d.Name AS department, 
                s2.Name AS semester 
            FROM subjects s 
            JOIN 
                departments d ON d.Id = s.DepartmentId
            JOIN 
                semesters s2 ON s2.Id = s.SemesterId
            LIMIT @offset,@limit";
        private static string GetAllPrerequisiteSubjectListQuery =>
            @"
            SELECT 
                p.Name AS subjectName, 
                p.SubjectId AS prerequisiteId
            FROM 
                subjects s
            LEFT JOIN 
                subjectprerequisites sp ON sp.subjectId = s.Id
            LEFT JOIN 
                subjects p ON p.Id = sp.PrerequisiteSubjectId
            WHERE s.Id = @subjectId";
        private static string GetSubjectCountQuery =>
           @"
            SELECT 
                COUNT(Id)
            FROM
                subjects";

        private static string GetSubjectInfoQuery =>
            @"SELECT 
                s.SubjectId AS subjectId, 
                s.Name AS Name, 
                d.Name AS Department 
            FROM 
                subjects s
            JOIN 
                departments d ON s.DepartmentId = d.Id
            WHERE s.Id = @Id";

        private static string GetCreditHourQuery =>
            @"SELECT CreditHour FROM subjects WHERE SubjectId = @subjectId AND DepartmentId = @departmentId";

        #endregion

        #region methods

        public List<string> GetSemesters()
        {
            List<string> semesters = new List<string>();

            DataTable dt = _databaseAccess.GetData(GetSemestersQuery, null);

            foreach (DataRow row in dt.Rows)
            {
                string semester = row["Name"].ToString();
                semesters.Add(semester);
            }

            return semesters;
        }
        public List<string> GetDepartments()
        {
            List<string> departments = new List<string>();

            DataTable dt = _databaseAccess.GetData(GetDepartmentsQuery, null);

            foreach (DataRow row in dt.Rows)
            {
                string department = row["Name"].ToString();
                departments.Add(department);
            }

            return departments;
        }
        public int GetSemesterId(string semesterName)
        {
            var parameters = new Dictionary<string, object>()
            {
                {"semesterName", semesterName }
            };

            int semesterId = Convert.ToInt32(_databaseAccess.GetSingleData(GetSemesterIdQuery, parameters));
            return semesterId;
        }
        public int GetDepartmentId(string departmentName)
        {
            var parameters = new Dictionary<string, object>()
            {
                {"departmentName", departmentName }
            };

            int departmentId = Convert.ToInt32(_databaseAccess.GetSingleData(GetDepartmentIdQuery, parameters));
            return departmentId;
        }
        public int GetSubjectId(int subjectId, string department)
        {
            var parameters = new Dictionary<string, object>()
            {
                {"subjectId", subjectId},
                {"departmentId", GetDepartmentId(department) }
            };

            int id = Convert.ToInt32(_databaseAccess.GetSingleData(GetSubjectIdQuery, parameters));
            return id;

        }
        public List<string> GetSemesterWiseSubjectPrerequisites(string semesterName, string departmentName)
        {
            List<string> prerequisites = new List<string>();

            var parameters = new Dictionary<string, object>()
            {
                { "semesterId", GetSemesterId(semesterName) },
                { "departmentId", GetDepartmentId(departmentName) }
            };

            DataTable dataTable = _databaseAccess.GetData(GetSemesterWiseSubjectPrerequisitesQuery, parameters);

            foreach (DataRow row in dataTable.Rows)
            {
                string subjectId = row["SubjectId"].ToString();
                string subjectName = row["Name"].ToString();

                prerequisites.Add($"{departmentName}-{subjectId} : {subjectName}");
            }

            return prerequisites;
        }

        public List<string> GetSemesterWiseSubjects(string semesterName, string departmentName)
        {
            List<string> subjects = new List<string>();

            var parameters = new Dictionary<string, object>()
            {
                { "semesterId", GetSemesterId(semesterName) },
                { "departmentId", GetDepartmentId(departmentName) }
            };

            DataTable dataTable = _databaseAccess.GetData(GetSemesterWiseSubjectsQuery, parameters);

            foreach (DataRow row in dataTable.Rows)
            {
                string subjectId = row["SubjectId"].ToString();
                string subjectName = row["Name"].ToString();

                subjects.Add($"{departmentName}-{subjectId} : {subjectName}");
            }

            return subjects;
        }
        public bool IsSubjectBelongsToDepartment(string subjectName, string departmentName)
        {
            var parameters = new Dictionary<string, object>()
            {
                {"subjectName", subjectName },
                {"departmentId", GetDepartmentId(departmentName)}
            };

            string count = _databaseAccess.GetSingleData(IsSubjectBelongsToDepartmentQuery, parameters);

            if (count == null || count == "0")
            {
                return false;
            }

            return true;
        }
        public bool IsSameSubjectExists(int subjectId, string department)
        {
            var parameters = new Dictionary<string, object>()
            {
                {"subjectId", subjectId },
                {"departmentId", GetDepartmentId(department) }
            };

            string count = _databaseAccess.GetSingleData(IsSubjectCodeSameQuery, parameters);

            if (count == null || count == "0")
            {
                return false;
            }

            return true;
        }
        public bool AddSubject(Subject subject, User user, List<int> prerequisiteSubjectIdList)
        {
            var parameters = new Dictionary<string, object>()
            {
                {"SubjectId", subject.Id },
                {"Name", subject.Name},
                {"CreditHour", subject.CreditHour },
                {"DepartmentId", GetDepartmentId(subject.Department) },
                {"SemesterId", GetSemesterId(subject.Semester) },
                {"CreatedBy", user.UserName},
                {"CreatedDate",  DateTime.Now.ToString("yyyy-MM-dd")},
                {"UpdatedBy", user.UserName},
                {"UpdatedDate", DateTime.Now.ToString("yyyy-MM-dd") }
            };

            try
            {
                _databaseAccess.ExecuteQuery(AddSubjectQuery, parameters);

                int id = GetSubjectId(subject.Id, subject.Department);

                foreach (var prerequisiteId in prerequisiteSubjectIdList)
                {
                    bool success = AddPrerequisiteSubjects(id, prerequisiteId);

                    if (!success)
                    {
                        return false;
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
        }
        public bool AddPrerequisiteSubjects(int subjectId, int prerequisiteId)
        {
            var parameters = new Dictionary<string, object>()
            {
                { "subjectId", subjectId },
                { "prerequisiteId", prerequisiteId },
            };

            try
            {
                _databaseAccess.ExecuteQuery(AddPrerequisiteSubjectsQuery, parameters);
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
        }
        public bool IsSubjectBelongsToPrerequisites(int subjectId)
        {
            var parameters = new Dictionary<string, object>()
            {
                {"subjectId", subjectId }
            };

            string count = _databaseAccess.GetSingleData(IsSubjectBelongsToPrerequisiteQuery, parameters);

            if (count != "0")
            {
                return true;
            }

            return false;
        }
        public bool DeleteSubject(int subjectId)
        {
            var parameters = new Dictionary<string, object>()
            {
                { "subjectId", subjectId },
            };

            try
            {
                _databaseAccess.ExecuteQuery(DeletePrerequisiteSubjectsQuery, parameters);
                _databaseAccess.ExecuteQuery(DeleteSubjectQuery, parameters);

                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
        }
        public bool DeleteSubjectBelongsToPrerequisites(int subjectId)
        {
            var parameters = new Dictionary<string, object>()
            {
                {"subjectId", subjectId }
            };

            try
            {
                _databaseAccess.ExecuteQuery(DeleteSubjectBelongsToPrerequisiteQuery, parameters);
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
        }
        public bool UpdateSubject(Subject subject, User user, List<int> prerequisiteSubjectIdList)
        {
            var parameters = new Dictionary<string, object>()
            {
                {"subjectId", subject.Id },
                {"Name", subject.Name},
                {"CreditHour", subject.CreditHour },
                {"DepartmentId", GetDepartmentId(subject.Department) },
                {"SemesterId", GetSemesterId(subject.Semester) },
                {"UpdatedBy", user.UserName},
                {"UpdatedDate", DateTime.Now.ToString("yyyy-MM-dd") },
            };

            try
            {
                _databaseAccess.ExecuteQuery(UpdateSubjectQuery, parameters);

                int subjectId = GetSubjectId(subject.Id, subject.Department);

                parameters = new Dictionary<string, object>()
                {
                    { "subjectId", subjectId }
                };

                _databaseAccess.ExecuteQuery(DeletePrerequisiteSubjectsQuery, parameters);

                foreach (var prerequisiteId in prerequisiteSubjectIdList)
                {
                    bool success = AddPrerequisiteSubjects(subjectId, prerequisiteId);

                    if (!success)
                    {
                        return false;
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
        }
        public List<Subject> GetAllSubjectInformation(int limit, int offset)
        {
            List<Subject> subjectInfoList = new List<Subject>();
            var parameters = new Dictionary<string, object>
            {
                {"limit" , limit},
                {"offset", offset }
            };

            DataTable dataTable = _databaseAccess.GetData(GetAllSubjectInformationQuery, parameters);

            foreach (DataRow row in dataTable.Rows)
            {
                Subject subject = new Subject();
                subject.Id = Convert.ToInt32(row["subjectId"].ToString());
                subject.Name = row["subjectName"].ToString();
                subject.CreditHour = Convert.ToDouble(row["creditHour"]).ToString("F2");
                subject.Semester = row["semester"].ToString();
                subject.Department = row["department"].ToString();
                subject.Code = $"{subject.Department}-{subject.Id}";

                int id = GetSubjectId(subject.Id, subject.Department);

                List<string> prerequisiteSubjectList = GetAllPrerequisiteSubjectList(id, subject.Department);

                string commaSeparatedSubjects = "";

                for (int i = 0; i < prerequisiteSubjectList.Count; i++)
                {
                    commaSeparatedSubjects += prerequisiteSubjectList[i];
                    if (i < prerequisiteSubjectList.Count - 1)
                    {
                        commaSeparatedSubjects += ", ";
                    }
                }

                subject.PrerequisiteSubjectList = commaSeparatedSubjects;

                subjectInfoList.Add(subject);

            }

            return subjectInfoList;
        }
        public List<string> GetAllPrerequisiteSubjectList(int subjectId, string department)
        {
            List<string> prerequisiteSubjectList = new List<string>();
            var parameters = new Dictionary<string, object>
            {
                {"subjectId" , subjectId}
            };
            DataTable dataTable = _databaseAccess.GetData(GetAllPrerequisiteSubjectListQuery, parameters);

            foreach (DataRow row in dataTable.Rows)
            {
                if (row["prerequisiteId"] == DBNull.Value)
                {
                    prerequisiteSubjectList.Add("No Subject");
                }
                else
                {
                    string subjectName = row["subjectName"].ToString();
                    string id = row["prerequisiteId"].ToString();

                    prerequisiteSubjectList.Add($"{department}-{id} : {subjectName}");
                }
            }

            return prerequisiteSubjectList;
        }
        public int GetSubjectCount()
        {
            int count = Convert.ToInt32(_databaseAccess.GetSingleData(GetSubjectCountQuery, null));
            return count;
        }

        public List<Subject> GetSubjectInfo(int subjectId)
        {
            List<Subject> subjects = new List<Subject>();
            var parameters = new Dictionary<string, object>()
            {
                {"Id", subjectId }
            };

            DataTable dataTable = _databaseAccess.GetData(GetSubjectInfoQuery, parameters);

            foreach (DataRow row in dataTable.Rows)
            {
                Subject subject = new Subject();

                subject.Id = Convert.ToInt32(row["SubjectId"].ToString());
                subject.Name = row["Name"].ToString();
                subject.Department = row["Department"].ToString();
                subject.Code = $"{subject.Department}-{subject.Id}";
                subjects.Add(subject);
            }

            return subjects;
        }

        public string GetCreditHour(String subjectCode)
        {
            string[] parts = subjectCode.Split('-');
            int subjectId = Convert.ToInt32(parts[1]);
            string department = parts[0];

            var parameters = new Dictionary<string, object>()
            {
                {"subjectId", subjectId },
                {"DepartmentId", GetDepartmentId(department)},
            };

            string creditHour = _databaseAccess.GetSingleData(GetCreditHourQuery, parameters);
            return creditHour;
        }

        #endregion
    }
}
