using CrewScheduling.Api.Domain.Entities;
using CrewScheduling.Api.Handlers.Queries;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Tests.Integration.Fakes
{
    public class FakeAvailabitlityQueryHandler : IRequestHandler<AvailabilityQuery, Pilot>
    {
        private Pilot _fakePilots;

        public FakeAvailabitlityQueryHandler(Pilot fakePilots) =>
            _fakePilots = fakePilots;

        public async Task<Pilot> Handle(AvailabilityQuery request, CancellationToken cancellationToken) =>
            await Task.FromResult(_fakePilots);
    }
}
