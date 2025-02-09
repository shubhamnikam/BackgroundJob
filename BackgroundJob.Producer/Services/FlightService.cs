using BackgroundJob.Common.Entities;
using BackgroundJob.Common.Models;
using BackgroundJob.Common.Repositories;
using Microsoft.EntityFrameworkCore;

namespace BackgroundJob.Producer.Services;

public class FlightService : IFlightService
{
    private readonly SqlDbContext _context;
    private readonly IFlightQueueRepository _flightQueueRepository;
    private readonly IFlightRepository _flightRepository;

    public FlightService(SqlDbContext context, IFlightQueueRepository flightQueueRepository, IFlightRepository flightRepository)
    {
        _context = context;
        _flightRepository = flightRepository;
        _flightQueueRepository = flightQueueRepository;
    }

    public async Task<(FlightEntity, FlightQueueEntity)> CreateAsync(FlightCreateInputModel inputModel)
    {
        return await TransactionHelper.ExecuteAsync(_context, async () =>
        {
            var result = await _flightRepository.CreateAsync(inputModel);
            var resultQueue = await _flightQueueRepository.CreateAsync(result.Id);

            return (result, resultQueue);
        });
    }

    public async Task<FlightEntity> UpdatePriceAsync(FlightUpdateInputModel inputModel)
    {
        return await TransactionHelper.ExecuteAsync(_context, async () =>
        {
            var result = await _flightRepository.UpdateAsync(inputModel);

            return result;
        });
    }
}
