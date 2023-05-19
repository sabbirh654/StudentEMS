using StudentEMS.Models;
using StudentEMS.Services.Interfaces;
using System.Collections.Generic;
using System;
using System.Windows.Input;
using StudentEMS.Command;
using StudentEMS.Constants;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;

namespace StudentEMS.ViewModels
{
    public class SubjectViewModel : BaseViewModel
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

        private List<Subject> subjectList;
        public List<Subject> SubjectList
        {
            get { return subjectList; }
            set { subjectList = value; OnPropertyChanged(nameof(SubjectList)); }
        }

        private Course selectedSubject;
        public Course SelectedSubject
        {
            get { return selectedSubject; }
            set { selectedSubject = value; OnPropertyChanged(nameof(SelectedSubject)); }
        }

        public ICommand AddSubjectCommand { get; set; }
        public ICommand DeleteSubjectCommand { get; set; }
        public ICommand UpdateSubjectCommand { get; set; }
        public ICommand SelectedPageCommand { get; set; }
        public ICommand OnClickedNextButtonCommand { get; set; }
        public ICommand OnClickedPreviousButtonCommand { get; set; }


        public SubjectViewModel()
        {
            _subjectHelper = App.ServiceProvider.GetService<ISubjectHelper>();
            _courseHelper = App.ServiceProvider.GetService<ICourseHelper>();

            SelectedPageCommand = new RelayCommand(OnPageSizeChanged, CanExecutePageSizeChanged);
            OnClickedNextButtonCommand = new RelayCommand(OnClickedNextButton, CanExecuteNextCommand);
            OnClickedPreviousButtonCommand = new RelayCommand(OnClickedPreviousButton, CanExecutePreviousCommand);
            InitializePageComboBox();
            CountTotalRows();
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

        private void LoadData()
        {
            int limit = SelectedPageSize;

            if (CurrentPage == 0)
            {
                CurrentPage = 1;
            }

            int offset = (CurrentPage - 1) * SelectedPageSize;

            SubjectList = GetSubjectData(limit, offset);

            UpdatePageLabel();
        }

        private void InitializePageComboBox()
        {
            PageSizeList = Constant.PageSizeList.ToList();
            SelectedPageSize = Constant.DefaultPageSize;
        }

        private void CountTotalRows()
        {
            TotalRows = GetSubjectCount();
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

        public List<Subject> GetSubjectData(int limit, int offset)
        {
            List<Subject> data = _subjectHelper.GetAllSubjectInformation(limit, offset);
            return data;
        }
        public int GetSubjectCount()
        {
            int count = _subjectHelper.GetSubjectCount();
            return count;
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
    }
}