using VeloBid.Services.Auctions.Application.Abstractions.Persistence;
using VeloBid.Services.Auctions.Domain.Aggregates;
using VeloBid.Services.Auctions.Domain.ValueObjects;

namespace VeloBid.Services.Auctions.Application.UnitTests.Fakes;


internal sealed class FakeAuctionRepository : IAuctionRepository
{
    private readonly Dictionary<AuctionId, Auction> _auctions = [];

    public int AddCallCount { get; private set; }

    public int GetByIdCallCount { get; private set; }

    public Auction? LastAddedAuction { get; private set; }

    public Task<Auction?> GetByIdAsync(
        AuctionId auctionId,
        CancellationToken cancellationToken = default)
    {
        GetByIdCallCount++;

        _auctions.TryGetValue(auctionId, out var auction);

        return Task.FromResult(auction);
    }

    public Task AddAsync(
        Auction auction,
        CancellationToken cancellationToken = default)
    {
        AddCallCount++;
        LastAddedAuction = auction;
        _auctions[auction.Id] = auction;

        return Task.CompletedTask;
    }

    public void Seed(Auction auction)
    {
        _auctions[auction.Id] = auction;
    }
}