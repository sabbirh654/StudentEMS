using Microsoft.Extensions.DependencyInjection;

using StudentEMS.Models;
using StudentEMS.Services.Interfaces;

using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace StudentEMS.ViewModels
{
    public class HomeViewModel : BaseViewModel
    {
        private IStudentHelper _studentHelper;
        public PaginatedListViewModel PaginatedListViewModel { get; set; }
        public HomeViewModel()
        {
            _studentHelper = App.ServiceProvider.GetService<IStudentHelper>();
        }

        public ObservableCollection<ColumnModel> GenerateColumsForStaffHome()
        {
            ObservableCollection<ColumnModel> columns = new ObservableCollection<ColumnModel>()
            {
                new ColumnModel() {HeaderText = "First Name", BindingPath="FirstName"},
                new ColumnModel() {HeaderText = "Last Name", BindingPath="LastName"},
                new ColumnModel() {HeaderText = "Email", BindingPath="Email"},
                new ColumnModel() {HeaderText = "Session", BindingPath="Session"},
                new ColumnModel() {HeaderText = "Department", BindingPath="Department"},
                new ColumnModel() {HeaderText = "CGPA", BindingPath="CGPA", Size=200},
                new ColumnModel() {HeaderText = "Last Enrolled Semester", BindingPath="LastEnrolledSemester"},
                new ColumnModel() {HeaderText = "Total Earned Credits", BindingPath="TotalEarnedCredits"}
            };

            return columns;
        }

        public List<Student> GetStudentData(int limit, int offset, string filterText)
        {
            return _studentHelper.GetAllStudentInfo(limit, offset, filterText);
        }

        public int GetStudentCount(string filterText)
        {
            int staffCount = _studentHelper.GetStudentCount(filterText);
            return staffCount;
        }
    }
}