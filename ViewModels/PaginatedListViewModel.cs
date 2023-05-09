using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

using StudentEMS.AppData;
using StudentEMS.Command;
using StudentEMS.Constants;
using StudentEMS.Models;

namespace StudentEMS.ViewModels
{
    public class PaginatedListViewModel : BaseViewModel
    {
        public HomeViewModel Home { get; set; }

        public PaginatedListViewModel()
        {
            searchCommand = new RelayCommand(Search, CanExecuteSearch);
            OnClickedNextButtonCommand = new RelayCommand(OnClickedNextButton, CanExecuteNextCommand);
            OnClickedPreviousButtonCommand = new RelayCommand(OnClickedPreviousButton, CanExecutePreviousCommand);
            SelectedPageCommand = new RelayCommand(OnPageSizeChanged, CanExecutePageSizeChanged);
            CurrentPage = Constant.DefaultPage;
            Home = new HomeViewModel();
            Columns = new ObservableCollection<ColumnModel>();

            GenerateColumns();
            InitializeComboBox();
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

        private string searchText;
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


        private ObservableCollection<ColumnModel> columns;
        public ObservableCollection<ColumnModel> Columns
        {
            get { return columns; }
            set
            {
                if (columns != value)
                {
                    columns = value;
                    OnPropertyChanged(nameof(Columns));
                }
            }
        }

        private List<dynamic> gridData;

        public List<dynamic> GridData
        {
            get { return gridData; }
            set { gridData = value; OnPropertyChanged(nameof(GridData)); }
        }


        public ICommand searchCommand { get; set; }
        public ICommand OnClickedNextButtonCommand { get; set; }
        public ICommand OnClickedPreviousButtonCommand { get; set; }
        public ICommand SelectedPageCommand { get; set; }

        public ICommand GenerateColumnsCommand { get; set; }

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

        private void GenerateColumns()
        {
            switch (CurrentView.CurrentViewName)
            {
                case "Home":
                    Columns = Home.GenerateColumsForStaffHome();
                    break;
            }
        }

        private void LoadData()
        {
            int limit = SelectedPageSize;

            if (CurrentPage == 0)
            {
                CurrentPage = 1;
            }

            int offset = (CurrentPage - 1) * SelectedPageSize;

            switch (CurrentView.CurrentViewName)
            {
                case "Home":
                    GridData = Home.GetStudentData(limit, offset, searchText).Cast<dynamic>().ToList();
                    break;
            }

            UpdatePageLabel();
        }

        private void InitializeComboBox()
        {
            PageSizeList = Constant.PageSizeList.ToList();
            SelectedPageSize = Constant.DefaultPageSize;
        }

        private void CountTotalRows()
        {
            switch (CurrentView.CurrentViewName)
            {
                case "Home":
                    TotalRows = Home.GetStudentCount(searchText);
                    break;
            }
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
    }
}
