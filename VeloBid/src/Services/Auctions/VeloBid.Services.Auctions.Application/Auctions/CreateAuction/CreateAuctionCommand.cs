using MediatR;
using VeloBid.Services.Auctions.Application.Common;
using VeloBid.Services.Auctions.Application.Common.Results;

namespace VeloBid.Services.Auctions.Application.Auctions.CreateAuction;


public sealed record CreateAuctionCommand(
    Guid SellerId,
    string Title,
    string Description,
    decimal StartingPrice,
    decimal MinimumBidIncrement,
    string Currency,
    DateTimeOffset StartsAtUtc,
    DateTimeOffset EndsAtUtc) : IRequest<Result<CreateAuctionResult>>;