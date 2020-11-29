using System;

namespace CrewScheduling.Api.Domain.Entities
{
    public class Schedule
    {
        /// <summary> The Id of the scheduled flight. </summary>
        public Guid Id { get; set; }

        /// <summary> The pilot that will do the flight </summary>
        public int PilotId { get; set; }

        /// <summary> The UTC departure time of the flight. </summary>
        public DateTime DepartureDateTimeUtc { get; set; }

        /// <summary> The UTC return time of the flight. </summary>
        public DateTime ReturnDateTimeUtc { get; set; }

        /// <summary> The UTC date and time when the flight is schedule in the system. </summary>
        public DateTime CreatedAt { get; set; }
    }
}
