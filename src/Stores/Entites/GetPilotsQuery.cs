using System;

namespace CrewScheduling.Api.Stores.Entities
{
    public class GetPilotsQuery
    {
        public int? PilotId { get; set; }

        public string Base { get; set; }

        public DayOfWeek DepartureDay { get; set; }

        public DayOfWeek ReturnDay { get; set; }
    }
}