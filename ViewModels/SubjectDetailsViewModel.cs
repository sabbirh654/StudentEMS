using Microsoft.Extensions.DependencyInjection;

using StudentEMS.AppData;
using StudentEMS.Command;
using StudentEMS.Constants;
using StudentEMS.Models;
using StudentEMS.Services;
using StudentEMS.Services.Interfaces;
using StudentEMS.Views;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace StudentEMS.ViewModels
{
    public class SubjectDetailsViewModel : BaseViewModel
    {
        private Student selectedStudent;
        private ISubjectHelper _subjectHelper;
        private ICourseHelper _courseHelper;

        public Action CloseAction { get; set; }
        public Student SelectedStudent
        {
            get { return selectedStudent; }
            set { selectedStudent = value; OnPropertyChanged(nameof(SelectedStudent)); }
        }

        private int currentPage;
        public int CurrentPage
        {
            get { return currentPage; }
            set { currentPage = value; OnPropertyChanged(nameof(CurrentPage)); }
        }

        private List<int> pageSizeList;
        public List<int> PageSizeList
        {
            get { return pageSizeList; }
            set { pageSizeList = value; OnPropertyChanged(nameof(PageSizeList)); }
        }

        private int selectedPageSize;
        public int SelectedPageSize
        {
            get { return selectedPageSize; }
            set
            {
                selectedPageSize = value;
                OnPropertyChanged(nameof(SelectedPageSize));
                SelectedPageCommand.Execute(null);
            }
        }

        private int totalRows;
        public int TotalRows
        {
            get { return totalRows; }
            set { totalRows = value; OnPropertyChanged(nameof(TotalRows)); }
        }

        private string currentPageLabel;
        public string CurrentPageLabel
        {
            get { return currentPageLabel; }
            set { currentPageLabel = value; OnPropertyChanged(nameof(CurrentPageLabel)); }
        }

        private int totalPages;
        public int TotalPages
        {
            get { return totalPages; }
            set { totalPages = value; OnPropertyChanged(nameof(totalPages)); }
        }

        private List<Course> courseList;
        public List<Course> CourseList
        {
            get { return courseList; }
            set { courseList = value; OnPropertyChanged(nameof(CourseList)); }
        }

        private Course selectedCourse;
        public Course SelectedCourse
        {
            get { return selectedCourse; }
            set { selectedCourse = value; OnPropertyChanged(nameof(SelectedCourse)); }
        }

        private List<Course> addedCourseList;
        public List<Course> AddedCourseList
        {
            get { return addedCourseList; }
            set { addedCourseList = value; OnPropertyChanged(nameof(AddedCourseList)); }
        }

        private List<string> addableSubjectListForACourse;
        public List<string> AddableSubjectListForACourse
        {
            get { return addableSubjectListForACourse; }
            set { addableSubjectListForACourse = value; OnPropertyChanged(nameof(AddableSubjectListForACourse)); }
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

        private double gpa;

        public double Gpa
        {
            get { return gpa; }
            set { gpa = value; OnPropertyChanged(nameof(Gpa)); }
        }

        private double totalCredit;

        public double TotalCredit
        {
            get { return totalCredit; }
            set { totalCredit = value; OnPropertyChanged(nameof(TotalCredit)); }
        }

        public ICommand OnClickedNextButtonCommand { get; set; }
        public ICommand OnClickedPreviousButtonCommand { get; set; }
        public ICommand SelectedPageCommand { get; set; }
        public ICommand SelectedSemesterCommand { get; set; }
        public ICommand AddCourseCommand { get; set; }
        public ICommand RemoveCourseCommand { get; set; }
        public ICommand UpdateGradeCommand { get; set; }
        public ICommand SaveCommand { get; set; }
        public ICommand CancelCommand { get; set; }

        public SubjectDetailsViewModel(Student selectedStudent)
        {
            this.SelectedStudent = selectedStudent;

            _subjectHelper = App.ServiceProvider.GetService<ISubjectHelper>();
            _courseHelper = App.ServiceProvider.GetService<ICourseHelper>();
            AddableSubjectListForACourse = new List<string>();

            OnClickedNextButtonCommand = new RelayCommand(OnClickedNextButton, CanExecuteNextCommand);
            OnClickedPreviousButtonCommand = new RelayCommand(OnClickedPreviousButton, CanExecutePreviousCommand);
            SelectedPageCommand = new RelayCommand(OnPageSizeChanged, CanExecutePageSizeChanged);
            SelectedSemesterCommand = new RelayCommand(OnSemesterSelected, CanExecutedSemesterSelection);
            AddCourseCommand = new RelayCommand(AddCourse, CanAddCourse);
            UpdateGradeCommand = new RelayCommand(UpdateGrade, CanUpdateGrade);
            RemoveCourseCommand = new RelayCommand(RemoveCourse, CanRemoveCourse);
            SaveCommand = new RelayCommand(Save, CanSave);
            CancelCommand = new RelayCommand(Cancel, CanCancel);

            CurrentPage = Constant.DefaultPage;
            AddedCourseList = new List<Course>();
            InitializePageComboBox();
            InitializeSemesterComboBox();
            CountTotalRows();
            LoadData();
        }

        private void LoadData()
        {
            int limit = SelectedPageSize;

            if (CurrentPage == 0)
            {
                CurrentPage = 1;
            }

            int offset = (CurrentPage - 1) * SelectedPageSize;

            CourseList = GetStudentCourseData(limit, offset);

            UpdatePageLabel();
        }

        private void InitializePageComboBox()
        {
            PageSizeList = Constant.PageSizeList.ToList();
            SelectedPageSize = Constant.DefaultPageSize;
        }

        private void InitializeSemesterComboBox()
        {
            SemesterList = _subjectHelper.GetSemesters();
            SelectedSemester = "1st Semester";
        }

        private void CountTotalRows()
        {
            TotalRows = GetCourseCount();
        }

        private int GetTotalPages()
        {
            return (int)Math.Ceiling((double)TotalRows / SelectedPageSize);
        }

        private void UpdatePageLabel()
        {
            TotalPages = GetTotalPages();

            if (TotalPages == 0)
            {
                CurrentPage = 0;
            }

            CurrentPageLabel = $"{CurrentPage} of {TotalPages}";
        }

        private bool CanExecutePreviousCommand(object obj)
        {
            return CurrentPage > 1;
        }

        private void OnClickedPreviousButton(object obj)
        {
            CurrentPage--;
            UpdatePageLabel();
            LoadData();
        }

        private bool CanExecuteNextCommand(object obj)
        {
            return CurrentPage < GetTotalPages();
        }

        private void OnClickedNextButton(object obj)
        {
            CurrentPage++;
            UpdatePageLabel();
            LoadData();
        }

        private bool CanExecutePageSizeChanged(object obj)
        {
            return true;
        }
        private void OnPageSizeChanged(object obj)
        {
            CurrentPage = Constant.DefaultPage;
            UpdatePageLabel();
            LoadData();
        }

        public List<Course> GetStudentCourseData(int limit, int offset)
        {
            List<Course> courseExistsInDatabase = _courseHelper.GetCourseDetailsForAStudent(limit, offset, SelectedStudent);

            foreach (var course in courseExistsInDatabase)
            {
                int existingCourseIndex = AddedCourseList.FindIndex(c => c.SubjectCode == course.SubjectCode && c.SubjectName == course.SubjectName);

                if (existingCourseIndex < 0)
                {
                    course.DbStatus = Constant.Operation.None;
                    course.IsExistsInDatabase = true;
                    AddedCourseList.Add(course);
                }

            }

            if (AddedCourseList != null)
            {
                List<Course> filteredList = AddedCourseList.Where(c => (c.Semester == SelectedSemester) && (c.DbStatus != Constant.Operation.Delete)).ToList();

                SetCGPAInfo(filteredList);

                return filteredList;
            }

            return null;
        }

        private void SetCGPAInfo(List<Course> filteredList)
        {
            double totalCreditValue = filteredList.Sum(c => Convert.ToDouble(c.CreditHour));
            double averageGrade;

            if (filteredList.Count > 0)
            {
                double weightedSum = filteredList.Sum(c => Convert.ToDouble(c.Grade) * Convert.ToDouble(c.CreditHour));
                averageGrade = weightedSum / totalCreditValue;
                Gpa = averageGrade;
            }
            else
            {
                Gpa = 0.0;
            }

            TotalCredit = totalCreditValue;
        }

        public int GetCourseCount()
        {
            return _courseHelper.CountCourseForAStudent(SelectedStudent, SelectedSemester);
        }

        private bool CanExecutedSemesterSelection(object obj)
        {
            return true;
        }

        private void OnSemesterSelected(object obj)
        {
            CountTotalRows();
            LoadData();
        }

        private void AddCourse(object obj)
        {
            CoursesAvailableToAdd();

            if (AddableSubjectListForACourse.Count == 0)
            {
                MessageBox.Show("No course is available to add!");
                return;
            }

            AddUpdateCourseView addUpdateCourseView = new AddUpdateCourseView();
            AddUpdateCourseViewModel addUpdateCourseViewModel = new AddUpdateCourseViewModel(this, false);
            addUpdateCourseView.DataContext = addUpdateCourseViewModel;
            addUpdateCourseViewModel.CloseAction = addUpdateCourseView.Close;
            addUpdateCourseView.ShowDialog();
            LoadData();
        }

        private bool CanAddCourse(object obj)
        {
            return true;
        }

        private bool CanUpdateGrade(object obj)
        {
            return SelectedCourse != null;
        }

        private void UpdateGrade(object obj)
        {
            AddUpdateCourseView addUpdateCourseView = new AddUpdateCourseView();
            AddUpdateCourseViewModel addUpdateCourseViewModel = new AddUpdateCourseViewModel(this, true);
            addUpdateCourseView.DataContext = addUpdateCourseViewModel;
            addUpdateCourseViewModel.CloseAction = addUpdateCourseView.Close;
            addUpdateCourseView.ShowDialog();
            LoadData();
        }

        private bool CanRemoveCourse(object obj)
        {
            return SelectedCourse != null;
        }

        private void RemoveCourse(object obj)
        {
            if (IsSelectedCoursePrerequisiteForAnotherCourse())
            {
                MessageBox.Show("This course is a prerequisite for another course. Delete those courses before deleting it.");
                return;
            }

            if (AddedCourseList == null || AddedCourseList.Count <= 0)
            {
                return;
            }

            int existingCourseIndex = AddedCourseList.FindIndex(c => c.SubjectCode == selectedCourse.SubjectCode);

            if (existingCourseIndex >= 0)
            {
                AddedCourseList[existingCourseIndex].DbStatus = Constant.Operation.Delete;
                LoadData();
            }
        }

        private bool CanCancel(object obj)
        {
            return true;
        }

        private void Cancel(object obj)
        {
            CloseAction?.Invoke();
        }

        private bool CanSave(object obj)
        {
            return true;
        }

        private void Save(object obj)
        {
            foreach (var course in AddedCourseList)
            {
                if (course.IsExistsInDatabase == true)
                {
                    switch (course.DbStatus)
                    {
                        case Constant.Operation.Delete:

                            bool isCourseDeleted = _courseHelper.DeleteCourse(selectedStudent, course);

                            break;

                        case Constant.Operation.Add:
                        case Constant.Operation.Update:

                            bool isCourseUpdated = _courseHelper.UpdateCourse(selectedStudent, course);
                            break;
                    }
                }
                else
                {
                    switch (course.DbStatus)
                    {
                        case Constant.Operation.Add:
                        case Constant.Operation.Update:

                            bool isCourseAdded = _courseHelper.AddCourse(selectedStudent, course);
                            break;
                    }
                }
            }

            MessageBox.Show("Operations successfully performed");
            CloseAction?.Invoke();
        }

        void CoursesAvailableToAdd()
        {
            List<string> subjects = _subjectHelper.GetSemesterWiseSubjects(SelectedSemester, SelectedStudent.Department);
            AddableSubjectListForACourse.Clear();

            foreach (var subject in subjects)
            {
                string[] parts = subject.Split(':');
                string subjectCode = parts[0].Trim();
                string subjectName = parts[1].Trim();

                int existingCourseIndex = AddedCourseList.FindIndex(c => c.SubjectCode == subjectCode && c.SubjectName == subjectName && c.DbStatus != Constant.Operation.Delete);

                if (existingCourseIndex == -1)
                {
                    AddableSubjectListForACourse.Add(subject);
                }
            }
        }

        bool IsSelectedCoursePrerequisiteForAnotherCourse()
        {
            foreach (var subject in AddedCourseList)
            {
                if (subject.DbStatus == Constant.Operation.Delete)
                {
                    continue;
                }

                string[] parts = subject.SubjectCode.Split('-');
                int subjectId = Convert.ToInt32(parts[1].Trim());
                string department = parts[0].Trim();

                int id = _subjectHelper.GetSubjectId(subjectId, department);

                List<string> prerequisitesSubject = _subjectHelper.GetAllPrerequisiteSubjectList(id, department);

                foreach (var prerequisite in prerequisitesSubject)
                {
                    if (prerequisite == "No Subject")
                    {
                        continue;
                    }

                    parts = prerequisite.Split(':');
                    string prerequisiteSubjectCode = parts[0].Trim();
                    string prerequisiteSubjectName = parts[1].Trim();

                    if (selectedCourse.SubjectCode == prerequisiteSubjectCode && selectedCourse.SubjectName == prerequisiteSubjectName)
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }
}
