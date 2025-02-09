using BackgroundJob.Common.Constants;
using BackgroundJob.Common.Entities;
using Microsoft.EntityFrameworkCore;

namespace BackgroundJob.Common.Repositories;

public class SqlDbContext : DbContext
{
    public SqlDbContext(DbContextOptions<SqlDbContext> options) : base(options) { }
    public DbSet<ObjectTypeEntity> ObjectTypes { get; set; }
    public DbSet<FlightEntity> Flights { get; set; }
    public DbSet<BusEntity> Buses { get; set; }
    public DbSet<BusQueueEntity> BusQueues { get; set; }
    public DbSet<FlightQueueEntity> FlightQueues { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<ObjectTypeEntity>().HasData(
            new ObjectTypeEntity { Id = (int)ObjectTypeEnum.Flight, Name = "Flight" },
            new ObjectTypeEntity { Id = (int)ObjectTypeEnum.Bus, Name = "Bus" }
        );

        modelBuilder.Entity<FlightQueueEntity>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.HasOne(e => e.ObjectType)
                  .WithMany(ot => ot.FlightQueues)
                  .HasForeignKey(e => e.ObjectTypeId)
                  .OnDelete(DeleteBehavior.NoAction);
        });

        modelBuilder.Entity<BusQueueEntity>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.HasOne(e => e.ObjectType)
                  .WithMany(ot => ot.BusQueues)
                  .HasForeignKey(e => e.ObjectTypeId)
                  .OnDelete(DeleteBehavior.NoAction);
        });
    }
}
