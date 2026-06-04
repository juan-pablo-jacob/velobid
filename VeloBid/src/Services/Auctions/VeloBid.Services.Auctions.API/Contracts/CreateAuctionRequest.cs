namespace VeloBid.Services.Auctions.API.Contracts;

public sealed record CreateAuctionRequest(
    Guid SellerId,
    string Title,
    string Description,
    decimal StartingPrice,
    decimal MinimumBidIncrement,
    string Currency,
    DateTimeOffset StartsAtUtc,
    DateTimeOffset EndsAtUtc);