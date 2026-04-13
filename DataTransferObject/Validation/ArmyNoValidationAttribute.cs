using DataTransferObject.Localize;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DataTransferObject.Validation
{
    public class ArmyNoValidationAttribute : ValidationAttribute
    {
    private static readonly HashSet<string> ValidPrefixes = new HashSet<string>
    {
        "IC", "SL", "SS", "WC", "TA", "JC"
    };

        public ArmyNoValidationAttribute()
        {
            ErrorMessage = "Enter valid Army No.";
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            string? armyNo = value as string;

            if (string.IsNullOrWhiteSpace(armyNo))
                return ValidationResult.Success;

            armyNo = armyNo.Trim().ToUpper();

            if (armyNo.Length < 8 || armyNo.Length > 9)
                return new ValidationResult(ErrorMessage);

            if (!Regex.IsMatch(armyNo, @"^[A-Z]{2}\d{5,6}[A-Z]$"))
                return new ValidationResult(ErrorMessage);

            string prefix = armyNo.Substring(0, 2);
            if (!ValidPrefixes.Contains(prefix))
                return new ValidationResult(ErrorMessage);

            string numericPart = Regex.Replace(armyNo, "[A-Za-z]", "");
            string actualSuffix = armyNo[^1].ToString();

            int length = numericPart.Length;
            int multiplier = length + 1;
            int sum = 0;

            for (int i = 0; i < length; i++)
            {
                int digit = int.Parse(numericPart[i].ToString());
                sum += digit * multiplier;
                multiplier--;
            }

            int remainder = sum % 11;

            string expectedSuffix = remainder switch
            {
                0 => "A",
                1 => "F",
                2 => "H",
                3 => "K",
                4 => "L",
                5 => "M",
                6 => "N",
                7 => "P",
                8 => "W",
                9 => "X",
                10 => "Y",
                _ => ""
            };

            return actualSuffix == expectedSuffix
                ? ValidationResult.Success
                : new ValidationResult(ErrorMessage);
        }
    }
}
