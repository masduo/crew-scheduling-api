using CrewScheduling.Api.Domain.Entities;
using CrewScheduling.Api.Handlers.Commands;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CrewScheduling.Api.Stores.Interfaces
{
    public interface IQuerySchedules
    {
        Task<IEnumerable<Schedule>> QuerySchedules();

        Task<IEnumerable<Schedule>> QuerySchedules(int[] pilotIds);

        Task<bool> Schedule(ScheduleCommand command);
    }
}
