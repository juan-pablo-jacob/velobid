using Microsoft.EntityFrameworkCore;
using VeloBid.Services.Auctions.Application.Abstractions.Persistence;
using VeloBid.Services.Auctions.Domain.Aggregates;
using VeloBid.Services.Auctions.Domain.ValueObjects;

namespace VeloBid.Services.Auctions.Infrastructure.Persistence.Repositories;

internal sealed class AuctionRepository(AuctionsDbContext dbContext) : IAuctionRepository
{
    public async Task<Auction?> GetByIdAsync(
        AuctionId auctionId,
        CancellationToken cancellationToken = default)
    {
        return await dbContext.Auctions
            .Include(auction => auction.Bids)
            .FirstOrDefaultAsync(
                auction => auction.Id == auctionId,
                cancellationToken);
    }

    public async Task AddAsync(
        Auction auction,
        CancellationToken cancellationToken = default)
    {
        await dbContext.Auctions.AddAsync(auction, cancellationToken);
    }
}