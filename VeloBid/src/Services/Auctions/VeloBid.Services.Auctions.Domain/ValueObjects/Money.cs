using VeloBid.Services.Auctions.Domain.Exceptions;

namespace VeloBid.Services.Auctions.Domain.ValueObjects;

public sealed record Money
{
    private Money()
    {
        Currency = string.Empty;
    }

    private Money(decimal amount, string currency)
    {
        Amount = amount;
        Currency = currency;
    }

    public decimal Amount { get; }

    public string Currency { get; }

    public static Money Create(decimal amount, string currency)
    {
        if (amount <= 0)
        {
            throw new InvalidMoneyAmountException(amount);
        }

        if (string.IsNullOrWhiteSpace(currency))
        {
            throw new ArgumentException("Currency is required.", nameof(currency));
        }

        return new Money(decimal.Round(amount, 2, MidpointRounding.AwayFromZero), currency.ToUpperInvariant());
    }

    public Money Add(Money other)
    {
        EnsureSameCurrency(other);

        return Create(Amount + other.Amount, Currency);
    }

    public bool IsGreaterThan(Money other)
    {
        EnsureSameCurrency(other);

        return Amount > other.Amount;
    }

    public bool IsGreaterThanOrEqualTo(Money other)
    {
        EnsureSameCurrency(other);

        return Amount >= other.Amount;
    }

    public void EnsureSameCurrency(Money other)
    {
        if (Currency != other.Currency)
        {
            throw new InvalidOperationException($"Currency mismatch. Expected {Currency}, got {other.Currency}.");
        }
    }

    public override string ToString()
    {
        return $"{Amount:N2} {Currency}";
    }
}