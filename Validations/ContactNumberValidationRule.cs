using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows.Controls;

namespace StudentEMS.Validations
{
    public class ContactNumberValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            if (string.IsNullOrEmpty(value.ToString()) || !Regex.IsMatch(value.ToString(), @"^\d+$"))
            {
                return new ValidationResult(false, "Contact Number can only contains digit only");
            }
            else
            {
                return ValidationResult.ValidResult;
            }
        }
    }
}
