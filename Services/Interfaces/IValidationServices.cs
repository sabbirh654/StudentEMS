namespace StudentEMS.Interfaces
{
    public interface IValidationServices
    {
        bool IsValidName(string name);
        bool IsValidEmail(string email);
        bool IsValidPassword(string password);
        bool IsValidCode(string id);
        bool IsValidGrade(string grade);
    }
}
