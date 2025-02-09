using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BackgroundJob.Common.Entities;

[Table("Buses")]
public class BusEntity
{
    [Key]
    public int Id { get; init; }
    public Guid UUID { get; init; } = Guid.NewGuid();
    public required string Name { get; init; }
    public required string PassengerName { get; init; }
    public required string From { get; init; }
    public required string To { get; init; }
    public required Decimal Price { get; init; }
    public DateTimeOffset TimeStamp { get; init; }
}
