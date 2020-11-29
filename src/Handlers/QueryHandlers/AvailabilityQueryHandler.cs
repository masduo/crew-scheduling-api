using CrewScheduling.Api.Domain.Entities;
using CrewScheduling.Api.Handlers.Queries;
using CrewScheduling.Api.Stores;
using CrewScheduling.Api.Stores.Entities;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CrewScheduling.Api.Handlers.QueryHandlers
{
    public class AvailabilityQueryHandler : IRequestHandler<AvailabilityQuery, Pilot>
    {
        private readonly ILogger<AvailabilityQueryHandler> _logger;
        private readonly IPilotReader _pilotReader;
        private readonly IScheduleReader _scheduleReader;

        public AvailabilityQueryHandler(
            ILogger<AvailabilityQueryHandler> logger,
            IPilotReader pilotReader,
            IScheduleReader scheduleReader)
        {
            _logger = logger;
            _pilotReader = pilotReader;
            _scheduleReader = scheduleReader;
        }

        public async Task<Pilot> Handle(AvailabilityQuery query, CancellationToken cancellationToken)
        {
            var pilots = await _pilotReader.GetPilots(new GetPilotsQuery
            {
                PilotId = query.PilotId,
                Base = query.Base,
                DepartureDay = query.DepartureDateTime.DayOfWeek,
                ReturnDay = query.ReturnDateTime.DayOfWeek
            });

            if (!pilots.Any())
            {
                _logger.LogInformation("The {@query} yielded no pilots", query);
                return default;
            }

            var pilotSchedules = await _scheduleReader.GetPilotSchedules(pilots);

            // exempt busy pilots and order by their current schedules' count
            var availablePilots = pilotSchedules
                .Where(ps => !ps.Schedules.Any(s =>
                    query.DepartureDateTime < s.ReturnDateTimeUtc &&
                    query.ReturnDateTime > s.DepartureDateTimeUtc))
                .OrderBy(fp => fp.Schedules.Count())
                .Select(ps => ps.Pilot);

            if (!availablePilots.Any())
            {
                _logger.LogInformation("{@query} yielded no free pilots", query);
                return default;
            }

            return availablePilots.First();
        }
    }
}