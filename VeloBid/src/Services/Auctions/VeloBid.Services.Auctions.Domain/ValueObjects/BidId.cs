namespace VeloBid.Services.Auctions.Domain.ValueObjects;

public readonly record struct BidId(Guid Value)
{
    public static BidId New()
    {
        return new BidId(Guid.NewGuid());
    }

    public static BidId From(Guid value)
    {
        return value == Guid.Empty
            ? throw new ArgumentException("Bid id cannot be empty.", nameof(value))
            : new BidId(value);
    }

    public override string ToString()
    {
        return Value.ToString();
    }
}