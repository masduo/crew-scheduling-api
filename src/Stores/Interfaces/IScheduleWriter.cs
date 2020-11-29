using CrewScheduling.Api.Domain.Entities;
using CrewScheduling.Api.Handlers.Commands;
using System;
using System.Threading.Tasks;

namespace CrewScheduling.Api.Stores
{
    public interface IScheduleWriter
    {
        Task<Schedule> Schedule(int pilotId, DateTime departureDateTimeUtc, DateTime returnDateTimeUtc);
    }
}