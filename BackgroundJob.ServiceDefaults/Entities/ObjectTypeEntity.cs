using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace BackgroundJob.Common.Entities;

[Table("ObjectTypes")]
public class ObjectTypeEntity
{
    [Key]
    public required int Id { get; init; }
    public required string Name { get; init; }

    public ICollection<FlightQueueEntity> FlightQueues { get; set; } = new List<FlightQueueEntity>();
    public ICollection<BusQueueEntity> BusQueues { get; set; } = new List<BusQueueEntity>();
}
