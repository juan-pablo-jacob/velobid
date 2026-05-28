using VeloBid.Services.Auctions.Application.Auctions.GetActiveAuctions;
using VeloBid.Services.Auctions.Application.Auctions.GetAuctionById;
using VeloBid.Services.Auctions.Domain.ValueObjects;

namespace VeloBid.Services.Auctions.Application.Abstractions.Persistence;

/// <summary>
/// For queries. I can implement this with Dapper for example
/// </summary>
public interface IAuctionReadRepository
{
    Task<AuctionDetailsDto?> GetByIdAsync(
        AuctionId auctionId,
        CancellationToken cancellationToken = default);
    
    Task<IReadOnlyCollection<AuctionSummaryDto>> GetActiveAuctionsAsync(
        CancellationToken cancellationToken = default);
}