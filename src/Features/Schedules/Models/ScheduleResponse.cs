using CrewScheduling.Api.Domain.Entities;
using CrewScheduling.Api.Features.Shared.Models;
using System.Text.Json.Serialization;

namespace CrewScheduling.Api.Features.Schedules.Models
{
    public class ScheduleResponse
    {
        /// <summary> The newly created schedule with a guid reference. </summary>
        public Schedule Schedule { get; set; }

        /// <summary> Gets an array of hypermedia links to instruct the caller of next possible functions on the resource. </summary>
        [JsonPropertyName("_links")]
        public Link[] Links
        {
            get => new[]
            {
                new Link { Href = $"/v1/pilots/{Schedule.PilotId}/schedules/{Schedule.Id}", Rel = "self", Method = "GET" }
            };
        }
    }
}