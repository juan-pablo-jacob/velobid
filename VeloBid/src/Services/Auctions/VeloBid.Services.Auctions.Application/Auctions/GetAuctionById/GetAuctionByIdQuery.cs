using MediatR;
using VeloBid.Services.Auctions.Application.Common;
using VeloBid.Services.Auctions.Application.Common.Results;

namespace VeloBid.Services.Auctions.Application.Auctions.GetAuctionById;

public sealed record GetAuctionByIdQuery(Guid AuctionId)
    : IRequest<Result<AuctionDetailsDto>>;