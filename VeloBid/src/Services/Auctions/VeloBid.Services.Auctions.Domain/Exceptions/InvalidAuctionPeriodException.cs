namespace VeloBid.Services.Auctions.Domain.Exceptions;

public class InvalidAuctionPeriodException(string reason) : DomainException($"Invalid auction period. {reason}");