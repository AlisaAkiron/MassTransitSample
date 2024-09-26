using MassTransit;
using Microsoft.EntityFrameworkCore;
using Sample.Common.StateMachine;
using Sample.Database.Entities;

namespace Sample.Database;

public class SampleDbContext : DbContext
{
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<Payment> Payments => Set<Payment>();

    public DbSet<OrderingData> OrderingData => Set<OrderingData>();
    public DbSet<PingPong> PingPongs => Set<PingPong>();

    public SampleDbContext(DbContextOptions<SampleDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.AddInboxStateEntity();
        modelBuilder.AddOutboxMessageEntity();
        modelBuilder.AddOutboxStateEntity();

        modelBuilder.Entity<OrderingData>().HasKey(x => x.CorrelationId);
        modelBuilder.Entity<PingPong>().HasKey(x => x.CorrelationId);
    }
}
