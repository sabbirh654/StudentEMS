using StudentEMS.Models;

using System.Collections.Generic;

namespace StudentEMS.Services.Interfaces
{
    public interface ICourseHelper
    {
        bool AddCourse(Student student, Course course);
        bool IsSameCourseExists(Student student, Course course);
        List<Course> GetCourseDetailsForAStudent(int limit, int offset, Student student);
        int CountCourseForAStudent(Student student, string semester);
        bool UpdateCourse(Student student, Course course);
        bool DeleteCourse(Student student, Course course);

        bool DeleteCoursesWhenASubjectDeleted(int subjectId);
        bool DeleteCoursesWhenAStudentDeleted(int studentId);
    }
}
