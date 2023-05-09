using StudentEMS.Models;

using System;
using System.Collections.Generic;

namespace StudentEMS.Services.Interfaces
{
    public interface ISubjectHelper
    {
        List<string> GetSemesters();
        List<string> GetDepartments();
        List<string> GetSemesterWiseSubjectPrerequisites(string semesterName, string departmentName);
        bool IsSubjectBelongsToDepartment(string subjectName, string departmentName);
        bool IsSameSubjectExists(int subjectCode, string department);
        bool AddSubject(Subject subject, User user, List<int> prerequisiteSubjecCodes);
        bool IsSubjectBelongsToPrerequisites(int subjectId);
        bool DeleteSubject(int subjectId);
        bool DeleteSubjectBelongsToPrerequisites(int subjectId);
        bool UpdateSubject(Subject subject, User user, List<int> prerequisiteSubjectIdList);
        int GetSubjectId(int subjectId, string department);
        List<string> GetAllPrerequisiteSubjectList(int subjectId, string department);
        List<Subject> GetAllSubjectInformation(int limit, int offset);
        int GetSubjectCount();
        List<Subject> GetSubjectInfo(int subjectId);
        List<string> GetSemesterWiseSubjects(string semesterName, string departmentName);
        int GetSemesterId(string semesterName);
        string GetCreditHour(String subjectCode);
    }
}
