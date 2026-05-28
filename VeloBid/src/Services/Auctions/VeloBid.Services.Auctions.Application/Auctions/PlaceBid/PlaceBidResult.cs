namespace VeloBid.Services.Auctions.Application.Auctions.PlaceBid;

public sealed record PlaceBidResult(
    Guid AuctionId,
    Guid BidId,
    Guid BidderId,
    decimal Amount,
    string Currency,
    DateTimeOffset PlacedAtUtc,
    DateTimeOffset AuctionEndsAtUtc);