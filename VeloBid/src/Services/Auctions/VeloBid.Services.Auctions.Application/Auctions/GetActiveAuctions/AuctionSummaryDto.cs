using VeloBid.Services.Auctions.Domain.Enums;

namespace VeloBid.Services.Auctions.Application.Auctions.GetActiveAuctions;

public sealed record AuctionSummaryDto(
    Guid Id,
    string Title,
    AuctionStatus Status,
    decimal CurrentPrice,
    string Currency,
    DateTimeOffset EndsAtUtc,
    int TotalBids);