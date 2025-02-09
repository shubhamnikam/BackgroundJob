using BackgroundJob.Common.Constants;
using BackgroundJob.Common.Entities;

namespace BackgroundJob.Worker.Handlers;

public class BusHandler : IHandler
{
    public BusHandler()
    {

    }

    public ObjectTypeEnum ObjectType => ObjectTypeEnum.Bus;

    public async Task<bool> ProcessAsync(string jobId, TaskEntity taskEntity)
    {
        // lookup in queue table & fetch data
        // insert into mongodb
        // insert into rabbitmq
        Console.WriteLine("Hi from BusdHandler");
        return true;
    }
}
