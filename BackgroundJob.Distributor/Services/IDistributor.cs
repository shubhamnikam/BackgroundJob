namespace BackgroundJob.Distributor.Services;

public interface IDistributor
{
    Task ProcessAsync();
}
