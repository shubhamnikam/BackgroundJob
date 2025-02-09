using BackgroundJob.Common.Entities;
using BackgroundJob.Common.Models;

namespace BackgroundJob.Common.Repositories;

public interface IFlightQueueRepository
{
    Task<FlightQueueEntity> CreateAsync(int id);
    Task<List<FlightQueueEntity>> GetDatabaseQueuedItemsAsync();
    Task<JobEntity?> CreateJobsAsync(List<FlightQueueEntity> dbQueuedItems);
    Task<(int updatedSqlRecordCount, int updatedMongoRecordCount)> UpdateQueueItemExtractedStatusAsync(List<int> dbQueueIds);
}
