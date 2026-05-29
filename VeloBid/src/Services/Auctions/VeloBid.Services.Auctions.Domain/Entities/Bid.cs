using VeloBid.Services.Auctions.Domain.Primitives;
using VeloBid.Services.Auctions.Domain.ValueObjects;

namespace VeloBid.Services.Auctions.Domain.Entities;

public sealed class Bid : Entity<BidId>
{
    private Bid()
        : base(default!)
    {
        AuctionId = default;
        BidderId = default;
        Amount = null!;
    }

    private Bid(
        BidId id,
        AuctionId auctionId,
        UserId bidderId,
        Money amount,
        DateTimeOffset placedAtUtc)
        : base(id)
    {
        AuctionId = auctionId;
        BidderId = bidderId;
        Amount = amount;
        PlacedAtUtc = placedAtUtc;
    }

    public AuctionId AuctionId { get; }

    public UserId BidderId { get; }

    public Money Amount { get; }

    public DateTimeOffset PlacedAtUtc { get; }

    internal static Bid Create(
        AuctionId auctionId,
        UserId bidderId,
        Money amount,
        DateTimeOffset placedAtUtc)
    {
        return new Bid(
            BidId.New(),
            auctionId,
            bidderId,
            amount,
            placedAtUtc);
    }
}