using StudentEMS.Models;

namespace StudentEMS.Services.Interfaces
{
    public interface ILoginServices
    {
        bool IsStudentExist(string email, string password);
        void StoreCredentials(string userName, string password, string userRole);
        User GetUserCredentials();
        void RemoveCredentials();
        bool IsRememberingUser();
    }
}
