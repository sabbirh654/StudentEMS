using StudentEMS.Interfaces;

using System.Text.RegularExpressions;

namespace StudentEMS.Services
{
    public class ValidationServices : IValidationServices
    {
        public bool IsValidName(string name)
        {
            return !(string.IsNullOrEmpty(name));
        }

        public bool IsValidEmail(string email)
        {
            Regex regex = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){3})+)$");
            Match match = regex.Match(email);
            if (match.Success)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool IsValidPassword(string password)
        {
            return !(password.Length < 6);
        }

        public bool IsValidCode(string id)
        {
            bool ok = int.TryParse(id, out int result);
            return ok;
        }

        public bool IsValidGrade(string grade)
        {
            if(string.IsNullOrEmpty(grade))
            {
                return false;
            }

            return double.TryParse(grade, out double result) && result > 0.0 && result <= 4.00;
        }
    }
}
