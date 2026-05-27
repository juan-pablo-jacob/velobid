using VeloBid.Services.Auctions.Domain.ValueObjects;

namespace VeloBid.Services.Auctions.Domain.Events;

public sealed record AuctionExpiredDomainEvent(
    AuctionId AuctionId,
    DateTimeOffset ExpiredAtUtc,
    DateTimeOffset OccurredOnUtc) : IDomainEvent;