using BackgroundJob.Common.Entities;
using BackgroundJob.Common.Repositories;
using BackgroundJob.Common.Services;
using MongoDB.Driver;
using System.Data;

namespace BackgroundJob.Distributor.Services;

public class FlightDistributor : IDistributor
{
    private readonly IConfiguration _configuration;
    private readonly IFlightQueueRepository _flightQueueRepository;
    private readonly IRabbitMqService _rabbitMqService;

    public FlightDistributor(
        IConfiguration configuration,
        IRabbitMqService rabbitMqService,
        IMongoDbContext mongoDbContext,
        IFlightQueueRepository flightQueueRepository)
    {
        _configuration = configuration;
        _rabbitMqService = rabbitMqService;
        _flightQueueRepository = flightQueueRepository;
    }
    public async Task ProcessAsync()
    {
        // lookup in queue table & fetch data
        var dbQueuedItems = await GetDatabaseQueuedItemsAsync();
        if (dbQueuedItems is null || dbQueuedItems?.Count == 0)
        {
            return;
        }

        // insert into mongodb
        var job = await CreateJobsAsync(dbQueuedItems);
        if (job is null)
        {
            return;
        }

        // insert into rabbitmq
        var isEnqueued = await EnqueueToRabbitMQ(job);
        if (!isEnqueued)
        {
            return;
        }
        // update mongodb and sql qb with status 
        await UpdateTaskExtractedStatusesAsync(dbQueuedItems, job);
    }

    private async Task<List<FlightQueueEntity>> GetDatabaseQueuedItemsAsync()
    {
        return await _flightQueueRepository.GetDatabaseQueuedItemsAsync();
    }

    private async Task<JobEntity?> CreateJobsAsync(List<FlightQueueEntity> dbQueuedItems)
    {
        return await _flightQueueRepository.CreateJobsAsync(dbQueuedItems);
    }

    private async Task<bool> EnqueueToRabbitMQ(JobEntity job)
    {
        bool isEnqueued = false;
        try
        {
            // enqueue logic
            await _rabbitMqService.EnqueueAsync<JobEntity>(job, _configuration["RabbitMQ:DefaultQueueName"]);
            isEnqueued = true;
        }
        catch (Exception)
        {
            throw;
        }
        return isEnqueued;
    }

    private async Task UpdateTaskExtractedStatusesAsync(List<FlightQueueEntity> dbQueuedItems, JobEntity job)
    {
        // update mssql + mongodb
        var queueIds = job.Tasks
            .Select(x => x.QueueId)
            .ToList();
        var result = await _flightQueueRepository.UpdateQueueItemExtractedStatusAsync(queueIds);

        if (result.updatedSqlRecordCount == result.updatedMongoRecordCount &&
            result.updatedMongoRecordCount == queueIds.Count())
        {
            Console.WriteLine(@$"Updated statuses of queueIds: {string.Join(",", queueIds)}");
        }
    }
}
