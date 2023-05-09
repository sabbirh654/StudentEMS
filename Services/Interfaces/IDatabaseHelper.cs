using StudentEMS.Models;
using System.Collections.Generic;

namespace StudentEMS
{
    public interface IDatabaseHelper
    {
        List<User> GetStaffsInfo(int limit, int offset, int userRoleId, string filterText);
        string GetUserId(string username);
        string GetUserPreferenceKeyId(string currentPage);
        void InsertUserPreferences(int userId, int keyId, string value);
        bool IsUserPreferenceExists(int userId, int keyId);
        void UpdateUserPreferences(int userId, int keyId, string value);
        string GetUserPreferenceValue(int userId, int keyId);
        bool IsUserNameExist(string username);
        bool IsEmailExist(string email);
        bool AddStudent(Student student, string userName);
        string GetRoleId(string roleName);
        string GetUserStaffId(string email);
        string GetDepartmentId(string departmentName);
        List<string> GetDepartments();
        User GetStaffData(string userName);
        Student GetStudentData(string userName);
        bool UpdateStaffData(User staff);
        bool AddStaff(User staff);
        bool UpdateStudentData(Student student, string userId);
        bool IsAnotherUserExistWithSameEmail(string email);
        string GetUserIdWithEmail(string email);
        int GetStaffCount(int userRoleId, string filterText);
        List<Department> GetDepartmentsInfo();
        List<Semester> GetSemestersInfo();
    }
}
