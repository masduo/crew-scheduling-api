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
    public class PilotReaderTests
    {
        private const string MUNICH = "Munich";

        private GetPilotsQuery _munichMondayToMondayQuery = new GetPilotsQuery
        {
            Base = MUNICH,
            DepartureDay = DayOfWeek.Monday,
            ReturnDay = DayOfWeek.Monday
        };

        private Mock<ILogger<PilotReader>> _loggerMock = new Mock<ILogger<PilotReader>>();

        [Fact]
        public void GetPilots_ShouldThrowNullReferenceException_WhenPilotsDbIsNull()
        {
            var pilotDb = (PilotsDb)null;

            var pilotReader = new Mock<PilotReader>(_loggerMock.Object);
            pilotReader.CallBase = true;
            pilotReader.Setup(m => m.ReadPilotsDbFile()).ReturnsAsync(pilotDb);

            // Act
            Func<Task<IEnumerable<Pilot>>> act =
                async () => await pilotReader.Object.GetPilots(_munichMondayToMondayQuery);

            act.Should().Throw<NullReferenceException>();
        }

        [Fact]
        public async Task GetPilots_ShouldReturnEmptyArray_WhenPilotsIsEmpty()
        {
            var pilots = new List<Pilot>();

            var pilotReader = new Mock<PilotReader>(_loggerMock.Object);
            pilotReader.CallBase = true;
            pilotReader.Setup(m => m.ReadPilotsDbFile()).ReturnsAsync(new PilotsDb { Pilots = pilots });

            // Act
            var workingPilots = await pilotReader.Object.GetPilots(_munichMondayToMondayQuery);

            workingPilots.Should().BeEmpty();
        }

        [Fact]
        public async Task GetPilots_ShouldReturnEmptyArray_WhenBaseIsNotSet()
        {
            var pilots = new List<Pilot>
            {
                // base is not set
                new Pilot { Id = 1, Name = "Andy", WorkDays = new[] { "Monday" } }
            };

            var pilotReader = new Mock<PilotReader>(_loggerMock.Object);
            pilotReader.CallBase = true;
            pilotReader.Setup(m => m.ReadPilotsDbFile()).ReturnsAsync(new PilotsDb { Pilots = pilots });

            // Act
            var workingPilots = await pilotReader.Object.GetPilots(_munichMondayToMondayQuery);

            workingPilots.Should().BeEmpty();
        }

        [Fact]
        public async Task GetPilots_ShouldTreatBaseCaseInsensitively()
        {
            var pilots = new List<Pilot>
            {
                new Pilot { Id = 1, Base = MUNICH, WorkDays = new[] { "Monday" } }
            };

            var pilotReader = new Mock<PilotReader>(_loggerMock.Object);
            pilotReader.CallBase = true;
            pilotReader.Setup(m => m.ReadPilotsDbFile()).ReturnsAsync(new PilotsDb { Pilots = pilots });

            // Act
            var sameCase = await pilotReader.Object.GetPilots(new GetPilotsQuery { Base = MUNICH, DepartureDay = DayOfWeek.Monday, ReturnDay = DayOfWeek.Monday });
            var upperCase = await pilotReader.Object.GetPilots(new GetPilotsQuery { Base = "MUNICH", DepartureDay = DayOfWeek.Monday, ReturnDay = DayOfWeek.Monday });
            var lowerCase = await pilotReader.Object.GetPilots(new GetPilotsQuery { Base = "munich", DepartureDay = DayOfWeek.Monday, ReturnDay = DayOfWeek.Monday });
            var mixedCase = await pilotReader.Object.GetPilots(new GetPilotsQuery { Base = "MuNiCh", DepartureDay = DayOfWeek.Monday, ReturnDay = DayOfWeek.Monday });

            sameCase.Should().HaveCount(1);
            upperCase.Should().HaveCount(1);
            lowerCase.Should().HaveCount(1);
            mixedCase.Should().HaveCount(1);
        }

        [Theory]
        [InlineData(MUNICH, MUNICH, 2)]
        [InlineData(MUNICH, "Zurich", 1)]
        [InlineData("Berlin", "Zurich", 0)]
        public async Task GetPilots_ShouldReturnExpectdPilotsCount_WhenPilotAreFromTheRequestedBase(string firstPilotBase, string secondPilotBase, int expectedPilotCount)
        {
            var pilots = new List<Pilot>
            {
                new Pilot { Id = 1, Name = "Andy", Base = firstPilotBase, WorkDays = new[] { "Monday" } },
                new Pilot { Id = 2, Name = "Greg", Base = secondPilotBase, WorkDays = new[] { "Monday" } }
            };

            var pilotReader = new Mock<PilotReader>(_loggerMock.Object);
            pilotReader.CallBase = true;
            pilotReader.Setup(m => m.ReadPilotsDbFile()).ReturnsAsync(new PilotsDb { Pilots = pilots });

            // Act
            var workingPilots = await pilotReader.Object.GetPilots(_munichMondayToMondayQuery);

            workingPilots.Should().HaveCount(expectedPilotCount);
        }

        [Theory]
        // same day
        [InlineData(DayOfWeek.Monday, DayOfWeek.Monday, 2, new[] { 1, 4 })]
        [InlineData(DayOfWeek.Sunday, DayOfWeek.Sunday, 3, new[] { 2, 4, 5 })]
        [InlineData(DayOfWeek.Friday, DayOfWeek.Friday, 2, new[] { 3, 5 })]

        // different days
        [InlineData(DayOfWeek.Monday, DayOfWeek.Sunday, 1, new[] { 4 })]
        [InlineData(DayOfWeek.Sunday, DayOfWeek.Monday, 1, new[] { 4 })]

        [InlineData(DayOfWeek.Friday, DayOfWeek.Sunday, 1, new[] { 5 })]
        [InlineData(DayOfWeek.Sunday, DayOfWeek.Friday, 1, new[] { 5 })]
        public async Task GetPilots_ShouldReturnPilotsWhoWorkOnBothDepartureAndReturnDay(
            DayOfWeek day1, DayOfWeek day2, int expectedPilotCount, int[] expectedPilotIds)
        {
            var pilots = new List<Pilot>
            {
                // base is not set
                new Pilot { Id = 1, Base = MUNICH, WorkDays = new[] { "Monday" } },
                new Pilot { Id = 2, Base = MUNICH, WorkDays = new[] { "Sunday" } },
                new Pilot { Id = 3, Base = MUNICH, WorkDays = new[] { "Friday" } },
                new Pilot { Id = 4, Base = MUNICH, WorkDays = new[] { "Monday", "Sunday" } },
                new Pilot { Id = 5, Base = MUNICH, WorkDays = new[] { "Friday", "Sunday" } }
            };

            var pilotReader = new Mock<PilotReader>(_loggerMock.Object);
            pilotReader.CallBase = true;
            pilotReader.Setup(m => m.ReadPilotsDbFile()).ReturnsAsync(new PilotsDb { Pilots = pilots });

            // Act
            var departOnDay1ReturnOnDay2 = await pilotReader.Object.GetPilots(new GetPilotsQuery { Base = MUNICH, DepartureDay = day1, ReturnDay = day2 });
            var departOnDay2ReturnOnDay1 = await pilotReader.Object.GetPilots(new GetPilotsQuery { Base = MUNICH, DepartureDay = day2, ReturnDay = day1 });

            departOnDay1ReturnOnDay2.Select(p => p.Id).Should().HaveCount(expectedPilotCount).And.Contain(expectedPilotIds);
            departOnDay2ReturnOnDay1.Select(p => p.Id).Should().HaveCount(expectedPilotCount).And.Contain(expectedPilotIds);
        }
    }
}
