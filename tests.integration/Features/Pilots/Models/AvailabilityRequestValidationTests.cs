using CrewScheduling.Api;
using CrewScheduling.Api.Domain.Entities;
using CrewScheduling.Api.Handlers.Queries;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Tests.Integration.Fakes;
using Xunit;

namespace Tests.Integration.Features.Pilots
{
    [Collection("Features")]
    public class AvailabilityRequestValidationTests : IClassFixture<WebApplicationFactory<Startup>>
    {
        private const string Version1_Pilots_Availability_Url = "/v1/pilots/availability";

        private readonly HttpClient _client;

        public AvailabilityRequestValidationTests(WebApplicationFactory<Startup> factory)
        {
            var fakePilots = new Pilot();

            _client = factory.WithWebHostBuilder(builder =>
                builder.ConfigureTestServices(services =>
                {
                    // inject fake service dependency
                    services.AddTransient<IRequestHandler<AvailabilityQuery, Pilot>>(implementationFactory =>
                        new FakeAvailabitlityQueryHandler(fakePilots));
                }))
                .CreateClient();
        }

        [Fact]
        public async Task Availability_ShouldReturnNotFound_WhenPilotsRootPathIsRequested()
        {
            using var response = await _client.GetAsync("/v1/pilots");

            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Theory]
        [InlineData("")] // empty querystring
        [InlineData("?quite=irrelevant")] // irrelevant querystring
        [InlineData("?base=Munich")]
        [InlineData("?base=Munich&departureDateTime=2020-12-07T09:00Z")]
        [InlineData("?base=Munich&returnDateTime=2020-12-07T11:00Z")]
        [InlineData("?departureDateTime=2020-12-07T09:00Z")]
        [InlineData("?departureDateTime=2020-12-07T09:00Z&returnDateTime=2020-12-07T11:00Z")]
        [InlineData("?returnDateTime=2020-12-07T11:00Z")]
        public async Task Availability_ShouldReturnBadRequest_WhenQueryStringIsMissingOneOrMorePairs(string querystring)
        {
            using var response = await _client.GetAsync($"{Version1_Pilots_Availability_Url}{querystring}");

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task Availability_ShouldReturnBadReqeuest_WhenBaseIsNotSet()
        {
            var queryString = new Dictionary<string, string>
            {
                { "base", string.Empty },
                { "departureDateTime", "2020-12-07T09:00Z" },
                { "returnDateTime", "2020-12-07T11:00Z" }
            };
            var url = QueryHelpers.AddQueryString(Version1_Pilots_Availability_Url, queryString);

            using var response = await _client.GetAsync(url);

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Theory]
        [InlineData("")]
        [InlineData("invalid-date")]
        public async Task Availability_ShouldReturnBadReqeuest_WhenDepartureDateTimeIsInvalid(string departureDateTime)
        {
            var queryString = new Dictionary<string, string>
            {
                { "base", "Munich" },
                { "departureDateTime", departureDateTime },
                { "returnDateTime", "2020-12-07T11:00Z" }
            };
            var url = QueryHelpers.AddQueryString(Version1_Pilots_Availability_Url, queryString);

            using var response = await _client.GetAsync(url);

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Theory]
        [InlineData("")]
        [InlineData("invalid-date")]
        public async Task Availability_ShouldReturnBadReqeuest_WhenReturnDateTimeIsInvalid(string returnDateTime)
        {
            var queryString = new Dictionary<string, string>
            {
                { "base", "Munich" },
                { "departureDateTime", "2020-12-07T09:00Z" },
                { "returnDateTime", returnDateTime }
            };
            var url = QueryHelpers.AddQueryString(Version1_Pilots_Availability_Url, queryString);

            using var response = await _client.GetAsync(url);

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task Availability_ShouldReturnNotFound_WhenQueryStringIsValid()
        {
            var queryString = new Dictionary<string, string>
            {
                { "base", "Munich" },
                { "departureDateTime", "2020-12-07T09:00Z" },
                { "returnDateTime", "2020-12-07T11:00Z" }
            };
            var url = QueryHelpers.AddQueryString(Version1_Pilots_Availability_Url, queryString);

            using var response = await _client.GetAsync(url);

            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }
    }
}
