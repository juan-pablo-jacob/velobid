using VeloBid.Services.Auctions.Domain.ValueObjects;

namespace VeloBid.Services.Auctions.Domain.Events;

public sealed record AuctionCompletedDomainEvent(
    AuctionId AuctionId,
    UserId WinnerId,
    Money WinningAmount,
    DateTimeOffset CompletedAtUtc,
    DateTimeOffset OccurredOnUtc) : IDomainEvent;