namespace BackgroundJob.Distributor.Services
{
    public class BusDistributor : IDistributor
    {
        public BusDistributor()
        {

        }
        public async Task ProcessAsync()
        {
            // lookup in queue table & fetch data
            // insert into mongodb
            // insert into rabbitmq
            Console.WriteLine("Hi from Busdistributor");
        }
    }
}
