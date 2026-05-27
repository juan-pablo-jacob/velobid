namespace VeloBid.Services.Auctions.Domain.ValueObjects;

public readonly record struct UserId(Guid Value)
{
    public static UserId From(Guid value)
    {
        return value == Guid.Empty
            ? throw new ArgumentException("User id cannot be empty.", nameof(value))
            : new UserId(value);
    }

    public override string ToString()
    {
        return Value.ToString();
    }
}