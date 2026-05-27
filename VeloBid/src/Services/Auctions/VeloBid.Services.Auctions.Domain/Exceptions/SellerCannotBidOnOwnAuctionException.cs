namespace VeloBid.Services.Auctions.Domain.Exceptions;

public class SellerCannotBidOnOwnAuctionException(Guid sellerId, Guid auctionId)
    : DomainException($"Seller '{sellerId}' cannot bid on their own auction '{auctionId}'.");