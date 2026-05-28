using VeloBid.Services.Auctions.Domain.Enums;

namespace VeloBid.Services.Auctions.Application.Auctions.CreateAuction;

public sealed record CreateAuctionResult(
    Guid AuctionId,
    AuctionStatus Status,
    DateTimeOffset StartsAtUtc,
    DateTimeOffset EndsAtUtc);