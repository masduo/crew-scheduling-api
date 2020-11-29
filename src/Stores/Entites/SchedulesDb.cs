using CrewScheduling.Api.Domain.Entities;
using System.Collections.Generic;

namespace CrewScheduling.Api.Stores.Entities
{
    public class SchedulesDb
    {
        public IEnumerable<Schedule> Schedules { get; set; }
    }
}
