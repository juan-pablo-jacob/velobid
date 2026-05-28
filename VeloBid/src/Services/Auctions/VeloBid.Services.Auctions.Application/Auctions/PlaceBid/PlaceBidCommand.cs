using MediatR;
using VeloBid.Services.Auctions.Application.Common;
using VeloBid.Services.Auctions.Application.Common.Results;

namespace VeloBid.Services.Auctions.Application.Auctions.PlaceBid;

public sealed record PlaceBidCommand(
    Guid AuctionId,
    Guid BidderId,
    decimal Amount,
    string Currency) : IRequest<Result<PlaceBidResult>>;