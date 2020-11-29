using CrewScheduling.Api.Features.Schedules.Models;
using CrewScheduling.Api.Handlers.Commands;
using CrewScheduling.Api.Handlers.Queries;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;
using System.Net.Mime;
using System.Threading.Tasks;

namespace CrewScheduling.Api.Features.Schedules
{
    [ApiController]
    public class PilotSchedulesController : ControllerBase
    {
        private readonly ILogger<PilotSchedulesController> _logger;
        private readonly IMediator _mediator;

        public PilotSchedulesController(ILogger<PilotSchedulesController> logger, IMediator mediator)
        {
            _logger = logger;
            _mediator = mediator;
        }

        /// <summary> Schedules the requested pilots for the requested period. </summary>
        /// <param name="id" example="1"> The Id of the pilot being scheduled. </param>
        /// <param name="request"> The request payload containing departure and return dates. </param>
        [HttpPost]
        [ApiVersion(Startup.DefaultApiVersion)]
        [Route("/v{version:apiVersion}/pilots/{id}/schedules")]
        [Consumes(MediaTypeNames.Application.Json)]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(ScheduleResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        public async Task<IActionResult> Schedule(
            [FromRoute, BindRequired] int id,
            [FromBody, BindRequired] ScheduleRequest request)
        {
            var pilot = await _mediator.Send(new AvailabilityQuery
            {
                PilotId = id,
                DepartureDateTime = request.DepartureDateTime.Value,
                ReturnDateTime = request.ReturnDateTime.Value
            });

            if (pilot == default)
                return UnprocessableEntity("Requested pilot is not available for the requested period");

            var schedule = await _mediator.Send(new ScheduleCommand
            {
                PilotId = id,
                DepartureDateTimeUtc = request.DepartureDateTime.Value.ToUniversalTime(),
                ReturnDateTimeUtc = request.ReturnDateTime.Value.ToUniversalTime()
            });

            return Created($"{Request.Path}/{schedule.Id}", new ScheduleResponse
            {
                Schedule = schedule
            });
        }
    }
}
