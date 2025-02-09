
using BackgroundJob.Common.Extensions;
using BackgroundJob.Common.Repositories;
using BackgroundJob.Common.Services;
using BackgroundJob.Producer.Services;
using BackgroundJob.Worker.BackgroundServices;
using BackgroundJob.Worker.Handlers;
using Microsoft.EntityFrameworkCore;

namespace BackgroundJob.Worker;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.AddServiceDefaults();

        // Add services to the container.

        builder.Services.AddControllers();
        // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
        builder.Services.AddOpenApi();
        // mssql
        builder.Services.AddDbContext<SqlDbContext>(options =>
           options.UseSqlServer(builder.Configuration.GetValue<string>("MsSqlDB:DefaultConnection")));

        // mongodb
        builder.Services.AddScoped<IMongoDbContext, MongoDbContext>();

        // rabbitmq
        builder.Services.AddSingleton<IRabbitMqService, RabbitMqService>();

        //repo DI
        builder.Services.AddScoped<IHandler, FlightHandler>();
        builder.Services.AddScoped<IHandler, BusHandler>();
        builder.Services.AddScoped<IFlightQueueRepository, FlightQueueRepository>();
        //service DI 
        builder.Services.AddScoped<IFlightService, FlightService>();

        // add background hosted service
        builder.Services.AddHostedService<WorkerBackgroundService>();
        var app = builder.Build();

        app.UseMiddleware<RequestIdMiddleware>();
        app.MapDefaultEndpoints();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
        }

        app.UseAuthorization();


        app.MapControllers();

        app.Run();
    }
}
