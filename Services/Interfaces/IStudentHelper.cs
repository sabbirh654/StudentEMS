using System.Collections.Generic;

namespace StudentEMS.Services.Interfaces
{
    public interface IStudentHelper
    {
        List<Student> GetAllStudentInfo(int limit, int offset, string filterText);
        int GetStudentCount(string filterText);
        Student GetSingleStudentInfo(string email);
        int GetUserIdOfStudent(int studentId);
        bool DeleteStudent(int studentId);
        bool UpdateStudent(Student student);
        string GetStudentCGPA(int studentId);
        string GetLastEnrolledSemester(int studentId);
        string GetTotalEarnedCredits(int studentId);
        string GetStudentImagePath(string email);
    }
}
