namespace BackgroundJob.Common.Models;

public class FlightCreateInputModel
{
    public required string Name { get; init; }
    public required string PassengerName { get; init; }
    public required string From { get; init; }
    public required string To { get; init; }
    public required Decimal Price { get; init; }
}
