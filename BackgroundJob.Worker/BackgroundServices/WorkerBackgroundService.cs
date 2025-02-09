
using BackgroundJob.Common.Constants;
using BackgroundJob.Common.Entities;
using BackgroundJob.Common.Repositories;
using BackgroundJob.Common.Services;
using BackgroundJob.Worker.Handlers;

namespace BackgroundJob.Worker.BackgroundServices;

public class WorkerBackgroundService : BackgroundService
{
    private readonly Dictionary<ObjectTypeEnum, IHandler> _handlerDict;
    private readonly IConfiguration _configuration;
    private readonly IRabbitMqService _rabbitMqService;

    public WorkerBackgroundService(
        IServiceProvider serviceProvider,
        IConfiguration configuration,
        IRabbitMqService rabbitMqService,
        IMongoDbContext mongoDbContext)
    {
        _handlerDict = serviceProvider.GetRequiredService<IEnumerable<IHandler>>()
            .ToDictionary(x => x.ObjectType, x => x);

        _configuration = configuration;
        _rabbitMqService = rabbitMqService;
    }

    public override async Task StartAsync(CancellationToken cancellationToken)
    {
        await base.StartAsync(cancellationToken);
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        await base.StopAsync(cancellationToken);
    }

    protected async override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                // dequeue message
                await _rabbitMqService.DequeueAsync<JobEntity>(
                    _configuration["RabbitMQ:DefaultQueueName"],
                    async (jobEntity) =>
                    {
                        List<bool> allTasksSuccessList = new();
                        foreach (var task in jobEntity.Tasks)
                        {
                            try
                            {
                                // detect handler based on objecttypeid
                                if (_handlerDict.TryGetValue(task.ObjectTypeId, out var handler))
                                {
                                    throw new InvalidOperationException($"No handler found for {task.ObjectTypeId}");
                                }
                                // trigger common method on handler & pass data
                                var isSuccess = await handler!.ProcessAsync(jobEntity.Id, task);
                                allTasksSuccessList.Add(isSuccess);
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(new { Message = ex.Message, InnerEx = ex?.InnerException });
                                allTasksSuccessList.Add(false);
                            }
                        }
                        return allTasksSuccessList.All(x => x is true);
                    });
            }
            catch (Exception ex)
            {
                Console.WriteLine(new { Message = ex.Message, InnerEx = ex?.InnerException });
            }
            await Task.Delay(1000, stoppingToken);
        }
    }
}
