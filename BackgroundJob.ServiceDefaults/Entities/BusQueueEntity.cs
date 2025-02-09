using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BackgroundJob.Common.Constants;

namespace BackgroundJob.Common.Entities;

[Table("BusQueue")]
public class BusQueueEntity
{
    [Key]
    public int Id { get; init; }
    public DateTimeOffset CreatedOn { get; set; }
    public DateTimeOffset UpdatedOn { get; set; }
    public bool IsQueued { get; set; }
    public bool IsExtracted { get; set; }
    public bool IsDistributed { get; set; }
    public bool IsWorkerPickup { get; set; }
    public bool IsWorkerProcessing { get; set; }
    public bool IsWorkerCompleted { get; set; }

    public int ObjectId { get; init; }
    [Required]
    [ForeignKey("ObjectTypeId")]
    public ObjectTypeEnum ObjectTypeId { get; init; }
    public ObjectTypeEntity ObjectType { get; set; }
}
