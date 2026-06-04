namespace VeloBid.Services.Auctions.API.Contracts;

public sealed record PlaceBidRequest(
    Guid BidderId,
    decimal Amount,
    string Currency);