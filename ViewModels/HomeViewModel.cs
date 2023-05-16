using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;

using Microsoft.Extensions.DependencyInjection;

using StudentEMS.Command;
using StudentEMS.Constants;
using StudentEMS.Services.Interfaces;
using StudentEMS.Views;

namespace StudentEMS.ViewModels
{
    public class HomeViewModel : BaseViewModel
    {
        private string searchText;
        private IStudentHelper? _studentHelper;
        private ICourseHelper? _courseHelper;

        public HomeViewModel()
        {
            _studentHelper = App.ServiceProvider.GetService<IStudentHelper>();
            _courseHelper = App.ServiceProvider.GetRequiredService<ICourseHelper>();
            searchCommand = new RelayCommand(Search, CanExecuteSearch);
            OnClickedNextButtonCommand = new RelayCommand(OnClickedNextButton, CanExecuteNextCommand);
            OnClickedPreviousButtonCommand = new RelayCommand(OnClickedPreviousButton, CanExecutePreviousCommand);
            SelectedPageCommand = new RelayCommand(OnPageSizeChanged, CanExecutePageSizeChanged);
            DeleteStudentCommand = new RelayCommand(DeleteStudent, CanDeleteStudent);
            UpdateStudentCommand = new RelayCommand(UpdateStudent, CanUpdateStudent);
            SubjectDetailsCommand = new RelayCommand(SubjectDetails, CanGoToSubjectDetails);
            CurrentPage = Constant.DefaultPage;
            InitializeComboBox();
            CountTotalRows();
            LoadData();
        }

        public string SearchText
        {
            get { return searchText; }
            set
            {
                searchText = value;
                OnPropertyChanged(nameof(SearchText));
                searchCommand.Execute(null);
            }
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

        private List<Student> gridData;

        public List<Student> GridData
        {
            get { return gridData; }
            set { gridData = value; OnPropertyChanged(nameof(GridData)); }
        }

        private Student selectedStudent;

        public Student SelectedStudent
        {
            get { return selectedStudent; }
            set { selectedStudent = value; OnPropertyChanged(nameof(SelectedStudent)); }
        }

        public ICommand searchCommand { get; set; }
        public ICommand OnClickedNextButtonCommand { get; set; }
        public ICommand OnClickedPreviousButtonCommand { get; set; }
        public ICommand SelectedPageCommand { get; set; }
        public ICommand DeleteStudentCommand { get; set; }
        public ICommand UpdateStudentCommand { get; set; }
        public ICommand SubjectDetailsCommand { get; set; }

        private void LoadData()
        {
            int limit = SelectedPageSize;

            if (CurrentPage == 0)
            {
                CurrentPage = 1;
            }

            int offset = (CurrentPage - 1) * SelectedPageSize;

            GridData = GetStudentData(limit, offset, searchText);

            UpdatePageLabel();
        }

        private void InitializeComboBox()
        {
            PageSizeList = Constant.PageSizeList.ToList();
            SelectedPageSize = Constant.DefaultPageSize;
        }

        private void CountTotalRows()
        {
            TotalRows = GetStudentCount(searchText);
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

        private void Search(object parameter)
        {
            CurrentPage = Constant.DefaultPage;
            CountTotalRows();
            LoadData();
        }

        private bool CanExecuteSearch(object parameter)
        {
            return true;
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

        public List<Student> GetStudentData(int limit, int offset, string filterText)
        {
            List<Student> result = _studentHelper.GetAllStudentInfo(limit, offset, filterText);
            return result;
        }

        public int GetStudentCount(string filterText)
        {
            int staffCount = _studentHelper.GetStudentCount(filterText);
            return staffCount;
        }

        private bool CanDeleteStudent(object obj)
        {
            return SelectedStudent != null;
        }

        private void DeleteStudent(object obj)
        {
            MessageBoxResult result = MessageBox.Show("Are you sure you want to delete?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.No)
            {
                return;
            }

            bool isCourseDeleted = _courseHelper.DeleteCoursesWhenAStudentDeleted(selectedStudent.StudentId);

            if (!isCourseDeleted)
            {
                return;
            }

            //string studentImagePath = _studentHelper.GetStudentImagePath(selectedStudent.Email);

            //if (File.Exists(studentImagePath))
            //{
            //    File.Delete(studentImagePath);
            //}


            bool isStudentDeleted = _studentHelper.DeleteStudent(selectedStudent.StudentId);

            if (isStudentDeleted)
            {
                MessageBox.Show("Succesfully Deleted");
                LoadData();
            }
        }

        private bool CanUpdateStudent(object obj)
        {
            return SelectedStudent != null;
        }

        private void UpdateStudent(object obj)
        {
            UpdateStudentViewModel updateViewModel = new UpdateStudentViewModel(SelectedStudent);
            UpdateStudentWindow updateView = new UpdateStudentWindow();
            updateView.DataContext = updateViewModel;
            updateViewModel.CloseAction = updateView.Close;

            updateView.ShowDialog();
            LoadData();

        }

        private bool CanGoToSubjectDetails(object obj)
        {
            return SelectedStudent != null;
        }

        private void SubjectDetails(object obj)
        {
            SubjectDetailsViewModel sdvm = new SubjectDetailsViewModel(SelectedStudent);
            SubjectDetailsView sdv = new SubjectDetailsView();
            sdv.DataContext = sdvm;
            sdvm.CloseAction = sdv.Close;
            sdv.ShowDialog();
            LoadData();
        }
    }
}
