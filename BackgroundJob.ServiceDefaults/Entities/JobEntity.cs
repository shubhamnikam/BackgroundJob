using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using BackgroundJob.Common.Constants;

namespace BackgroundJob.Common.Entities;

public class JobEntity
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    [BsonElement("_id")]
    public string Id { get; set; }

    [BsonElement("CreatedOn")]
    public DateTimeOffset CreatedOn { get; set; }

    [BsonElement("ModifiedOn")]
    public DateTimeOffset ModifiedOn { get; set; }

    [BsonElement("Tasks")]
    public List<TaskEntity> Tasks { get; set; }
}

public class TaskEntity
{
    [BsonElement("Id")]
    public Guid Id { get; set; } = Guid.CreateVersion7();

    [BsonElement("QueueId")]
    public int QueueId { get; set; }

    [BsonElement("ObjectId")]
    public int ObjectId { get; set; }

    [BsonElement("ObjectTypeId")]
    public ObjectTypeEnum ObjectTypeId { get; set; }

    [BsonElement("CreatedOn")]
    public DateTimeOffset CreatedOn { get; set; }

    [BsonElement("ModifiedOn")]
    public DateTimeOffset ModifiedOn { get; set; }

    [BsonElement("Status")]
    public StatusEntity Status { get; set; }
}

public class StatusEntity
{

    [BsonElement("IsExtracted")]
    public bool IsExtracted { get; set; }

    [BsonElement("IsQueued")]
    public bool IsQueued { get; set; }

    [BsonElement("WorkerRetryCount")]
    public int WorkerRetryCount { get; set; }

    [BsonElement("IsWorkerSucceesed")]
    public bool IsWorkerSucceesed { get; set; }

    [BsonElement("IsWorkerFailed")]
    public bool IsWorkerFailed { get; set; }

    [BsonElement("IsWorkerFinalFailed")]
    public bool IsWorkerFinalFailed { get; set; }
}


