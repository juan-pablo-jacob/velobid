using VeloBid.Services.Auctions.Domain.ValueObjects;

namespace VeloBid.Services.Auctions.Domain.Events;

public sealed record BidPlacedDomainEvent(
    AuctionId AuctionId,
    UserId BidderId,
    Money Amount,
    DateTimeOffset PlacedAtUtc,
    DateTimeOffset AuctionEndsAtUtc,
    bool AuctionWasExtended,
    DateTimeOffset OccurredOnUtc) : IDomainEvent;