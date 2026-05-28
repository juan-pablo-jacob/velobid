using VeloBid.Services.Auctions.Domain.Enums;

namespace VeloBid.Services.Auctions.Application.Auctions.CloseAuction;

public sealed record CloseAuctionResult(
    Guid AuctionId,
    AuctionStatus Status,
    Guid? WinnerId,
    decimal? WinningAmount,
    string? Currency);