using BackgroundJob.Common.Constants;
using BackgroundJob.Common.Entities;

namespace BackgroundJob.Common.Models;

public class FlightUpdateInputModel
{
    public required int ObjectId { get; init; }
    public required ObjectTypeEnum ObjectTypeId { get; init; }
    public required Decimal TaxPrice { get; init; }
}
