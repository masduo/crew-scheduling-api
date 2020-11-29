using CrewScheduling.Api.Domain.Entities;
using MediatR;
using System;

namespace CrewScheduling.Api.Handlers.Commands
{
    /// <summary> The criteria to trigger a schedule command. </summary>
    public class ScheduleCommand : IRequest<Schedule>
    {
        public int PilotId { get; set; }

        public DateTime DepartureDateTimeUtc { get; set; }

        public DateTime ReturnDateTimeUtc { get; set; }
    }
}
