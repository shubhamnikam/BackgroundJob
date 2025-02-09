
using BackgroundJob.Distributor.Services;

namespace BackgroundJob.Distributor.BackgroundServices;

public class DistributorBackgroundService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;

    public DistributorBackgroundService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
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
            using (var scope = _serviceProvider.CreateScope())
            {
                var distributors = scope.ServiceProvider.GetRequiredService<IEnumerable<IDistributor>>();

                foreach (var distributor in distributors)
                {
                    try
                    {
                        await distributor.ProcessAsync();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(new { Message = ex.Message, InnerEx = ex?.InnerException });
                    }
                }
                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}
