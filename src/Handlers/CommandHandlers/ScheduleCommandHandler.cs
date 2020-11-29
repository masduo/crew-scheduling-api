using CrewScheduling.Api.Domain.Entities;
using CrewScheduling.Api.Handlers.Commands;
using CrewScheduling.Api.Stores;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

namespace CrewScheduling.Api.Handlers.CommandsHandlers
{
    public class ScheduleCommandHandler : IRequestHandler<ScheduleCommand, Schedule>
    {
        private ILogger<ScheduleCommandHandler> _logger;
        private IScheduleWriter _scheduleWriter;

        public ScheduleCommandHandler(ILogger<ScheduleCommandHandler> logger, IScheduleWriter scheduleWriter)
        {
            _logger = logger;
            _scheduleWriter = scheduleWriter;
        }

        public async Task<Schedule> Handle(ScheduleCommand command, CancellationToken cancellationToken)
        {
            return await _scheduleWriter.Schedule(
                command.PilotId,
                command.DepartureDateTimeUtc,
                command.ReturnDateTimeUtc);
        }
    }
}