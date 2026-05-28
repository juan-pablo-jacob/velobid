using VeloBid.Services.Auctions.Domain.Aggregates;
using VeloBid.Services.Auctions.Domain.ValueObjects;

namespace VeloBid.Services.Auctions.Application.UnitTests.TestData;

internal static class AuctionFactory
{
    public static Auction CreateScheduledAuction(
        DateTimeOffset nowUtc,
        UserId? sellerId = null)
    {
        return Auction.Create(
            sellerId ?? NewUserId(),
            "Carbon road bike",
            "A lightweight carbon road bike.",
            Money.Create(1_000m, "EUR"),
            Money.Create(10m, "EUR"),
            AuctionPeriod.Create(
                nowUtc.AddHours(1),
                nowUtc.AddHours(4)),
            nowUtc);
    }

    public static Auction CreateActiveAuction(
        DateTimeOffset nowUtc,
        UserId? sellerId = null)
    {
        var auction = Auction.Create(
            sellerId ?? NewUserId(),
            "Carbon road bike",
            "A lightweight carbon road bike.",
            Money.Create(1_000m, "EUR"),
            Money.Create(10m, "EUR"),
            AuctionPeriod.Create(
                nowUtc,
                nowUtc.AddHours(3)),
            nowUtc);

        auction.Activate(nowUtc);

        return auction;
    }

    public static UserId NewUserId()
    {
        return UserId.From(Guid.NewGuid());
    }
}