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
    public class PilotReader : IPilotReader
    {
        public const string CREW_DB_FILE_PATH = "./Data/Pilots.json";

        private ILogger<PilotReader> _logger;

        public PilotReader(ILogger<PilotReader> logger)
        {
            _logger = logger;
        }

        public async Task<IEnumerable<Pilot>> GetPilots(GetPilotsQuery query)
        {
            var db = await ReadPilotsDbFile();

            var pilots = db.Pilots;

            if (query.PilotId.HasValue)
                pilots = pilots.Where(p => p.Id == query.PilotId.Value);

            if (!string.IsNullOrWhiteSpace(query.Base))
                pilots = pilots.Where(p => p.Base?.Equals(query.Base, StringComparison.InvariantCultureIgnoreCase) ?? false);

            return pilots.Where(p =>
                p.WorkingDays.Contains(query.DepartureDay) &&
                p.WorkingDays.Contains(query.ReturnDay));
        }

        public virtual async Task<PilotsDb> ReadPilotsDbFile()
        {
            try
            {
                using var crewDatabaseStream = File.OpenRead(CREW_DB_FILE_PATH);

                var pilotsDb = await JsonSerializer.DeserializeAsync<PilotsDb>(crewDatabaseStream, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                });

                return pilotsDb;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to read or deserialize the {dbFile}", CREW_DB_FILE_PATH);

                throw;
            }
        }
    }
}
