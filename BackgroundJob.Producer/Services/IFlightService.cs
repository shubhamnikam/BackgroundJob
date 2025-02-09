using BackgroundJob.Common.Entities;
using BackgroundJob.Common.Models;

namespace BackgroundJob.Producer.Services;

public interface IFlightService
{
    Task<(FlightEntity, FlightQueueEntity)> CreateAsync(FlightCreateInputModel inputModel);
    Task<FlightEntity> UpdatePriceAsync(FlightUpdateInputModel inputModel);
}
