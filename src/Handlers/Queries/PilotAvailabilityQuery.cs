using System;

namespace CrewScheduling.Api.Handlers.Queries
{
    /// <summary> The criteria to query whether the requested pilot is available for the given period. </summary>
    public class PilotAvailabilityQuery
    {
        public int PilotId { get; set; }

        public DateTime DepartureDateTime { get; set; }

        public DateTime ReturnDateTime { get; set; }
    }
}
