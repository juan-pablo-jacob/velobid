using VeloBid.Services.Auctions.Domain.Enums;

namespace VeloBid.Services.Auctions.Application.Auctions.GetAuctionById;

public sealed record AuctionDetailsDto(
    Guid Id,
    Guid SellerId,
    string Title,
    string Description,
    AuctionStatus Status,
    decimal StartingPrice,
    decimal CurrentPrice,
    string Currency,
    DateTimeOffset StartsAtUtc,
    DateTimeOffset EndsAtUtc,
    Guid? WinnerId,
    decimal? WinningAmount);