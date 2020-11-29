using System;
using System.ComponentModel.DataAnnotations;

namespace CrewScheduling.Api.Features.Shared.Validation
{
    public class OnTheHourAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var dateTime = (DateTime?)value;

            if (dateTime.HasValue && (
                dateTime.Value.Minute > 0 ||
                dateTime.Value.Second > 0 ||
                dateTime.Value.Millisecond > 0))
            {
                return new ValidationResult($"{validationContext.DisplayName} value must be on the hour.");
            }

            return ValidationResult.Success;
        }
    }
}
