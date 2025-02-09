using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BackgroundJob.Common.Entities;

[Table("Flights")]
public class FlightEntity
{
    [Key]
    public int Id { get; init; }
    public Guid UUID { get; init; } = Guid.NewGuid();
    public required string Name { get; set; }
    public required string PassengerName { get; set; }
    public required string From { get; set; }
    public required string To { get; set; }
    public required Decimal Price { get; set; }
    public DateTimeOffset TimeStamp { get; set; }
}
