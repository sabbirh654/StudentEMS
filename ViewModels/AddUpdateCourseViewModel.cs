using Microsoft.Extensions.DependencyInjection;

using StudentEMS.Command;
using StudentEMS.Constants;
using StudentEMS.Models;
using StudentEMS.Services.Interfaces;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace StudentEMS.ViewModels
{
    public class AddUpdateCourseViewModel : BaseViewModel
    {

        private SubjectDetailsViewModel courseViewModel;
        private ISubjectHelper _subjectHelper;
        private bool isUpdateOperation;
        public Action CloseAction { get; set; }
        public SubjectDetailsViewModel CourseViewModel
        {
            get { return courseViewModel; }
            set { courseViewModel = value; }
        }

        private List<string> semesterList;
        public List<string> SemesterList
        {
            get { return semesterList; }
            set { semesterList = value; OnPropertyChanged(nameof(SemesterList)); }
        }

        private string selectedSemester;
        public string SelectedSemester
        {
            get { return selectedSemester; }
            set
            {
                selectedSemester = value;
                OnPropertyChanged(nameof(SelectedSemester));
                SelectedSemesterCommand.Execute(null);
            }
        }

        private List<string> gradeList;
        public List<string> GradeList
        {
            get { return gradeList; }
            set { gradeList = value; OnPropertyChanged(nameof(GradeList)); }
        }

        private string selectedGrade;

        public string SelectedGrade
        {
            get { return selectedGrade; }
            set { selectedGrade = value; OnPropertyChanged(nameof(SelectedGrade)); }
        }

        private List<string> subjectList;

        public List<string> SubjectList
        {
            get { return subjectList; }
            set { subjectList = value; OnPropertyChanged(nameof(SubjectList)); }
        }

        private string selectedSubject;

        public string SelectedSubject
        {
            get { return selectedSubject; }
            set { selectedSubject = value; OnPropertyChanged(nameof(SelectedSubject)); }
        }

        private bool _isComboBoxEnabled;
        public bool IsComboBoxEnabled
        {
            get { return _isComboBoxEnabled; }
            set
            {
                _isComboBoxEnabled = value;
                OnPropertyChanged(nameof(IsComboBoxEnabled));
            }
        }

        public ICommand OkCommand { get; set; }
        public ICommand CancelCommand { get; set; }
        public ICommand SelectedSemesterCommand { get; set; }


        public AddUpdateCourseViewModel(SubjectDetailsViewModel courseViewModel, bool isUpdateOperation)
        {
            this.CourseViewModel = courseViewModel;
            _subjectHelper = App.ServiceProvider.GetService<ISubjectHelper>();
            IsComboBoxEnabled = true;
            this.isUpdateOperation = isUpdateOperation;

            OkCommand = new RelayCommand(SaveData, CanSaveData);
            CancelCommand = new RelayCommand(CancelOperation, CanCancelOperation);
            SelectedSemesterCommand = new RelayCommand(OnSemesterSelected, CanExecutedSemesterSelection);

            InitilizeSemesterComboBox();
            InitializeGradeComboBox();

            if (isUpdateOperation)
            {
                IsComboBoxEnabled = false;
                SelectedSubject = $"{CourseViewModel.SelectedCourse.SubjectCode} : {CourseViewModel.SelectedCourse.SubjectName}";
                SelectedGrade = CourseViewModel.SelectedCourse.Grade;
            }
        }

        private void InitializeGradeComboBox()
        {
            GradeList = Constant.GradeList.ToList();
            SelectedGrade = GradeList[0];
        }

        private void InitilizeSemesterComboBox()
        {
            SemesterList = _subjectHelper.GetSemesters();
            SelectedSemester = CourseViewModel.SelectedSemester;
        }

        private bool CanExecutedSemesterSelection(object obj)
        {
            return true;
        }

        private void OnSemesterSelected(object obj)
        {
            InitializeSubjectComboBox();
        }

        private void InitializeSubjectComboBox()
        {

            List<string> addableSubjects = CourseViewModel.AddableSubjectListForACourse;

            List<string> subjects = _subjectHelper.GetSemesterWiseSubjects(SelectedSemester, CourseViewModel.SelectedStudent.Department);

            if (isUpdateOperation)
            {
                SubjectList = subjects;
                return;
            }

            SubjectList = addableSubjects;
            SelectedSubject = SubjectList[0];
        }

        private bool CanCancelOperation(object obj)
        {
            return true;
        }

        private void CancelOperation(object obj)
        {
            CloseAction?.Invoke();
        }

        private bool CanSaveData(object obj)
        {
            return true;
        }

        private void SaveData(object obj)
        {
            if (!IsPrerequisiteCourseAdded())
            {
                MessageBox.Show("Prerequisite Courses are not added. Please add those courses before!");
                CloseAction?.Invoke();
                return;
            }

            Course course = new Course();

            string[] parts = SelectedSubject.Split(':');

            course.SubjectCode = parts[0].Trim();
            course.SubjectName = parts[1].Trim();
            course.Semester = SelectedSemester;
            course.CreditHour = Convert.ToDouble(_subjectHelper.GetCreditHour(course.SubjectCode)).ToString("F2");
            course.Grade = Convert.ToDouble(SelectedGrade).ToString("F2");

            int existingCourseIndex = CourseViewModel.AddedCourseList.FindIndex(c => c.SubjectCode == course.SubjectCode && c.SubjectName == course.SubjectName);

            if (existingCourseIndex >= 0)
            {
                if (CourseViewModel.AddedCourseList[existingCourseIndex].DbStatus == Constant.Operation.Delete)
                {
                    CourseViewModel.AddedCourseList[existingCourseIndex].Grade = Convert.ToDouble(SelectedGrade).ToString("F2");
                    CourseViewModel.AddedCourseList[existingCourseIndex].DbStatus = Constant.Operation.Add;
                }
                else
                {
                    CourseViewModel.AddedCourseList[existingCourseIndex].Grade = Convert.ToDouble(SelectedGrade).ToString("F2");
                    CourseViewModel.AddedCourseList[existingCourseIndex].DbStatus = Constant.Operation.Update;
                }
            }
            else
            {
                course.DbStatus = Constant.Operation.Add;
                course.IsExistsInDatabase = false;
                CourseViewModel.AddedCourseList.Add(course);
            }

            CloseAction?.Invoke();
        }

        bool IsPrerequisiteCourseAdded()
        {
            string selectedSubject = SelectedSubject;
            string[] parts = selectedSubject.Split('-', ':');
            int subjectId = Convert.ToInt32(parts[1].Trim());
            string department = parts[0].Trim();

            int id = _subjectHelper.GetSubjectId(subjectId, department);

            List<string> prerequisiteSubjectList = _subjectHelper.GetAllPrerequisiteSubjectList(id, department);

            foreach (var subject in prerequisiteSubjectList)
            {
                if (subject == "No Subject")
                {
                    continue;
                }

                parts = subject.Split(':');
                string subjectCode = parts[0].Trim();
                string subjectName = parts[1].Trim();

                int existingCourseIndex = CourseViewModel.AddedCourseList.FindIndex(c => c.SubjectCode == subjectCode && c.SubjectName == subjectName && c.DbStatus != Constant.Operation.Delete);

                if (existingCourseIndex == -1)
                {
                    return false;
                }
            }

            return true;
        }
    }
}
