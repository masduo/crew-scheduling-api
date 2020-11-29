using CrewScheduling.Api.Features.Pilots.Models;
using CrewScheduling.Api.Handlers.Queries;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;
using System.Net.Mime;
using System.Threading.Tasks;

namespace CrewScheduling.Api.Features.Pilots
{
    [ApiController]
    [Produces(MediaTypeNames.Application.Json)]
    public class PilotsController : ControllerBase
    {
        private readonly ILogger<PilotsController> _logger;
        private readonly IMediator _mediator;

        public PilotsController(ILogger<PilotsController> logger, IMediator mediator)
        {
            _logger = logger;
            _mediator = mediator;
        }

        /// <summary> Gets the next avilable pilot from the requested base and for the given period. </summary>
        /// <param name="request"> The request querystring parameters. </param>
        [HttpGet]
        [ApiVersion(Startup.DefaultApiVersion)]
        [Route("/v{version:apiVersion}/pilots/availability")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AvailabilityResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Availability([FromQuery, BindRequired] AvailabilityReqeust request)
        {
            var pilot = await _mediator.Send(new AvailabilityQuery
            {
                Base = request.Base,
                DepartureDateTime = request.DepartureDateTime.Value,
                ReturnDateTime = request.ReturnDateTime.Value
            });

            if (pilot == default)
                return NotFound("No pilots were available for the requested base and period.");

            var response = new AvailabilityResponse
            {
                Pilot = new Pilot { Id = pilot.Id, Name = pilot.Name },
                Base = pilot.Base,
                DepartureDateTime = request.DepartureDateTime.Value.ToString("dddd, MMMM dd, yyyy h:mm tt"),
                ReturnDateTime = request.ReturnDateTime.Value.ToString("dddd, MMMM dd, yyyy h:mm tt")
            };

            return Ok(response);
        }
    }
}
