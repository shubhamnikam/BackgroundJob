using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace BackgroundJob.Common.Services;

public class RabbitMqService : IRabbitMqService
{
    private readonly ConnectionFactory _connectionFactory;
    public RabbitMqService(IConfiguration configuration)
    {
        _connectionFactory = new ConnectionFactory()
        {
            HostName = configuration["RabbitMQ:HostName"],
            Port = int.TryParse(configuration["RabbitMQ:Port"], out int port) ? port : 5672,
            UserName = configuration["RabbitMQ:UserName"],
            Password = configuration["RabbitMQ:Password"],
        };
    }
    public async Task EnqueueAsync<T>(T obj, string queueName)
    {
        using var connection = await _connectionFactory.CreateConnectionAsync();
        using var channel = await connection.CreateChannelAsync();

        await channel.QueueDeclareAsync(queue: queueName,
                             durable: true,
                             exclusive: false,
                             autoDelete: false,
                             arguments: null);

        var message = JsonSerializer.Serialize(obj);
        var body = Encoding.UTF8.GetBytes(message);

        await channel.BasicPublishAsync(exchange: "",
                             routingKey: queueName,
                             mandatory: true,
                             body: body);

        Console.WriteLine($"RabbitMQ message enqueued: {message}");
    }

    public async Task DequeueAsync<T>(string queueName, Func<T, Task<bool>> messageHandler)
    {
        using var connection = await _connectionFactory.CreateConnectionAsync();
        using var channel = await connection.CreateChannelAsync();

        await channel.QueueDeclareAsync(queue: queueName,
                             durable: true,
                             exclusive: false,
                             autoDelete: false,
                             arguments: null);

        var consumer = new AsyncEventingBasicConsumer(channel);
        consumer.ReceivedAsync += async (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            var obj = JsonSerializer.Deserialize<T>(message);
            Console.WriteLine($"RabbitMQ message dequeued: {message}");

            bool isSuccess = await messageHandler(obj);
            if (isSuccess)
            {
                await channel.BasicAckAsync(ea.DeliveryTag, multiple: false);
            }
            else
            {
                await channel.BasicNackAsync(ea.DeliveryTag, multiple: false, requeue: true);
            }
        };

        await channel.BasicConsumeAsync(
            queue: queueName,
            autoAck: false,
            consumer: consumer);
    }
}

