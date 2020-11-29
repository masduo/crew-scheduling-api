using CrewScheduling.Api.Domain.Entities;
using CrewScheduling.Api.Stores.Entities;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace CrewScheduling.Api.Stores
{
    public class ScheduleReader : IScheduleReader
    {
        public const string SCHEDULES_DB_FILE_PATH = "./Data/Schedules.json";

        private ILogger<ScheduleReader> _logger;

        public ScheduleReader(ILogger<ScheduleReader> logger)
        {
            _logger = logger;
        }

        public async Task<IEnumerable<PilotSchedules>> GetPilotSchedules(IEnumerable<Pilot> pilots)
        {
            var db = await ReadSchedulesDbFile();

            // join pilots to their schedules
            return from pilot in pilots
                   join schedule in db.Schedules
                   on pilot.Id equals schedule.PilotId into schedules
                   select new PilotSchedules { Pilot = pilot, Schedules = schedules };
        }

        public virtual async Task<SchedulesDb> ReadSchedulesDbFile()
        {
            try
            {
                using var schedulesDbFileStream = File.OpenRead(SCHEDULES_DB_FILE_PATH);

                var schedulesDb = await JsonSerializer.DeserializeAsync<SchedulesDb>(schedulesDbFileStream, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                });

                return schedulesDb;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to read or deserialize the {dbFile}", SCHEDULES_DB_FILE_PATH);

                throw;
            }
        }
    }
}
