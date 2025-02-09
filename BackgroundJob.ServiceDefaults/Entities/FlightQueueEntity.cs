using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BackgroundJob.Common.Constants;

namespace BackgroundJob.Common.Entities;

[Table("FlightQueue")]
public class FlightQueueEntity
{
    [Key]
    public int Id { get; init; }
    public DateTimeOffset CreatedOn { get; set; }
    public DateTimeOffset UpdatedOn { get; set; }
    // true when pickup entry from job queue
    public bool IsExtracted { get; set; }
    public int ObjectId { get; init; }
    [Required]
    [ForeignKey("ObjectTypeId")]
    public ObjectTypeEnum ObjectTypeId { get; init; }
    public ObjectTypeEntity ObjectType { get; set; }
}
