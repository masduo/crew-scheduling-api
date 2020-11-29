using CrewScheduling.Api.Domain.Entities;
using MediatR;
using System;

namespace CrewScheduling.Api.Handlers.Queries
{
    /// <summary> The criteria to query the pilot(s) that are available from the requested base and for the requested period. </summary>
    public class AvailabilityQuery : IRequest<Pilot>
    {
        public int? PilotId { get; set; }

        public string Base { get; set; }

        public DateTime DepartureDateTime { get; set; }

        public DateTime ReturnDateTime { get; set; }
    }
}
