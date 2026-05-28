using VeloBid.Services.Auctions.Application.Abstractions.Persistence;
using VeloBid.Services.Auctions.Application.Auctions.GetActiveAuctions;
using VeloBid.Services.Auctions.Application.Auctions.GetAuctionById;
using VeloBid.Services.Auctions.Domain.ValueObjects;

namespace VeloBid.Services.Auctions.Application.UnitTests.Fakes;

internal sealed class FakeAuctionReadRepository : IAuctionReadRepository
{
    private readonly Dictionary<AuctionId, AuctionDetailsDto> _auctionDetails = [];

    private IReadOnlyCollection<AuctionSummaryDto> _activeAuctions = [];

    public Task<AuctionDetailsDto?> GetByIdAsync(
        AuctionId auctionId,
        CancellationToken cancellationToken = default)
    {
        _auctionDetails.TryGetValue(auctionId, out var auction);

        return Task.FromResult(auction);
    }

    public Task<IReadOnlyCollection<AuctionSummaryDto>> GetActiveAuctionsAsync(
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_activeAuctions);
    }

    public void Seed(AuctionDetailsDto auction)
    {
        _auctionDetails[AuctionId.From(auction.Id)] = auction;
    }

    public void SeedActiveAuctions(IReadOnlyCollection<AuctionSummaryDto> activeAuctions)
    {
        _activeAuctions = activeAuctions;
    }
}