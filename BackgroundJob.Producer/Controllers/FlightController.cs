using BackgroundJob.Producer.Mapper;
using BackgroundJob.Common.Models;
using BackgroundJob.Common.Repositories;
using BackgroundJob.Producer.Services;
using Microsoft.AspNetCore.Mvc;

namespace BackgroundJob.Producer.Controllers;

[Route("api/[controller]")]
[ApiController]
public class FlightController : ControllerBase
{
    private readonly IFlightService _flightService;

    public FlightController(IFlightService flightService)
    {
        _flightService = flightService;
    }

    [HttpPost("Create")]
    public async Task<IActionResult> Create([FromBody] FlightCreateInputModel inputModel)
    {
        try
        {
            if (inputModel is null)
            {
                throw new ArgumentNullException(nameof(inputModel));
            }

            var (result, resultQueue) = await _flightService.CreateAsync(inputModel);

            var output = FlightMapper.ToType(result, resultQueue);

            return Ok(output);
        }
        catch (Exception ex)
        {
            return BadRequest(new { Message = ex.Message, Exception = ex, InnerException = ex?.InnerException });
        }
    }
}
