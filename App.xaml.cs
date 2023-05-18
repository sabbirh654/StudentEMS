using Microsoft.Extensions.DependencyInjection;
using StudentEMS.Interfaces;
using StudentEMS.Services.Interfaces;
using StudentEMS.Services;

using System;
using System.Windows;
using StudentEMS.Views;
using StudentEMS.ViewModels;

namespace StudentEMS
{
    public partial class App : Application
    {
        public static IServiceProvider? ServiceProvider { get; set; }
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var services = new ServiceCollection();
            services.AddTransient<IDatabaseHelper, DatabaseHelper>();
            services.AddTransient<IDatabaseAccess, DatabaseAccess>();
            services.AddTransient<IValidationServices, ValidationServices>();
            services.AddTransient<ILoginServices, LoginServices>();
            services.AddTransient<IUserPreferenceService, UserPreferenceService>();
            services.AddTransient<IAuditLogServices, AuditLogServices>();
            services.AddTransient<ISubjectHelper, SubjectHelper>();
            services.AddTransient<IStudentHelper, StudentHelper>();
            services.AddTransient<ICourseHelper, CourseHelper>();

            ServiceProvider = services.BuildServiceProvider();

            LoginView loginView = new LoginView();
            LoginViewModel loginViewModel = new LoginViewModel(loginView);
        }
    }
}
