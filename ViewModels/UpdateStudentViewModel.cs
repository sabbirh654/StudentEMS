using Microsoft.Extensions.DependencyInjection;
using Microsoft.Win32;

using StudentEMS.Command;
using StudentEMS.Services.Interfaces;
using StudentEMS.Constants;

using System;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Threading.Tasks;

namespace StudentEMS.ViewModels
{
    public class UpdateStudentViewModel : BaseViewModel
    {
        private Student selectedStudent;
        private IStudentHelper _studentHelper;

        public ICommand SaveCommand { get; set; }
        public ICommand CancelCommand { get; set; }
        public ICommand UploadImageCommand { get; set; }
        public ICommand RemoveImageCommand { get; set; }

        public Action CloseAction { get; set; }

        public Student SelectedStudent
        {
            get { return selectedStudent; }
            set { selectedStudent = value; OnPropertyChanged(nameof(SelectedStudent)); }
        }

        private bool isModalVisible;
        public bool IsModalVisible
        {
            get { return isModalVisible; }
            set
            {
                isModalVisible = value;
                OnPropertyChanged(nameof(IsModalVisible));
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

        public UpdateStudentViewModel(Student selectedStudent)
        {
            _studentHelper = App.ServiceProvider.GetService<IStudentHelper>();
            SaveCommand = new RelayCommand(SaveStudent, CanSaveStudent);
            CancelCommand = new RelayCommand(CancelStudentUpdate, CanCancel);
            UploadImageCommand = new RelayCommand(UploadImage, CanUploadImage);
            RemoveImageCommand = new RelayCommand(RemoveImage, CanRemoveImage);

            SelectedStudent = selectedStudent;
            SelectedImagePath = SelectedStudent.ProfilePicturePath;

            if(string.IsNullOrEmpty(SelectedImagePath))
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
                SelectedImagePath = Constant.DummyImagePath;
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
            return true;
        }

        private void SaveStudent(object obj)
        {
            SaveImage();

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

            if(SelectedImagePath == Constant.DummyImagePath)
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
    }
}
