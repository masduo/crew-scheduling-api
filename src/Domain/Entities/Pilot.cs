using System;
using System.Collections.Generic;
using System.Linq;

namespace CrewScheduling.Api.Domain.Entities
{
    public class Pilot
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Base { get; set; }

        public IEnumerable<string> WorkDays { get; set; }

        public IEnumerable<DayOfWeek> WorkingDays
        {
            // todo add unit test, e.g. whe data is corrupt? `Momduy`
            get => WorkDays.Select(wd => Enum.Parse<DayOfWeek>(wd, ignoreCase: true));
        }
    }
}
