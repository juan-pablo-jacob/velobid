using MediatR;
using VeloBid.Services.Auctions.Application.Common;

namespace VeloBid.Services.Auctions.Application.Auctions.PlaceBid;

public sealed record PlaceBidCommand(
    Guid AuctionId,
    Guid BidderId,
    decimal Amount,
    string Currency) : IRequest<Result<PlaceBidResult>>;