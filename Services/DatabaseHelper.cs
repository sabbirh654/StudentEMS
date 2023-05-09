using Microsoft.Extensions.DependencyInjection;

using StudentEMS.Models;

using System;
using System.Collections.Generic;
using System.Data;
using System.Windows;

namespace StudentEMS
{
    public class DatabaseHelper : IDatabaseHelper
    {
        private IDatabaseAccess _databaseAccess = App.ServiceProvider.GetService<IDatabaseAccess>();
        private static string GetUserCountQuery =>
            @"
            SELECT
                COUNT(Id)
            FROM users
            WHERE UserName = @userName
            ";
        private static string GetUserWithSameEmailQuery =>
            @"
            SELECT
                COUNT(Id)
            FROM users
            WHERE Email = @email
            AND Id <> @userId
            ";
        private static string GetUserCountWithEmail =>
             @"
            SELECT
                COUNT(Id)
            FROM users
            WHERE Email = @email
            ";
        public static string GetDepartmentsQuery =>
            @"
            SELECT 
                Name
            FROM departments
            ";
        public static string GetRoleIdQuery =>
            @"
            SELECT
                Id
            FROM userroles
            WHERE Name = @roleName
            ";
        public static string GetDepartmentIdQuery =>
            @"
            SELECT
                Id
            FROM departments
            WHERE Name = @departmentName
            ";
        public static string GetUserIdWithEmailQuery =>
            @"
            SELECT
                Id
            FROM users
            WHERE Email = @email
            ";
        public static string AddUserQuery =>
            @"
            INSERT INTO users(
                FirstName,
                LastName,
                Email,
                ContactNumber,
                UserName,
                DOB,
                RoleId,
                ImagePath,
                CreatedBy,
                CreatedDate,
                UpdatedBy,
                UpdatedDate,
                ExpiryTime
            )VALUES(
                @firstName,
                @lastName,
                @email,
                @contactNumber,
                @userName,
                @dob,
                @roleId,
                @imagePath,
                @createdBy,
                @createdDate,
                @updatedBy,
                @updatedDate,
                @expiryTime
            )";
        public static string AddStudentQuery =>
            @"
            INSERT INTO students(
                Session,
                UserId,
                DepartmentId,
                Password
            )VALUES(
                @session,
                (   
                    SELECT
                        Id
                    FROM users
                    WHERE UserName = @userName
                ),
                (   
                    SELECT
                        Id
                    FROM departments
                    WHERE Name = @departmentName
                ),
                @password
            )";
        public static string GetStaffDataQuery =>
            @"
            SELECT
                Id,
                FirstName,
                LastName,
                DOB,
                Email,
                ContactNumber,
                ImagePath
            FROM users
            WHERE UserName = @userName
            ";
        public static string GetStudentDataQuery =>
            @"
            SELECT
                students.Id,
                users.FirstName,
                users.LastName,
                users.ImagePath,
                IFNULL(users.ContactNumber,'N/A') as ContactNumber,
                users.DOB,   
                students.Session,
                students.Password,
                departments.Name as DepartmentName
            FROM users
            INNER JOIN students
                ON users.Id = students.UserId
            INNER JOIN departments
                ON students.DepartmentId = departments.Id
            WHERE
                users.UserName = @userName
        ";
        private static string GetStaffsInfoQuery =>
            @"
            SELECT 
                FirstName, 
                LastName, 
                Email, 
                ContactNumber 
            FROM 
                users u 
            JOIN 
                userroles u1 ON u.RoleId = u1.Id
            WHERE 
                u.RoleId = @userRoleId
                AND (
                    @searchText = '' OR 
                    (
                        FirstName LIKE @searchText OR 
                        LastName LIKE @searchText OR 
                        Email LIKE @searchText OR 
                        ContactNumber LIKE @searchText
                    )
                )
            LIMIT @offset, @limit";
        private static string GetStaffCountQuery =>
            @"
            SELECT 
                COUNT(u.Id)
            FROM 
                users u
            JOIN 
                userroles u1 ON u.RoleId = u1.Id
            WHERE 
                u.RoleId = @userRoleId
                AND (
                    @searchText = '' OR 
                    (
                        FirstName LIKE @searchText OR 
                        LastName LIKE @searchText OR 
                        Email LIKE @searchText OR 
                        ContactNumber LIKE @searchText
                    )
                 )";
        private static string GetUserIdQuery =>
            @"
            SELECT 
                Id 
            FROM 
                users 
            WHERE 
                UserName = @userName";
        private static string GetPreferenceKeyIdQuery =>
            @"
            SELECT 
                Id 
            FROM 
                userpreferencekeys 
            WHERE 
                KeyName = @currentFormPage";
        private static string InsertUserPreferenceQuery =>
            @"
            INSERT INTO 
                userpreferences (UserId, KeyId, Value) 
            VALUES 
                (@userId, @keyId, @value)";
        private static string IsUserPreferenceExistQuery =>
            @"
            SELECT EXISTS
            (SELECT 1 
            FROM 
                userpreferences 
            WHERE 
                KeyId = @keyId AND 
                UserId = @userId
            )";
        private static string UpdateUserPreferencesQuery =>
            @"
            UPDATE 
                userpreferences 
            SET 
                Value = @value 
            WHERE 
                UserId = @userId AND 
                KeyId = @keyId";
        private static string GetPreferenceStringQuery =>
            @"
            SELECT 
                Value 
            FROM 
                userpreferences 
            WHERE 
                UserId = @userId AND 
                KeyId = @keyId";
        public static string UpdateUserDataQuery =>
            @"
            UPDATE users
            SET 
                FirstName = @firstName,
                LastName = @lastName,
                Email = @email,
                ContactNumber = @contactNumber,
                UserName = @userName,
                DOB = @dob,
                UpdatedBy = @updatedBy,
                UpdatedDate = @updatedDate,
                ExpiryTime = @expiryTime,
                ImagePath = @imagePath
            WHERE
                Id = @userId
            ";
        public static string UpdateStudentPasswordQuery =>
            @"
            UPDATE students
            SET 
                Password = @password
            WHERE
                UserId = @userId
            ";
        private static string GetDepartmentsInfoQuery =>
            @"
            SELECT 
                Id, 
                Name 
            FROM 
                departments";
        private static string GetSemestersInfoQuery =>
            @"
            SELECT 
                Id, 
                Name 
            FROM 
                semesters";

        public bool IsUserNameExist(string userName)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("userName", userName);

            string queryResult = _databaseAccess.GetSingleData(GetUserCountQuery, parameters);
            if (queryResult != null
                && int.TryParse(queryResult, out int count)
                && count > 0)
            {
                return true;
            }
            return false;
        }
        public string GetUserIdWithEmail(string email)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("email", email);

            return _databaseAccess.GetSingleData(GetUserIdWithEmailQuery, parameters);
        }
        public bool IsEmailExist(string email)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("email", email);

            string queryResult = _databaseAccess.GetSingleData(GetUserCountWithEmail, parameters);
            if (queryResult != null
                && int.TryParse(queryResult, out int count)
                && count > 0)
            {
                return true;
            }
            return false;
        }
        public bool IsAnotherUserExistWithSameEmail(string email)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("email", email);
            parameters.Add("userId", GetUserIdWithEmail(email));

            string queryResult = _databaseAccess.GetSingleData(GetUserWithSameEmailQuery, parameters);
            if (queryResult != null
                && int.TryParse(queryResult, out int count)
                && count > 0)
            {
                return true;
            }
            return false;
        }
        public bool AddStudent(Student student, string userName)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();

            parameters.Add("firstName", student.FirstName);
            parameters.Add("lastName", student.LastName);
            parameters.Add("email", student.Email);
            parameters.Add("contactNumber", null);
            parameters.Add("userName", student.Email);
            parameters.Add("dob", student.DateOfBirth.ToString("yyyy-MM-dd"));
            parameters.Add("roleId", GetRoleId("Student"));
            parameters.Add("imagePath", student.ProfilePicturePath);
            parameters.Add("createdBy", userName);
            parameters.Add("createdDate", DateTime.Now.ToString("yyyy-MM-dd"));
            parameters.Add("updatedBy", userName);
            parameters.Add("updatedDate", DateTime.Now.ToString("yyyy-MM-dd"));
            DateTime dateTime = DateTime.Now.AddMinutes(10);
            parameters.Add("expiryTime", dateTime.ToString("hh:mm:ss"));

            try
            {
                _databaseAccess.ExecuteQuery(AddUserQuery, parameters);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                return false;
            }
            parameters.Clear();

            parameters.Add("session", student.Session);
            parameters.Add("userName", student.Email);
            parameters.Add("departmentName", student.Department);
            parameters.Add("password", student.Password);

            try
            {
                _databaseAccess.ExecuteQuery(AddStudentQuery, parameters);
            }

            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                return false;
            }
            return true;
        }
        public bool AddStaff(User staff)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("firstName", staff.FirstName);
            parameters.Add("lastName", staff.LastName);
            parameters.Add("email", staff.Email);
            parameters.Add("contactNumber", staff.ContactNumber);
            parameters.Add("userName", staff.UserName);
            parameters.Add("roleId", GetRoleId("Staff"));
            parameters.Add("imagePath", staff.ProfilePicturePath);
            parameters.Add("createdBy", staff.UserName);
            parameters.Add("createdDate", DateTime.Now.ToString("yyyy-MM-dd"));
            parameters.Add("updatedBy", staff.UserName);
            parameters.Add("updatedDate", DateTime.Now.ToString("yyyy-MM-dd"));
            DateTime dateTime = DateTime.Now.AddMinutes(10);
            parameters.Add("expiryTime", dateTime.ToString("hh:mm:ss"));
            parameters.Add("dob", staff.DateOfBirth.ToString("yyyy-MM-dd"));

            try
            {
                _databaseAccess.ExecuteQuery(AddUserQuery, parameters);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                return false;
            }
            return true;
        }
        public string GetRoleId(string roleName)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("roleName", roleName);
            return _databaseAccess.GetSingleData(GetRoleIdQuery, parameters);
        }
        public string GetUserId(string userName)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("userName", userName);

            return _databaseAccess.GetSingleData(GetUserIdQuery, parameters);
        }
        public string GetDepartmentId(string departmentName)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("departmentName", departmentName);
            return _databaseAccess.GetSingleData(GetDepartmentIdQuery, parameters);
        }
        public List<string> GetDepartments()
        {
            DataTable dataTable = _databaseAccess.GetData(GetDepartmentsQuery);
            List<string> departments = new List<string>();

            foreach (DataRow row in dataTable.Rows)
            {
                departments.Add(row["Name"].ToString());
            }
            return departments;
        }
        public User GetStaffData(string userName)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("userName", userName);

            DataTable dataTable = _databaseAccess.GetData(GetStaffDataQuery, parameters);
            Student student = new Student();
            DataRow row = dataTable.Rows[0];

            User user = new User();

            user.UserId = Convert.ToInt32(row["Id"]);
            user.FirstName = row["FirstName"].ToString();
            user.LastName = row["LastName"].ToString();
            user.DateOfBirth = DateTime.Parse(row["DOB"].ToString());
            user.Email = row["Email"].ToString();
            user.ContactNumber = row["ContactNumber"].ToString();
            user.UserName = userName;
            user.ProfilePicturePath = row["ImagePath"].ToString();

            return user;
        }
        public bool UpdateStaffData(User staff)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();

            parameters.Add("firstName", staff.FirstName);
            parameters.Add("lastName", staff.LastName);
            parameters.Add("email", staff.Email);
            parameters.Add("contactNumber", staff.ContactNumber);
            parameters.Add("userName", staff.UserName);
            parameters.Add("dob", staff.DateOfBirth);
            parameters.Add("imagePath", staff.ProfilePicturePath);
            parameters.Add("updatedBy", staff.UserName);
            parameters.Add("updatedDate", DateTime.Now.ToString("yyyy-MM-dd"));
            DateTime dateTime = DateTime.Now.AddMinutes(10);
            parameters.Add("expiryTime", dateTime.ToString("hh:mm:ss"));
            parameters.Add("userId", GetUserId(staff.UserName));

            try
            {
                _databaseAccess.ExecuteQuery(UpdateUserDataQuery, parameters);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                return false;
            }
            return true;
        }
        public Student GetStudentData(string userName)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("userName", userName);

            DataTable dataTable = _databaseAccess.GetData(GetStudentDataQuery, parameters);

            Student student = new Student();
            DataRow row = dataTable.Rows[0];

            string studentId = row["Id"].ToString();
            int.TryParse(studentId, out int Id);

            student.StudentId = Id;
            student.FirstName = row["FirstName"].ToString();
            student.LastName = row["LastName"].ToString();
            student.ContactNumber = row["ContactNumber"].ToString();
            student.DateOfBirth = DateTime.Parse(row["DOB"].ToString());
            student.Session = row["Session"].ToString();
            student.Department = row["DepartmentName"].ToString();
            student.Password = row["Password"].ToString();
            student.ProfilePicturePath = row["ImagePath"].ToString();
            student.Email = userName;
            student.UserName = userName;

            return student;
        }

        #region Staff

        public List<User> GetStaffsInfo(int limit, int offset, int userRoleId, string filterText)
        {
            List<User> staffList = new List<User>();

            var parameters = new Dictionary<string, object>
            {

                {"limit", limit },
                {"offset", offset },
                {"userRoleId", userRoleId },
                { "searchText", "%" + filterText + "%"}
            };

            DataTable dataTable = _databaseAccess.GetData(GetStaffsInfoQuery, parameters);

            foreach (DataRow row in dataTable.Rows)
            {
                User user = new User();
                user.FirstName = row["FirstName"].ToString();
                user.LastName = row["LastName"].ToString();
                user.Email = row["Email"].ToString();
                user.ContactNumber = row["ContactNumber"].ToString();

                staffList.Add(user);
            }

            return staffList;
        }
        public int GetStaffCount(int userRoleId, string filterText)
        {
            var parameters = new Dictionary<string, object>
            {
                { "userRoleId", userRoleId },
                { "searchText", "%" + filterText + "%"},
            };

            int count = Convert.ToInt32(_databaseAccess.GetSingleData(GetStaffCountQuery, parameters));

            return count;
        }
        public string GetUserStaffId(string userName)
        {
            var parameters = new Dictionary<string, object>
            {
                {"userName", userName }
            };

            string userId = _databaseAccess.GetSingleData(GetUserIdQuery, parameters);
            return userId;
        }

        #endregion

        public string GetUserPreferenceKeyId(string currentPage)
        {
            var parameters = new Dictionary<string, object>
            {
                {"currentFormPage", currentPage }
            };

            string userPreferenceKeyId = _databaseAccess.GetSingleData(GetPreferenceKeyIdQuery, parameters);
            return userPreferenceKeyId;
        }
        public void InsertUserPreferences(int userId, int keyId, string value)
        {
            var parameters = new Dictionary<string, object>
            {
                {"userId", userId },
                {"keyId", keyId  },
                {"value", value }
            };

            try
            {
                _databaseAccess.ExecuteQuery(InsertUserPreferenceQuery, parameters);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        public void UpdateUserPreferences(int userId, int keyId, string value)
        {
            var parameters = new Dictionary<string, object>
            {
                {"userId", userId },
                {"keyId", keyId  },
                {"value", value }
            };

            try
            {
                _databaseAccess.ExecuteQuery(UpdateUserPreferencesQuery, parameters);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        public bool IsUserPreferenceExists(int userId, int keyId)
        {
            var parameters = new Dictionary<string, object>
            {
                {"userId", userId },
                {"keyId", keyId  },
            };

            string queryResult = _databaseAccess.GetSingleData(IsUserPreferenceExistQuery, parameters);

            if (queryResult == "1")
            {
                return true;
            }

            return false;
        }
        public string GetUserPreferenceValue(int userId, int keyId)
        {
            var parameters = new Dictionary<string, object>
            {
                {"userId", userId },
                {"keyId", keyId  },
            };

            string value = _databaseAccess.GetSingleData(GetPreferenceStringQuery, parameters);
            return value;
        }
        public bool UpdateStudentData(Student student, string userId)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();

            parameters.Add("firstName", student.FirstName);
            parameters.Add("lastName", student.LastName);
            parameters.Add("email", student.Email);
            parameters.Add("contactNumber", student.ContactNumber);
            parameters.Add("userName", student.Email);
            parameters.Add("updatedBy", student.Email);
            parameters.Add("updatedDate", DateTime.Now.ToString("yyyy-MM-dd"));
            DateTime dateTime = DateTime.Now.AddMinutes(10);
            parameters.Add("expiryTime", dateTime.ToString("hh:mm:ss"));
            parameters.Add("dob", student.DateOfBirth.ToString("yyyy-MM-dd"));
            parameters.Add("userId", userId);
            parameters.Add("imagePath", student.ProfilePicturePath);
            try
            {
                _databaseAccess.ExecuteQuery(UpdateUserDataQuery, parameters);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                return false;
            }

            parameters.Clear();
            parameters.Add("password", student.Password);
            parameters.Add("userId", userId);
            try
            {
                _databaseAccess.ExecuteQuery(UpdateStudentPasswordQuery, parameters);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                return false;
            }
            return true;
        }
        public List<Department> GetDepartmentsInfo()
        {
            List<Department> departments = new List<Department>();
            DataTable dt = _databaseAccess.GetData(GetDepartmentsInfoQuery, null);

            foreach (DataRow row in dt.Rows)
            {
                Department department = new Department();
                department.Id = Convert.ToInt32(row["Id"]);
                department.Name = Convert.ToString(row["Name"]);
                departments.Add(department);
            }

            return departments;
        }
        public List<Semester> GetSemestersInfo()
        {
            List<Semester> semesters = new List<Semester>();
            DataTable dt = _databaseAccess.GetData(GetSemestersInfoQuery, null);

            foreach (DataRow row in dt.Rows)
            {
                Semester semester = new Semester();
                semester.Id = Convert.ToInt32(row["Id"]);
                semester.Name = Convert.ToString(row["Name"]);
                semesters.Add(semester);
            }

            return semesters;
        }
    }
}

