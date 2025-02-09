using BackgroundJob.Common.Entities;
using BackgroundJob.Common.Models;

namespace BackgroundJob.Common.Repositories;

public interface IFlightRepository
{
    Task<FlightEntity> CreateAsync(FlightCreateInputModel inputModel);
    Task<FlightEntity> UpdateAsync(FlightUpdateInputModel inputModel);
    Task<int> UpdateTaskStatusAsync(string jobId, TaskEntity taskEntity);
}
