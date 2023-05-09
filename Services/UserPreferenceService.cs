using Microsoft.Extensions.DependencyInjection;

using StudentEMS.Services.Interfaces;

using System;

namespace StudentEMS.Services
{
    public class UserPreferenceService : IUserPreferenceService
    {
        private IDatabaseHelper _databaseHelper;
        public UserPreferenceService()
        {
            _databaseHelper = App.ServiceProvider.GetService<IDatabaseHelper>();
        }
        public bool IsUserPreferenceExists(string userName, string currentPageName)
        {
            int userId = Convert.ToInt32(_databaseHelper.GetUserStaffId(userName));
            int keyId = Convert.ToInt32(_databaseHelper.GetUserPreferenceKeyId(currentPageName));

            return _databaseHelper.IsUserPreferenceExists(userId, keyId);
        }
        public void UpdateUserPreferences(string userName, string currentPageName, string value)
        {
            int userId = Convert.ToInt32(_databaseHelper.GetUserStaffId(userName));
            int keyId = Convert.ToInt32(_databaseHelper.GetUserPreferenceKeyId(currentPageName));

            _databaseHelper.UpdateUserPreferences(userId, keyId, value);
        }
        public void InsertUserPreferences(string userName, string currentPageName, string value)
        {
            int userId = Convert.ToInt32(_databaseHelper.GetUserStaffId(userName));
            int keyId = Convert.ToInt32(_databaseHelper.GetUserPreferenceKeyId(currentPageName));

            _databaseHelper.InsertUserPreferences(userId, keyId, value);
        }
        public string GetUserPreferenceValue(string userName, string currentPageName)
        {
            int userId = Convert.ToInt32(_databaseHelper.GetUserStaffId(userName));
            int keyId = Convert.ToInt32(_databaseHelper.GetUserPreferenceKeyId(currentPageName));

            string value = _databaseHelper.GetUserPreferenceValue(userId, keyId);

            return value;
        }
    }
}
