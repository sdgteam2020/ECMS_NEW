using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Validation
{
    public class DestructionDateValidationAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value == null)
                return new ValidationResult("Destruction date is required.");

            if (value is not DateTime destructionDate)
                return new ValidationResult("Invalid date format.");

            // India Standard Time
            var indiaTimeZone = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
            DateTime todayIST = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, indiaTimeZone).Date;

            DateTime oneMonthAhead = todayIST.AddMonths(1);

            // Check past date first
            if (destructionDate.Date < todayIST)
                return new ValidationResult("Destruction date cannot be a past date.");

            // Check more than one month ahead
            if (destructionDate.Date > oneMonthAhead)
                return new ValidationResult("Destruction date cannot be more than one month from the current date.");

            return ValidationResult.Success;
        }
    }
}
