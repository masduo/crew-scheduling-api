using CrewScheduling.Api.Domain.Entities;
using System.Collections.Generic;

namespace CrewScheduling.Api.Stores.Entities
{
    public class PilotsDb
    {
        public IEnumerable<Pilot> Pilots { get; set; }
    }
}
