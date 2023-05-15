using StudentEMS.AppData;
using StudentEMS.Command;
using StudentEMS.Views;

using System.Windows.Input;

using static StudentEMS.Constants.Constant;

namespace StudentEMS.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        private BaseViewModel selectedViewModel;

        public BaseViewModel SelectedViewModel
        {
            get { return selectedViewModel; }
            set { selectedViewModel = value; OnPropertyChanged("SelectedViewModel"); }
        }

        public ICommand SelectViewCommand { get; set; }

        public MainViewModel()
        {
            HomeViewModel homeViewModel = new HomeViewModel();
            HomeView homeView = new HomeView();
            homeView.DataContext = homeViewModel;
            SelectedViewModel = homeViewModel;

            CurrentView.CurrentViewName = "Home";
            SelectViewCommand = new RelayCommand(SelectCurrentView, CanSelectCurrentView);
        }

        private bool CanSelectCurrentView(object obj)
        {
            return true;
        }

        private void SelectCurrentView(object obj)
        {
            string? parameter = obj as string;

            if (parameter == NavigationItem.Home.ToString())
            {
                SelectedViewModel = new HomeViewModel();
                CurrentView.CurrentViewName = parameter;
            }
            else if (parameter == NavigationItem.UpdateProfile.ToString())
            {
                SelectedViewModel = new UpdateProfileViewModel();
                CurrentView.CurrentViewName = parameter;
            }
            else if (parameter == NavigationItem.Subject.ToString())
            {
                SelectedViewModel = new SubjectViewModel();
                CurrentView.CurrentViewName = parameter;
            }
            else if (parameter == NavigationItem.Exit.ToString())
            {
                SelectedViewModel = new ExitViewModel();
                CurrentView.CurrentViewName = parameter;
            }
        }
    }
}
