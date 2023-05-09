using Microsoft.Extensions.DependencyInjection;
using Microsoft.Win32;

using StudentEMS.Constants;
using StudentEMS.Models;
using StudentEMS.Services.Interfaces;

using System;
using System.Collections.Generic;
using System.Globalization;

namespace StudentEMS.Services
{
    public class LoginServices : ILoginServices
    {
        private IDatabaseAccess _databaseAccess = App.ServiceProvider.GetService<IDatabaseAccess>();
        private static string GetSingleStudentQuery =>
            @"
            SELECT
                Users.Id
            FROM users
            INNER JOIN students
                ON users.Id = students.UserId
            WHERE users.UserName = @userName
                AND students.Password = @password
                AND students.IsDeleted = FALSE";

        public bool IsStudentExist(string email, string password)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();

            parameters.Add("userName", email);
            parameters.Add("password", password);

            string queryResult = _databaseAccess.GetSingleData(GetSingleStudentQuery, parameters);

            if (queryResult != null
                && int.TryParse(queryResult, out int count)
                && count > 0)
            {
                return true;
            }
            return false;
        }
        public void StoreCredentials(string userName, string password, string userRole)
        {
            RegistryKey key = Registry.CurrentUser.CreateSubKey(Constant.RegistryPath);
            string valueEditedTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            key.SetValue("UserName", userName);
            key.SetValue("Password", password);
            key.SetValue("UserRole", userRole);
            key.SetValue("EditTime", valueEditedTime);
        }
        public User GetUserCredentials()
        {
            RegistryKey key = Registry.CurrentUser.OpenSubKey(Constant.RegistryPath, true);
            User user = new User();

            if (key != null)
            {
                var username = key.GetValue("UserName");
                var password = key.GetValue("Password");
                var userrole = key.GetValue("UserRole");

                if (username != null && password != null && userrole != null)
                {
                    user.UserName = username.ToString();
                    user.UserRole = userrole.ToString();
                    user.Password = password.ToString();
                }
            }
            return user;
        }
        public void RemoveCredentials()
        {
            RegistryKey key = Registry.CurrentUser.OpenSubKey(Constant.RegistryPath, true);
            if (key != null)
            {
                key.DeleteValue("UserName", false);
                key.DeleteValue("Password", false);
                key.DeleteValue("UserRole", false);
                key.DeleteValue("EditTime", false);
            }
        }
        public bool IsRememberingUser()
        {
            RegistryKey key = Registry.CurrentUser.OpenSubKey(Constant.RegistryPath, true);
            if (key != null)
            {
                var lastedit = key.GetValue("EditTime");
                if (lastedit != null)
                {
                    string lastEditTimeString = lastedit.ToString();
                    DateTime lastEditTime = DateTime.ParseExact(lastEditTimeString, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture); // constant
                    TimeSpan timeElapsed = DateTime.Now - lastEditTime;

                    if (timeElapsed.TotalDays < Constant.RememberUserTimeInDays)
                    {
                        return true;
                    }
                }
            }
            RemoveCredentials();
            return false;
        }
    }
}
