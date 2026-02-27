using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Validation
{
    public class LostDateValidationAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value == null)
                return new ValidationResult("Lost date is required.");

            if (value is not DateTime lostDate)
                return new ValidationResult("Invalid date format.");

            // India Standard Time
            var indiaTimeZone = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
            DateTime todayIST = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, indiaTimeZone);

            DateTime oneMonthBack = todayIST.AddMonths(-1);

            if (lostDate > todayIST)
                return new ValidationResult("Lost date cannot be in the future.");

            if (lostDate < oneMonthBack)
                return new ValidationResult("Lost date cannot be older than one month.");

            return ValidationResult.Success;
        }
    }
}
