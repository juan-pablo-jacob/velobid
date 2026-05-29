using Microsoft.EntityFrameworkCore;
using VeloBid.Services.Auctions.Domain.Aggregates;
using VeloBid.Services.Auctions.Domain.Entities;

namespace VeloBid.Services.Auctions.Infrastructure.Persistence;

public sealed class AuctionsDbContext(DbContextOptions<AuctionsDbContext> options) : DbContext(options)
{
    public DbSet<Auction> Auctions => Set<Auction>();

    public DbSet<Bid> Bids => Set<Bid>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("auctions");

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AuctionsDbContext).Assembly);

        base.OnModelCreating(modelBuilder);
    }
}