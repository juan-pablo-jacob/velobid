using VeloBid.Services.Auctions.Domain.Exceptions;

namespace VeloBid.Services.Auctions.Domain.ValueObjects;

public sealed record AuctionPeriod
{
    private static readonly TimeSpan MinimumDuration = TimeSpan.FromHours(1);
    private static readonly TimeSpan MaximumDuration = TimeSpan.FromDays(7);
    private static readonly TimeSpan AntiSnipingThreshold = TimeSpan.FromMinutes(2);
    private static readonly TimeSpan AntiSnipingExtension = TimeSpan.FromMinutes(2);

    private AuctionPeriod(DateTimeOffset startsAtUtc, DateTimeOffset endsAtUtc)
    {
        StartsAtUtc = startsAtUtc;
        EndsAtUtc = endsAtUtc;
    }

    public DateTimeOffset StartsAtUtc { get; }

    public DateTimeOffset EndsAtUtc { get; }

    public TimeSpan Duration => EndsAtUtc - StartsAtUtc;

    public static AuctionPeriod Create(DateTimeOffset startsAtUtc, DateTimeOffset endsAtUtc)
    {
        if (startsAtUtc >= endsAtUtc)
        {
            throw new InvalidAuctionPeriodException("Start date must be earlier than end date.");
        }

        var duration = endsAtUtc - startsAtUtc;

        if (duration < MinimumDuration)
        {
            throw new InvalidAuctionPeriodException($"Minimum duration is {MinimumDuration.TotalHours:N0} hour.");
        }

        if (duration > MaximumDuration)
        {
            throw new InvalidAuctionPeriodException($"Maximum duration is {MaximumDuration.TotalDays:N0} days.");
        }

        return new AuctionPeriod(startsAtUtc, endsAtUtc);
    }

    public bool HasStarted(DateTimeOffset nowUtc)
    {
        return nowUtc >= StartsAtUtc;
    }

    public bool HasEnded(DateTimeOffset nowUtc)
    {
        return nowUtc >= EndsAtUtc;
    }

    public bool IsWithinAntiSnipingWindow(DateTimeOffset nowUtc)
    {
        var remainingTime = EndsAtUtc - nowUtc;

        return remainingTime > TimeSpan.Zero && remainingTime <= AntiSnipingThreshold;
    }

    public AuctionPeriod ExtendForAntiSniping()
    {
        return new AuctionPeriod(StartsAtUtc, EndsAtUtc.Add(AntiSnipingExtension));
    }
}