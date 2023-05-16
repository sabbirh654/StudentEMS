using Microsoft.Extensions.DependencyInjection;
using Microsoft.Win32;

using StudentEMS.Command;
using StudentEMS.Services.Interfaces;
using StudentEMS.Constants;

using System;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.ComponentModel;
using System.Collections;
using System.Text.RegularExpressions;

namespace StudentEMS.ViewModels
{
    public class UpdateStudentViewModel : BaseViewModel, INotifyDataErrorInfo
    {
        private IStudentHelper _studentHelper;
        private readonly ErrorsViewModel _errorsViewModel;
        public ICommand SaveCommand { get; set; }
        public ICommand CancelCommand { get; set; }
        public ICommand UploadImageCommand { get; set; }
        public ICommand RemoveImageCommand { get; set; }

        public Action CloseAction { get; set; }

        private Student selectedStudent;
        public Student SelectedStudent
        {
            get { return selectedStudent; }
            set
            {
                selectedStudent = value;
                OnPropertyChanged(nameof(SelectedStudent));
            }
        }

        private string _selectedImagePath;
        public string SelectedImagePath
        {
            get { return _selectedImagePath; }
            set
            {
                _selectedImagePath = value;
                OnPropertyChanged(nameof(SelectedImagePath));
            }
        }

        private string fisrtName;
        public string FirstName
        {
            get { return fisrtName; }
            set
            {
                fisrtName = value;

                _errorsViewModel.ClearErrors(nameof(FirstName));

                if (string.IsNullOrEmpty(FirstName))
                {
                    _errorsViewModel.AddError(nameof(FirstName), "The first name field cannot be left empty.");
                }

                else
                {
                    _errorsViewModel.ClearErrors(nameof(FirstName));
                }

                OnPropertyChanged(nameof(FirstName));
            }
        }

        private string lastName;
        public string LastName
        {
            get { return lastName; }
            set
            {
                lastName = value;

                _errorsViewModel.ClearErrors(nameof(LastName));

                if (string.IsNullOrEmpty(LastName))
                {
                    _errorsViewModel.AddError(nameof(LastName), "The last name field cannot be left empty.");
                }

                else
                {
                    _errorsViewModel.ClearErrors(nameof(LastName));
                }

                OnPropertyChanged(nameof(LastName));
            }
        }

        private string email;
        public string Email
        {
            get { return email; }
            set { email = value; OnPropertyChanged(nameof(Email)); }
        }

        private DateTime dateOfBirth;
        public DateTime DateOfBirth
        {
            get { return dateOfBirth; }
            set { dateOfBirth = value; OnPropertyChanged(nameof(DateOfBirth)); }
        }

        private string contactNumber;

        public string ContactNumber
        {
            get { return contactNumber; }
            set
            {
                contactNumber = value;

                _errorsViewModel.ClearErrors(nameof(ContactNumber));

                if (string.IsNullOrEmpty(ContactNumber) || !Regex.IsMatch(ContactNumber, @"^\d+$"))
                {
                    _errorsViewModel.AddError(nameof(ContactNumber), "The contact no. field can only accept digits.");
                }

                else
                {
                    _errorsViewModel.ClearErrors(nameof(ContactNumber));
                }

                OnPropertyChanged(nameof(ContactNumber));
            }
        }

        private string password;

        public string Password
        {
            get { return password; }
            set { password = value; OnPropertyChanged(nameof(Password)); }
        }


        public UpdateStudentViewModel(Student selectedStudent)
        {
            _studentHelper = App.ServiceProvider.GetService<IStudentHelper>();
            _errorsViewModel = new ErrorsViewModel();
            _errorsViewModel.ErrorsChanged += ErrorsViewModel_ErrorsChanged;

            SaveCommand = new RelayCommand(SaveStudent, CanSaveStudent);
            CancelCommand = new RelayCommand(CancelStudentUpdate, CanCancel);
            UploadImageCommand = new RelayCommand(UploadImage, CanUploadImage);
            RemoveImageCommand = new RelayCommand(RemoveImage, CanRemoveImage);

            SelectedStudent = selectedStudent;
            FirstName = selectedStudent.FirstName;
            LastName = selectedStudent.LastName;
            Email = selectedStudent.Email;
            DateOfBirth = selectedStudent.DateOfBirth;
            ContactNumber = selectedStudent.ContactNumber;
            Password = selectedStudent.Password;
            SelectedImagePath = selectedStudent.ProfilePicturePath;

            if (string.IsNullOrEmpty(SelectedImagePath))
            {
                SelectedImagePath = Constant.DummyImagePath;
            }

        }

        private bool CanRemoveImage(object obj)
        {
            return true;
        }

        private void RemoveImage(object obj)
        {
            if (!string.IsNullOrEmpty(SelectedImagePath) && SelectedImagePath != Constant.DummyImagePath)
            {
                SelectedImagePath = Constant.DummyImagePath;
            }
        }

        private bool CanUploadImage(object obj)
        {
            return true;
        }

        private void UploadImage(object obj)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "Select an image";
            openFileDialog.Filter = Constant.ImageExtensionFilter;

            SelectedImagePath = Constant.DummyImagePath;
            if (openFileDialog.ShowDialog() == true)
            {
                SelectedImagePath = openFileDialog.FileName;
            }
        }

        private bool CanCancel(object obj)
        {
            return true;
        }

        private void CancelStudentUpdate(object obj)
        {
            CloseAction?.Invoke();
        }

        private bool CanSaveStudent(object obj)
        {
            return CanCreate;
        }

        private void SaveStudent(object obj)
        {
            SaveImage();

            SelectedStudent.FirstName = FirstName;
            SelectedStudent.LastName = LastName;
            SelectedStudent.Email = Email;
            SelectedStudent.DateOfBirth = DateOfBirth;
            SelectedStudent.ContactNumber = ContactNumber;
            SelectedStudent.Password = Password;

            bool isUpdateSuccess = _studentHelper.UpdateStudent(SelectedStudent);
            if (isUpdateSuccess)
            {
                MessageBox.Show("Succesfully Updated");
                CloseAction?.Invoke();
            }
            else
            {
                MessageBox.Show("Student can't be updated!");
            }
        }

        private void SaveImage()
        {
            RemovePreviousImage();

            if (SelectedImagePath == Constant.DummyImagePath)
            {
                SelectedStudent.ProfilePicturePath = null;
                return;
            }

            if (!string.IsNullOrEmpty(SelectedImagePath))
            {

                string imageFolderPath = @"D:\ImageResources";
                if (!Directory.Exists(imageFolderPath))
                {
                    Directory.CreateDirectory(imageFolderPath);
                }

                string randomFileName = Path.GetFileNameWithoutExtension(SelectedImagePath) + "_" + Guid.NewGuid().ToString().Substring(0, 5) + Path.GetExtension(SelectedImagePath);

                string destinationPath = Path.Combine(imageFolderPath, randomFileName);
                File.Copy(SelectedImagePath, destinationPath);
                SelectedStudent.ProfilePicturePath = destinationPath;
            }
        }
        private void RemovePreviousImage()
        {
            string previousImagePath = SelectedStudent.ProfilePicturePath;

            if (File.Exists(previousImagePath))
            {
                File.Delete(previousImagePath);
            }
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
    }
}
