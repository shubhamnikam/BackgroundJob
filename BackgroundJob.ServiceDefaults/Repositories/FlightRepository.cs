using BackgroundJob.Common.Entities;
using BackgroundJob.Common.Models;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace BackgroundJob.Common.Repositories;

public class FlightRepository : IFlightRepository
{
    private readonly SqlDbContext _context;
    private readonly MongoDbContext _mongoDbContext;

    public FlightRepository(SqlDbContext context, MongoDbContext mongoDbContext)
    {
        _context = context;
        _mongoDbContext = mongoDbContext;
    }

    public async Task<FlightEntity> CreateAsync(FlightCreateInputModel inputModel)
    {
        var result = new FlightEntity
        {
            UUID = Guid.CreateVersion7(),
            Name = inputModel.Name,
            PassengerName = inputModel.PassengerName,
            From = inputModel.From,
            To = inputModel.To,
            Price = inputModel.Price,
            TimeStamp = DateTimeOffset.UtcNow
        };

        await _context.Flights.AddAsync(result);
        await _context.SaveChangesAsync();
        return result;
    }

    public async Task<FlightEntity> UpdateAsync(FlightUpdateInputModel inputModel)
    {
        var flightEntity = await _context.Flights.FindAsync(inputModel.ObjectId);
        if (flightEntity == null)
        {
            throw new Exception($"No entity found for ObjectId: {inputModel.ObjectId}, ObjectTypeId: {inputModel.ObjectTypeId}");
        }

        flightEntity.Price += (flightEntity.Price * inputModel.TaxPrice) / 100;

        _context.Flights.Update(flightEntity);
        await _context.SaveChangesAsync();
        return flightEntity;
    }


    public async Task<int> UpdateTaskStatusAsync(string jobId, TaskEntity taskEntity)
    {
        var jobsCollection = _mongoDbContext.GetMongoCollection<JobEntity>("Jobs");

        var filter = Builders<JobEntity>.Filter.And(
             Builders<JobEntity>.Filter.Eq(j => j.Id, jobId),
             Builders<JobEntity>.Filter.ElemMatch(j => j.Tasks, t => t.Id == taskEntity.Id)
         );

        var update = Builders<JobEntity>.Update
                    .Set("Tasks.$.Status.IsWorkerFailed", taskEntity.Status.IsWorkerFailed)
                    .Set("Tasks.$.Status.IsWorkerFinalFailed", taskEntity.Status.IsWorkerFinalFailed)
                    .Set("Tasks.$.Status.IsWorkerSucceesed", taskEntity.Status.IsWorkerSucceesed)
                    .Set("Tasks.$.Status.WorkerRetryCount", taskEntity.Status.WorkerRetryCount);

        var result = await jobsCollection.UpdateOneAsync(filter, update);

        return (int)result.ModifiedCount;
    }
}

