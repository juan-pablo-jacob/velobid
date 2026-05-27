namespace VeloBid.Services.Auctions.Domain.Exceptions;

public class BidTooLowException(decimal bidAmount, decimal minimumRequiredAmount, string currency)
    : DomainException(
        $"Bid amount {bidAmount} {currency} is too low. Minimum required amount is {minimumRequiredAmount} {currency}.");