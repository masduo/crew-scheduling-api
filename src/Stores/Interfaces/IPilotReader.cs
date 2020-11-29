using CrewScheduling.Api.Domain.Entities;
using CrewScheduling.Api.Stores.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CrewScheduling.Api.Stores
{
    public interface IPilotReader
    {
        Task<IEnumerable<Pilot>> GetPilots(GetPilotsQuery query);
    }
}
