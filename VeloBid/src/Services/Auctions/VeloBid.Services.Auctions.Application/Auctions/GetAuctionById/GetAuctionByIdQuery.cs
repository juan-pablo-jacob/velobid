using MediatR;
using VeloBid.Services.Auctions.Application.Common;

namespace VeloBid.Services.Auctions.Application.Auctions.GetAuctionById;

public sealed record GetAuctionByIdQuery(Guid AuctionId)
    : IRequest<Result<AuctionDetailsDto>>;