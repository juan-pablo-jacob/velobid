using VeloBid.Services.Auctions.Domain.Enums;

namespace VeloBid.Services.Auctions.Application.Auctions.ActivateAuction;

public sealed record ActivateAuctionResult(
    Guid AuctionId,
    AuctionStatus Status,
    Guid? WinnerId,
    decimal? WinningAmount,
    string? Currency);