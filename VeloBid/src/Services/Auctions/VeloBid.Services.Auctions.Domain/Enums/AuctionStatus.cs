namespace VeloBid.Services.Auctions.Domain.Enums;

public enum AuctionStatus
{
    Draft = 0, // Not published yet. Pending Confirmation.
    Scheduled = 1, // Published. Auction is scheduled to start.
    Active = 2, // Auction is live.
    Completed = 3, // Auction is over. Final bids are published.
    Expired = 4, // Auction is over. Final bids are not published.
    Canceled = 5 // Auction was manually canceled.
}