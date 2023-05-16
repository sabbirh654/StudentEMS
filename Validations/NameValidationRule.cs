using StudentEMS.ViewModels;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace StudentEMS.Validations
{
    public class NameValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            if(string.IsNullOrEmpty(value.ToString()))
            {
                return new ValidationResult(false, "Name can't be empty");
            }
            else
            {
                return ValidationResult.ValidResult;
            }
        }
    }
}
