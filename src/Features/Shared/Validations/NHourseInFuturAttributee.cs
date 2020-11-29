using System;
using System.ComponentModel.DataAnnotations;

namespace CrewScheduling.Api.Features.Shared.Validation
{
    public class NHoursInFutureAttribute : ValidationAttribute
    {
        private readonly int _n;

        public NHoursInFutureAttribute(int n) =>
            _n = n;

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var date = (DateTime?)value;

            return date.HasValue && date.Value < DateTime.UtcNow.AddHours(_n)
                ? new ValidationResult($"{validationContext.DisplayName} value must be at least {_n} hour(s) in the future.")
                : ValidationResult.Success;
        }
    }
}
