namespace VeloBid.Services.Auctions.Domain.Exceptions;

public class InvalidMoneyAmountException(decimal amount)
    : DomainException($"Money amount must be greater than zero. Provided amount: {amount}.");