using VeloBid.Services.Auctions.Application.Abstractions.Data;

namespace VeloBid.Services.Auctions.Infrastructure.Persistence;

internal sealed class UnitOfWork(AuctionsDbContext dbContext) : IUnitOfWork
{
    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}