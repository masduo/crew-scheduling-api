using CrewScheduling.Api.Handlers.Commands;
using System.Threading.Tasks;

namespace CrewScheduling.Api.Stores.Interfaces
{
    public interface ICommandSchedules
    {
        Task<bool> Schedule(ScheduleCommand command);
    }
}
