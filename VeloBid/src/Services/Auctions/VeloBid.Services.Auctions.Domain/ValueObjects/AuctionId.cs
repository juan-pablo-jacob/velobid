namespace VeloBid.Services.Auctions.Domain.ValueObjects;

public readonly record struct AuctionId(Guid Value)
{
    public static AuctionId New()
    {
        return new AuctionId(Guid.NewGuid());
    }

    public static AuctionId From(Guid value)
    {
        return value == Guid.Empty
            ? throw new ArgumentException("Auction id cannot be empty.", nameof(value))
            : new AuctionId(value);
    }

    public override string ToString()
    {
        return Value.ToString();
    }
}