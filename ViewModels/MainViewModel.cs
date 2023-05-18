﻿using StudentEMS.AppData;
using StudentEMS.Command;
using StudentEMS.Models;
using StudentEMS.Views;

using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

using static StudentEMS.Constants.Constant;

namespace StudentEMS.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        private BaseViewModel selectedViewModel;

        private bool studentNavigationVisibility;

        private User currentUser;

        public User CurrentUser
        {
            get { return currentUser; }
            set { currentUser = value; OnPropertyChanged(nameof(CurrentUser)); }
        }


        public bool StudentNavigationVisibility
        {
            get { return studentNavigationVisibility; }
            set { studentNavigationVisibility = value; OnPropertyChanged(nameof(StudentNavigationVisibility)); }
        }

        private bool staffNavigationVisibility;

        public bool StaffNavigationVisibility
        {
            get { return staffNavigationVisibility; }
            set { staffNavigationVisibility = value; OnPropertyChanged(nameof(StaffNavigationVisibility)); }
        }


        public BaseViewModel SelectedViewModel
        {
            get { return selectedViewModel; }
            set { selectedViewModel = value; OnPropertyChanged("SelectedViewModel"); }
        }

        private string currentDateTime;

        public string CurrentDateTime
        {
            get { return currentDateTime; }
            set { currentDateTime = value; OnPropertyChanged(nameof(CurrentDateTime)); }
        }

        private DispatcherTimer timer;
        public ICommand SelectViewCommand { get; set; }
        public ICommand ExitCommand { get; set; }

        public MainViewModel(User currentUser)
        {
            this.CurrentUser = currentUser;

            switch (CurrentUser.UserRole)
            {
                case nameof(UserRole.Student):
                    StudentNavigationVisibility = true;
                    StaffNavigationVisibility = false;
                    break;
                case nameof(UserRole.Staff):
                    StudentNavigationVisibility = false;
                    StaffNavigationVisibility = true;
                    break;
                default:
                    break;
            }

            if (StaffNavigationVisibility)
            {
                HomeViewModel homeViewModel = new HomeViewModel();
                HomeView homeView = new HomeView();
                homeView.DataContext = homeViewModel;
                SelectedViewModel = homeViewModel;
            }
            else
            {
                StudentHomeViewModel studentHomeViewModel = new StudentHomeViewModel();
                StudentHomeView studentHomeView = new StudentHomeView();
                studentHomeView.DataContext = studentHomeViewModel;
                SelectedViewModel = studentHomeViewModel;
            }

            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += TimerTick;
            timer.Start();

            ExitCommand = new RelayCommand(Exit, CanExit);

            CurrentView.CurrentViewName = "Home";
            SelectViewCommand = new RelayCommand(SelectCurrentView, CanSelectCurrentView);
        }

        private bool CanExit(object obj)
        {
            return true;
        }

        private void Exit(object obj)
        {
            Application.Current.Shutdown();
        }

        private bool CanSelectCurrentView(object obj)
        {
            return true;
        }

        private void SelectCurrentView(object obj)
        {
            string? parameter = obj as string;

            switch (parameter)
            {
                case nameof(NavigationItem.StaffHome):
                    SelectedViewModel = new HomeViewModel();
                    CurrentView.CurrentViewName = parameter;
                    break;
                case nameof(NavigationItem.StudentHome):
                    SelectedViewModel = new StudentHomeViewModel();
                    CurrentView.CurrentViewName = parameter;
                    break;
                case nameof(NavigationItem.StaffUpdateProfile):
                    SelectedViewModel = new UpdateProfileViewModel();
                    CurrentView.CurrentViewName = parameter;
                    break;
                case nameof(NavigationItem.Subject):
                    SelectedViewModel = new SubjectViewModel();
                    CurrentView.CurrentViewName = parameter;
                    break;
                default:
                    break;
            }
        }

        private void TimerTick(object sender, EventArgs e)
        {
            CurrentDateTime = DateTime.Now.ToString("dd-MMM-yyyy" + "\n" + "hh:mm:ss tt");
        }
    }
}
