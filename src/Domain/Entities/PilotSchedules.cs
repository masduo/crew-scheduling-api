using System.Collections.Generic;

namespace CrewScheduling.Api.Domain.Entities
{
    public class PilotSchedules
    {
        public Pilot Pilot { get; set; }

        public IEnumerable<Schedule> Schedules { get; set; }
    }
}
