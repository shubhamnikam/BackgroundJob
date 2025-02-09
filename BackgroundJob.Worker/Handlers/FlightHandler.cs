using BackgroundJob.Common.Constants;
using BackgroundJob.Common.Entities;
using BackgroundJob.Common.Models;
using BackgroundJob.Common.Repositories;
using BackgroundJob.Common.Services;
using BackgroundJob.Producer.Services;
using MongoDB.Driver;
using System.Data;

namespace BackgroundJob.Worker.Handlers;

public class FlightHandler : IHandler
{
    private readonly IConfiguration _configuration;
    private readonly IFlightService _flightService;
    private readonly IFlightRepository _flightRepository;

    public FlightHandler(
        IConfiguration configuration,
        IMongoDbContext mongoDbContext,
        IFlightService flightService,
        IFlightRepository flightRepository)
    {
        _configuration = configuration;
        _flightService = flightService;
        _flightRepository = flightRepository;
    }

    public ObjectTypeEnum ObjectType => ObjectTypeEnum.Flight;
    public async Task<bool> ProcessAsync(string jobId, TaskEntity taskEntity)
    {
        bool isSuccess = false;
        try
        {
            // skip task if final error else take all
            if (taskEntity.Status.IsWorkerFinalFailed || taskEntity.Status.WorkerRetryCount >= 5)
            {
                return true;
            }

            taskEntity.Status.WorkerRetryCount = ++taskEntity.Status.WorkerRetryCount;

            // call appropriate service from specific producer todo extra processing
            var flightUpdateInputModel = new FlightUpdateInputModel()
            {
                ObjectId = taskEntity.ObjectId,
                ObjectTypeId = taskEntity.ObjectTypeId,
                TaxPrice = 18
            };
            var flightEntity = await _flightService.UpdatePriceAsync(flightUpdateInputModel);

            if (flightEntity is null)
            {
                throw new Exception($"Not found objectId: {flightUpdateInputModel.ObjectId}");
            }

            // update status for task
            taskEntity.Status.IsWorkerSucceesed = true;
            taskEntity.Status.IsWorkerFailed = false;
            taskEntity.Status.IsWorkerFinalFailed = false;

            var result = await _flightRepository.UpdateTaskStatusAsync(jobId, taskEntity);
            isSuccess = true;
        }
        catch (Exception ex)
        {
            isSuccess = false;

            // update status for task
            taskEntity.Status.IsWorkerSucceesed = false;
            taskEntity.Status.IsWorkerFailed = true;
            taskEntity.Status.IsWorkerFinalFailed = taskEntity.Status.WorkerRetryCount >= 5 ? true : false;
            var result = await _flightRepository.UpdateTaskStatusAsync(jobId, taskEntity);
            isSuccess = false;

            Console.WriteLine(ex.Message);
        }

        return isSuccess;
    }
}
