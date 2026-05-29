using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace VeloBid.Services.Auctions.Infrastructure.Persistence;

internal sealed class AuctionsDbContextFactory : IDesignTimeDbContextFactory<AuctionsDbContext>
{
    private const string ConnectionString =
        "Host=localhost;Port=55432;Database=velobid_auctions;Username=postgres;Password=postgres";
    
    public AuctionsDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<AuctionsDbContext>();

        optionsBuilder.UseNpgsql(ConnectionString);

        return new AuctionsDbContext(optionsBuilder.Options);
    }
}