using Microsoft.Extensions.DependencyInjection;

using StudentEMS.Command;
using StudentEMS.Constants;
using StudentEMS.Models;
using StudentEMS.Services.Interfaces;

using System;
using System.Collections;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using System.DirectoryServices.AccountManagement;

using static StudentEMS.Constants.Constant;
using StudentEMS.Views;
using System.Linq;

namespace StudentEMS.ViewModels
{
    public class LoginViewModel : BaseViewModel, INotifyDataErrorInfo
    {

        private readonly ErrorsViewModel _errorsViewModel;
        private IDatabaseHelper? _databaseHelper;
        private ILoginServices? _loginServices;
        private LoginView loginView;

        private bool isRememberMeChecked;
        public bool IsRememberMeChecked
        {
            get { return isRememberMeChecked; }
            set { isRememberMeChecked = value; OnPropertyChanged(nameof(IsRememberMeChecked)); }
        }

        private bool isStudentLogin;
        public bool IsStudentLogin
        {
            get { return isStudentLogin; }
            set
            {
                isStudentLogin = value;
                OnPropertyChanged(nameof(IsStudentLogin));

                if (IsStudentLogin)
                {
                    OnStudentLogin.Execute(null);
                }
            }
        }

        private bool isStaffLogin;
        public bool IsStaffLogin
        {
            get { return isStaffLogin; }
            set
            {
                isStaffLogin = value;
                OnPropertyChanged(nameof(IsStaffLogin));

                if (IsStaffLogin)
                {
                    OnStaffLogin.Execute(null);
                }
            }
        }

        private string userName;
        public string UserName
        {
            get { return userName; }
            set
            {
                userName = value;
                ValidateUserName();
                OnPropertyChanged(nameof(UserName));
            }
        }

        private string password;
        public string Password
        {
            get { return password; }
            set
            {
                password = value;
                ValidatePassword();
                OnPropertyChanged(nameof(Password));
            }
        }

        private bool isStaffLoginEnabled;
        public bool IsStaffLoginEnabled
        {
            get { return isStaffLoginEnabled; }
            set { isStaffLoginEnabled = value; OnPropertyChanged(nameof(IsStaffLoginEnabled)); }
        }

        public ICommand LoginCommand { get; set; }
        public ICommand ClearCommand { get; set; }
        public ICommand ExitCommand { get; set; }
        public ICommand OnStaffLogin { get; set; }
        public ICommand OnStudentLogin { get; set; }

        public LoginViewModel(LoginView loginView)
        {
            _loginServices = App.ServiceProvider.GetService<ILoginServices>();
            _databaseHelper = App.ServiceProvider.GetService<IDatabaseHelper>();

            LoginCommand = new RelayCommand(Login, CanLogin);
            ClearCommand = new RelayCommand(Clear, CanClear);
            ExitCommand = new RelayCommand(Exit, CanExit);
            OnStaffLogin = new RelayCommand(StaffLogin, CanStaffLogin);
            OnStudentLogin = new RelayCommand(StudentLogin, CanStudentLogin);

            _errorsViewModel = new ErrorsViewModel();
            _errorsViewModel.ErrorsChanged += ErrorsViewModel_ErrorsChanged;

            this.loginView = loginView;
            IsStaffLoginEnabled = true;
            IsStudentLogin = true;
            IsRememberMeChecked = false;

            CheckUser();
            RememberUser();
        }

        private bool CanStudentLogin(object obj)
        {
            return true;
        }

        private void StudentLogin(object obj)
        {
            UserName = string.Empty;
            Password = string.Empty;
        }

        private bool CanStaffLogin(object obj)
        {
            return true;
        }

        private void StaffLogin(object obj)
        {
            UserName = Environment.UserName;
            Password = string.Empty;
        }

        private bool CanExit(object obj)
        {
            return true;
        }

        private void Exit(object obj)
        {
            Application.Current.Shutdown();
        }

        private bool CanClear(object obj)
        {
            return true;
        }

        private void Clear(object obj)
        {
            Password = string.Empty;
            IsRememberMeChecked = false;

            if (!IsStaffLogin)
            {
                UserName = string.Empty;
            }
        }

        private bool CanLogin(object obj)
        {
            return !HasErrors;
        }

        private void Login(object obj)
        {
            if (IsStudentLogin)
            {
                if (_loginServices.IsStudentExist(UserName.Trim(), Password.Trim()))
                {
                    CreateMainForm(Constant.UserRole.Student, UserName.Trim());
                }
                else
                {
                    MessageBox.Show("Wrong username or password!");
                }
            }
            else
            {
                try
                {
                    using (PrincipalContext context = new PrincipalContext(ContextType.Domain))
                    {
                        string username = UserName.Trim();
                        string password = Password.Trim();

                        if (context.ValidateCredentials(username, password))
                        {
                            CreateMainForm(Constant.UserRole.Staff, username);
                        }
                        else
                        {
                            MessageBox.Show("Wrong Password!");
                        }
                    }
                }
                catch (Exception d)
                {
                    MessageBox.Show(d.Message);
                }
            }
        }

        private void CreateMainForm(UserRole userRole, string userName)
        {
            if (IsRememberMeChecked)
            {
                _loginServices.StoreCredentials(userName, Password.Trim(), userRole.ToString());
            }

            User currentUser = new User();
            currentUser.UserName = userName;
            currentUser.UserRole = userRole.ToString();

            loginView.Visibility = Visibility.Collapsed;

            MainWindow main = new MainWindow();
            MainViewModel mainModel = new MainViewModel(currentUser);
            main.DataContext = mainModel;
            main.ShowDialog();
            loginView.Close();
        }

        private void RememberUser()
        {
            if (!_loginServices.IsRememberingUser())
            {
                loginView.DataContext = this;
                loginView.ShowDialog();
                return;
            }

            loginView.Visibility = Visibility.Collapsed;
            User user = _loginServices.GetUserCredentials();

            MainWindow main = new MainWindow();
            MainViewModel mainModel = new MainViewModel(user);
            main.DataContext = mainModel;
            main.ShowDialog();
            loginView.Close();
        }

        public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;
        public bool HasErrors => _errorsViewModel.HasErrors;
        public bool CanCreate => !HasErrors;

        public IEnumerable GetErrors(string propertyName)
        {
            return _errorsViewModel.GetErrors(propertyName);
        }

        private void ErrorsViewModel_ErrorsChanged(object sender, DataErrorsChangedEventArgs e)
        {
            ErrorsChanged?.Invoke(this, e);
            OnPropertyChanged(nameof(CanCreate));
        }

        private void ValidateUserName()
        {
            _errorsViewModel.ClearErrors(nameof(UserName));

            if (string.IsNullOrEmpty(UserName))
            {
                _errorsViewModel.AddError(nameof(UserName), "The user name field cannot be left empty.");
            }

            else
            {
                _errorsViewModel.ClearErrors(nameof(UserName));
            }
        }

        private void ValidatePassword()
        {
            if (IsStaffLogin)
            {
                _errorsViewModel.ClearErrors(nameof(Password));
                return;
            }

            if (IsStudentLogin)
            {
                _errorsViewModel.ClearErrors(nameof(Password));

                if (Password.Length < 6)
                {
                    _errorsViewModel.AddError(nameof(Password), "The password must be atleast 6 characters long.");
                }
                else
                {
                    _errorsViewModel.ClearErrors(nameof(Password));
                }
            }
        }

        private void CheckUser()
        {
            string userName = System.Net.NetworkInformation.IPGlobalProperties.GetIPGlobalProperties().DomainName;
            if (userName != System.Configuration.ConfigurationManager.AppSettings["DomainName"])
            {
                IsStaffLoginEnabled = false;
            }
        }
    }
}
