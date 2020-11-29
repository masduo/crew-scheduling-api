using CrewScheduling.Api.Domain.Entities;
using System.Threading.Tasks;

namespace CrewScheduling.Api.Stores.Interfaces
{
    public interface IPilotQueries
    {
        Task<Pilot> QueryPilot(int pilotId);
    }
}
