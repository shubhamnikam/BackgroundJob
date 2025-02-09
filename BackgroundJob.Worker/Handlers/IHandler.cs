using BackgroundJob.Common.Constants;
using BackgroundJob.Common.Entities;

namespace BackgroundJob.Worker.Handlers;

public interface IHandler
{
    ObjectTypeEnum ObjectType { get; }

    Task<bool> ProcessAsync(string jobId, TaskEntity taskEntity);
}
