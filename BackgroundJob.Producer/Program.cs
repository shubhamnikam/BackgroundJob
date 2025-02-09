
using BackgroundJob.Common.Extensions;
using BackgroundJob.Common.Repositories;
using BackgroundJob.Producer.Services;
using Microsoft.EntityFrameworkCore;

namespace BackgroundJob.Producer;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.AddServiceDefaults();

        // Add services to the container.
        builder.Services.AddControllers();
        builder.Services.AddOpenApi();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        // mssql
        builder.Services.AddDbContext<SqlDbContext>(options =>
           options.UseSqlServer(builder.Configuration.GetValue<string>("MsSqlDB:DefaultConnection")));

        // mongodb
        builder.Services.AddScoped<IMongoDbContext, MongoDbContext>();
        //repositories DI 
        builder.Services.AddScoped<IFlightRepository, FlightRepository>();
        builder.Services.AddScoped<IFlightQueueRepository, FlightQueueRepository>();
        //service DI 
        builder.Services.AddScoped<IFlightService, FlightService>();

        // Configure the HTTP request pipeline.
        var app = builder.Build();

        app.UseMiddleware<RequestIdMiddleware>();
        app.MapDefaultEndpoints();

        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}
