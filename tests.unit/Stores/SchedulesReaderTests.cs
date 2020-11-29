using CrewScheduling.Api.Domain.Entities;
using CrewScheduling.Api.Stores;
using CrewScheduling.Api.Stores.Entities;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Tests.Unit.Services
{
    [Collection("Stores")]
    public class ScheduleReaderTests
    {
        private Mock<ILogger<ScheduleReader>> _loggerMock = new Mock<ILogger<ScheduleReader>>();

        [Fact]
        public void GetSchedules_ShouldThrowArgumentNullException_WhenPilotsIsNull()
        {
            var pilots = (IEnumerable<Pilot>)null;

            var scheduleReader = new Mock<ScheduleReader>(_loggerMock.Object);
            scheduleReader.CallBase = true;
            scheduleReader.Setup(m => m.ReadSchedulesDbFile()).ReturnsAsync(new SchedulesDb());

            // Act
            Func<Task<IEnumerable<PilotSchedules>>> act =
                async () => await scheduleReader.Object.GetPilotSchedules(pilots);

            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void QueryPilotSchedules_ShouldThrowArgumentException_WhenPilotsIsEmpty()
        {
            var pilots = new List<Pilot>();

            var scheduleReader = new Mock<ScheduleReader>(_loggerMock.Object);
            scheduleReader.CallBase = true;
            scheduleReader.Setup(m => m.ReadSchedulesDbFile()).ReturnsAsync(new SchedulesDb());

            // Act
            Func<Task<IEnumerable<PilotSchedules>>> act =
                async () => await scheduleReader.Object.GetPilotSchedules(pilots);

            act.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void GetSchedules_ShouldThrowNullReferenceException_WhenSchedulesIsNull()
        {
            var schedulesDb = (SchedulesDb)null;
            var pilots = new List<Pilot> { new Pilot() };

            var scheduleReader = new Mock<ScheduleReader>(_loggerMock.Object);
            scheduleReader.CallBase = true;
            scheduleReader.Setup(m => m.ReadSchedulesDbFile()).ReturnsAsync(schedulesDb);

            // Act
            Func<Task<IEnumerable<PilotSchedules>>> act =
                async () => await scheduleReader.Object.GetPilotSchedules(pilots);

            act.Should().Throw<NullReferenceException>();
        }

        [Fact]
        public async Task GetSchedules_ShouldReturnThePilot_WhenPilotsIsNotEmpty()
        {
            var schedules = new List<Schedule>();
            var pilots = new List<Pilot>() { new Pilot { Id = 123 } };

            var scheduleReader = new Mock<ScheduleReader>(_loggerMock.Object);
            scheduleReader.CallBase = true;
            scheduleReader.Setup(m => m.ReadSchedulesDbFile()).ReturnsAsync(new SchedulesDb { Schedules = schedules });

            // Act
            var pilotSchedules = await scheduleReader.Object.GetPilotSchedules(pilots);

            pilotSchedules.Should().HaveCount(pilots.Count());
            pilotSchedules.First().Pilot.Id.Should().Be(123);
        }

        [Fact]
        public async Task QueryPilotSchedules_ShouldJoinPilotsToTheirSchedule()
        {
            var pilots = new List<Pilot>()
            {
                new Pilot { Id = 0 },
                new Pilot { Id = 1 },
                new Pilot { Id = 2 },
                new Pilot { Id = 3 },
            };

            var schedules = new List<Schedule>
            {
                new Schedule { PilotId = 1 },

                new Schedule { PilotId = 2 },
                new Schedule { PilotId = 2 },

                new Schedule { PilotId = 3 },
                new Schedule { PilotId = 3 },
                new Schedule { PilotId = 3 },

                // other pilots' schedules
                new Schedule { PilotId = 123 },
                new Schedule { PilotId = 234 },
                new Schedule { PilotId = 345 },
            };

            var scheduleReader = new Mock<ScheduleReader>(_loggerMock.Object);
            scheduleReader.CallBase = true;
            scheduleReader.Setup(m => m.ReadSchedulesDbFile()).ReturnsAsync(new SchedulesDb() { Schedules = schedules });

            // Act
            var pilotSchedules = await scheduleReader.Object.GetPilotSchedules(pilots);

            // one pilot-schedule per inuput pilot
            pilotSchedules.Should().HaveCount(pilots.Count());
            pilotSchedules.Where(ps => ps.Pilot.Id == 0).Should().HaveCount(1);
            pilotSchedules.Where(ps => ps.Pilot.Id == 1).Should().HaveCount(1);
            pilotSchedules.Where(ps => ps.Pilot.Id == 2).Should().HaveCount(1);
            pilotSchedules.Where(ps => ps.Pilot.Id == 3).Should().HaveCount(1);

            // schedules for each pilot is joined accordingly
            pilotSchedules.Where(ps => ps.Pilot.Id == 0).First().Schedules.Should().HaveCount(0);
            pilotSchedules.Where(ps => ps.Pilot.Id == 1).First().Schedules.Should().HaveCount(1);
            pilotSchedules.Where(ps => ps.Pilot.Id == 2).First().Schedules.Should().HaveCount(2);
            pilotSchedules.Where(ps => ps.Pilot.Id == 3).First().Schedules.Should().HaveCount(3);

            // other pilots' schedules should not be joined
            pilotSchedules.SelectMany(ps => ps.Schedules).Should().HaveCount(6);
            pilotSchedules.SelectMany(ps => ps.Schedules)
                .Select(s => s.PilotId).Should().NotContain(new[] { 123, 234, 345 });
        }
    }
}