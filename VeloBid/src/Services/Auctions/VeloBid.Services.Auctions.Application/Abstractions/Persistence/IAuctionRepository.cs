using VeloBid.Services.Auctions.Domain.Aggregates;
using VeloBid.Services.Auctions.Domain.ValueObjects;

namespace VeloBid.Services.Auctions.Application.Abstractions.Persistence;

public interface IAuctionRepository
{
    Task<Auction?> GetByIdAsync(
        AuctionId auctionId,
        CancellationToken cancellationToken = default);

    Task AddAsync(
        Auction auction,
        CancellationToken cancellationToken = default);
}