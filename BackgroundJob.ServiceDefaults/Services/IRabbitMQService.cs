namespace BackgroundJob.Common.Services;

public interface IRabbitMqService
{
    Task EnqueueAsync<T>(T obj, string queueName);
    Task DequeueAsync<T>(string queueName, Func<T, Task<bool>> messageHandler);
}

