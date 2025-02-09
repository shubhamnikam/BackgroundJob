using BackgroundJob.Common.Entities;

namespace BackgroundJob.Common.Models;

public class FlightCreateOutputModel
{
    public required int Id { get; init; }
    public required Guid UUID { get; init; }
    public required string Name { get; init; }
    public required string PassengerName { get; init; }
    public required string From { get; init; }
    public required string To { get; init; }
    public required Decimal Price { get; init; }
    public required DateTimeOffset TimeStamp { get; init; }
    public required FlightQueueEntity FlightQueue { get; init; }
}
