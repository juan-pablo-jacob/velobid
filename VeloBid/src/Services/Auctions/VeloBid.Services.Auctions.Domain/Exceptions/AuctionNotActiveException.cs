namespace VeloBid.Services.Auctions.Domain.Exceptions;

public class AuctionNotActiveException(Guid auctionId) : DomainException($"Auction '{auctionId}' is not active.");