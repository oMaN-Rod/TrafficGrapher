using System;
using System.Globalization;
using System.Net;
using System.Windows.Controls;

namespace TrafficGrapher.ValidationRules
{
    public class HostValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            var val = (string) value;
            if (string.IsNullOrEmpty(val)) return new ValidationResult(false, "Value cannot be null");
            try
            {
                IPAddress.Parse(val);
            }
            catch (Exception ex)
            {
                return new ValidationResult(false, ex.Message);
            }
            return ValidationResult.ValidResult;
        }
    }
}
