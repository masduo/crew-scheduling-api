using CrewScheduling.Api.Domain.Entities;
using CrewScheduling.Api.Handlers.Commands;
using CrewScheduling.Api.Stores.Entities;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace CrewScheduling.Api.Stores
{
    public class ScheduleWriter : IScheduleWriter
    {
        public const string SCHEDULES_DB_FILE_PATH = "./Data/Schedules.json";

        private ILogger<ScheduleWriter> _logger;

        public ScheduleWriter(ILogger<ScheduleWriter> logger)
        {
            _logger = logger;
        }

        public async Task<Schedule> Schedule(int pilotId, DateTime departureDateTimeUtc, DateTime returnDateTimeUtc)
        {
            var schedulesDb = await ReadSchedulesDbFile();

            var schedules = schedulesDb.Schedules.ToList();

            var newSchedule = new Schedule
            {
                Id = Guid.NewGuid(),
                CreatedAt = DateTime.UtcNow,

                PilotId = pilotId,
                DepartureDateTimeUtc = departureDateTimeUtc,
                ReturnDateTimeUtc = returnDateTimeUtc
            };

            schedules.Add(newSchedule);

            try
            {
                // prone to race condition, also the above read can be dirty
                using var schedulesDbFileStream = File.OpenWrite(SCHEDULES_DB_FILE_PATH);

                var writerOptions = new JsonSerializerOptions
                {
                    WriteIndented = true,
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                };

                await JsonSerializer.SerializeAsync(
                    schedulesDbFileStream,
                    new SchedulesDb { Schedules = schedules },
                    writerOptions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to read or deserialize the {dbFile}", SCHEDULES_DB_FILE_PATH);

                throw;
            }

            return newSchedule;
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
