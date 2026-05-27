using VeloBid.Services.Auctions.Domain.Enums;
using VeloBid.Services.Auctions.Domain.ValueObjects;

namespace VeloBid.Services.Auctions.Domain.Events;

public sealed record AuctionCreatedDomainEvent(
    AuctionId AuctionId,
    UserId SellerId,
    AuctionStatus InitialStatus,
    DateTimeOffset StartsAtUtc,
    DateTimeOffset EndsAtUtc,
    DateTimeOffset OccurredOnUtc) : IDomainEvent;