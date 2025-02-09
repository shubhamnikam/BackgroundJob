using BackgroundJob.Common.Entities;
using BackgroundJob.Common.Models;
using Riok.Mapperly.Abstractions;

namespace BackgroundJob.Producer.Mapper;

[Mapper]
public static partial class FlightMapper
{
    public static FlightCreateOutputModel ToType(FlightEntity flightEntity, FlightQueueEntity flightQueueEntity)
    {
        return new FlightCreateOutputModel()
        {
            Id = flightEntity.Id,
            Name = flightEntity.Name,
            From = flightEntity.From,
            To = flightEntity.To,
            UUID = flightEntity.UUID,
            Price = flightEntity.Price,
            TimeStamp = flightEntity.TimeStamp,
            PassengerName = flightEntity.PassengerName,
            FlightQueue = flightQueueEntity
        };
    }
}