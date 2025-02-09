using BackgroundJob.Common.Constants;
using BackgroundJob.Common.Entities;
using Microsoft.EntityFrameworkCore;
using MongoDB.Bson;
using MongoDB.Driver;

namespace BackgroundJob.Common.Repositories;

public class FlightQueueRepository : IFlightQueueRepository
{
    private readonly SqlDbContext _sqlDbContext;
    private readonly MongoDbContext _mongoDbContext;

    public FlightQueueRepository(SqlDbContext context, MongoDbContext mongoDbContext)
    {
        _sqlDbContext = context;
        _mongoDbContext = mongoDbContext;
    }

    public async Task<FlightQueueEntity> CreateAsync(int id)
    {
        ObjectTypeEnum objectTypeId = _sqlDbContext.ObjectTypes
            .Where(x => x.Name == AppConstants.Flight)
            .Select(x => (ObjectTypeEnum)x.Id).FirstOrDefault();
        var flightQueue = new FlightQueueEntity()
        {
            CreatedOn = DateTimeOffset.UtcNow,
            UpdatedOn = DateTimeOffset.UtcNow,
            IsExtracted = false,
            ObjectId = id,
            ObjectTypeId = objectTypeId,
        };

        await _sqlDbContext.FlightQueues.AddAsync(flightQueue);
        await _sqlDbContext.SaveChangesAsync();

        return flightQueue;
    }

    public async Task<List<FlightQueueEntity>> GetDatabaseQueuedItemsAsync()
    {
        return await _sqlDbContext.FlightQueues
            .Where(x => !x.IsExtracted)
            .ToListAsync();
    }

    public async Task<JobEntity?> CreateJobsAsync(List<FlightQueueEntity> dbQueuedItems)
    {
        var jobsCollection = _mongoDbContext.GetMongoCollection<JobEntity>("Jobs");

        var tasks = new List<TaskEntity>();
        foreach (var item in dbQueuedItems)
        {
            // check if combination of Objectid & objecttype & QueueId is present already or not else return
            bool isPresentAndShouldSkipTask = await jobsCollection.Find(
                x => x.Tasks.Any(x =>
                    x.QueueId == item.Id &&
                    x.ObjectId == item.ObjectId &&
                    x.ObjectTypeId == item.ObjectTypeId))
                .AnyAsync();

            if (isPresentAndShouldSkipTask)
            {
                continue;
            }

            tasks.Add(new TaskEntity()
            {
                CreatedOn = DateTimeOffset.UtcNow,
                ModifiedOn = DateTimeOffset.UtcNow,
                Id = Guid.CreateVersion7(),
                ObjectId = item.ObjectId,
                ObjectTypeId = item.ObjectTypeId,
                QueueId = item.Id,
                Status = new StatusEntity()
                {
                    WorkerRetryCount = 0,
                    IsExtracted = true,
                    IsQueued = false,
                    IsWorkerSucceesed = false,
                    IsWorkerFailed = false,
                    IsWorkerFinalFailed = false
                }
            });
        }
        if (tasks.Count == 0)
        {
            return null;
        }

        var job = new JobEntity()
        {
            CreatedOn = DateTimeOffset.UtcNow,
            ModifiedOn = DateTimeOffset.UtcNow,
            Tasks = tasks
        };

        await _mongoDbContext.GetMongoCollection<JobEntity>("Jobs")
            .InsertOneAsync(job);
        return job;
    }

    public async Task<(int updatedSqlRecordCount, int updatedMongoRecordCount)> UpdateQueueItemExtractedStatusAsync(List<int> dbQueueIds)
    {
        // sql update 
        var flightQueuesToUpdate = await _sqlDbContext.FlightQueues
       .Where(fq => dbQueueIds.Contains(fq.Id))
       .ToListAsync();

        var updatedSqlRecordCount = 0;

        if (flightQueuesToUpdate.Any())
        {
            foreach (var flightQueue in flightQueuesToUpdate)
            {
                flightQueue.IsExtracted = true;
            }

            updatedSqlRecordCount = await _sqlDbContext.SaveChangesAsync();
        }

        // mongo update
        var jobsCollection = _mongoDbContext.GetMongoCollection<JobEntity>("Jobs");

        var filter = Builders<JobEntity>.Filter.ElemMatch(job => job.Tasks, task => dbQueueIds.Contains(task.QueueId));

        var update = Builders<JobEntity>.Update.Set("Tasks.$[task].IsExtracted", true);

        var arrayFilters = new[]
        {
            new BsonDocumentArrayFilterDefinition<BsonDocument>(
                new BsonDocument("task.QueueId", new BsonDocument("$in", new BsonArray(dbQueueIds))))
        };

        var result = await jobsCollection.UpdateManyAsync(
            filter,
            update,
            new UpdateOptions { ArrayFilters = arrayFilters }
        );

        var updatedMongoRecordCount = (int)result.ModifiedCount;

        return (updatedSqlRecordCount, updatedMongoRecordCount);
    }
}