namespace StudentEMS.Services.Interfaces
{
    public interface IUserPreferenceService
    {
        bool IsUserPreferenceExists(string userName, string currentPageName);
        void UpdateUserPreferences(string userName, string currentPageName, string value);
        void InsertUserPreferences(string userName, string currentPageName, string value);
        string GetUserPreferenceValue(string userName, string currentPageName);
    }
}
