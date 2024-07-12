using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject.Validation
{
    public class ValidInteger : ValidationAttribute
    {
        private readonly string _id;
        public ValidInteger(string id)
        {
            _id = id;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            //if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
            //{
            //    return ValidationResult.Success;
            //}
            //if (!int.TryParse(value.ToString(), out _))
            //{
            //    return new ValidationResult(GetErrorMessage());
            //}
            //return ValidationResult.Success;
            if (validationContext is null)
                throw new ArgumentNullException(nameof(validationContext));

            var i = value as int?;
            if (i.HasValue && 0 < i)
                return ValidationResult.Success;

            return new ValidationResult(GetErrorMessage());
        }

        public string GetErrorMessage()
        {
            return $"{_id} must be numeric.";
        }
    }
}
