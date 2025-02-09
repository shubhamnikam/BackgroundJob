
using BackgroundJob.Common.Extensions;
using BackgroundJob.Common.Repositories;
using BackgroundJob.Common.Services;
using BackgroundJob.Distributor.BackgroundServices;
using BackgroundJob.Distributor.Services;
using Microsoft.AspNetCore.Connections;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using RabbitMQ.Client;

namespace BackgroundJob.Distributor;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.AddServiceDefaults();

        // Add services to the container.
        builder.Services.AddControllers();
        builder.Services.AddOpenApi();

        // mssql
        builder.Services.AddDbContext<SqlDbContext>(options =>
           options.UseSqlServer(builder.Configuration.GetValue<string>("MsSqlDB:DefaultConnection")));

        // mongodb
        builder.Services.AddScoped<IMongoDbContext, MongoDbContext>();

        // rabbitmq
        builder.Services.AddScoped<IRabbitMqService, RabbitMqService>();


        builder.Services.AddScoped<IFlightQueueRepository, FlightQueueRepository>();
        builder.Services.AddScoped<IDistributor, FlightDistributor>();
        builder.Services.AddScoped<IDistributor, BusDistributor>();
        // add background hosted service
        builder.Services.AddHostedService<DistributorBackgroundService>();

        var app = builder.Build();

        app.MapDefaultEndpoints();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
        }

        app.UseHttpsRedirection();

        app.UseAuthorization();


        app.MapControllers();

        app.Run();
    }
}
