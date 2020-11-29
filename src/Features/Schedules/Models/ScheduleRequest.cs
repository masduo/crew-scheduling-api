using CrewScheduling.Api.Features.Shared.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CrewScheduling.Api.Features.Schedules.Models
{
    public class ScheduleRequest : IValidatableObject
    {
        /// <summary> The ISO-8601 UTC departure date and time. Times must be on the hour. </summary>
        /// <example> 2020-12-07T09:00Z </example>
        [Required]
        [OnTheHour]
        [NHoursInFuture(1)]
        public DateTime? DepartureDateTime { get; set; }

        /// <summary> The ISO-8601 UTC rerturn date and time. Times must be on the hour. </summary>
        /// <example> 2020-12-07T11:00:00Z </example>
        [Required]
        [OnTheHour]
        [NHoursInFuture(2)]
        public DateTime? ReturnDateTime { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (ReturnDateTime.Value.Subtract(DepartureDateTime.Value).TotalHours < 1)
            {
                yield return new ValidationResult($"{nameof(ReturnDateTime)} must be at least 1 hour after {nameof(DepartureDateTime)}.",
                    new[] { nameof(ReturnDateTime) });
            }
        }
    }
}
