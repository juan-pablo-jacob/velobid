using VeloBid.Services.Auctions.Domain.ValueObjects;

namespace VeloBid.Services.Auctions.Domain.Events;

public sealed record AuctionActivatedDomainEvent(
    AuctionId AuctionId,
    DateTimeOffset OccurredOnUtc) : IDomainEvent;