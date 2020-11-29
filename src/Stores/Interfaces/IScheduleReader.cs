using CrewScheduling.Api.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CrewScheduling.Api.Stores
{
    public interface IScheduleReader
    {
        // Task<IEnumerable<Pilot>> GetPilots(string @base, DayOfWeek departureDay, DayOfWeek returnDay);
        Task<IEnumerable<PilotSchedules>> GetPilotSchedules(IEnumerable<Pilot> pilots);
    }
}