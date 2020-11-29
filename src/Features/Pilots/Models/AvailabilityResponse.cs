using CrewScheduling.Api.Features.Shared.Models;
using System.Text.Json.Serialization;

namespace CrewScheduling.Api.Features.Pilots.Models
{
    public class AvailabilityResponse
    {
        /// <summary> The next available pilot based on priority. </summary>
        public Pilot Pilot { get; set; }

        /// <summary> The base for departure. The value is equalized case-insensitively.  </summary>
        /// <example> Munich </example>
        public string Base { get; set; }

        /// <summary> The ISO-8601 UTC departure date and time. </summary>
        /// <example> 2020-12-07T09:00Z </example>
        public string DepartureDateTime { get; set; }

        /// <summary> the ISO-8601 UTC rerturn date and time. </summary>
        /// <example> 2020-12-07T11:00:00Z </example>
        public string ReturnDateTime { get; set; }

        /// <summary> Gets an array of hypermedia links to instruct the caller of next possible functions on the resource </summary>
        [JsonPropertyName("_links")]
        public Link[] Links
        {
            get => new[]
            {
                new Link { Href = $"/v1/pilots/{Pilot.Id}", Rel = "self", Method = "GET" },
                new Link { Href = $"/v1/pilots/{Pilot.Id}/schedules", Rel = "schedules", Method = "POST" }
            };
        }
    }
}