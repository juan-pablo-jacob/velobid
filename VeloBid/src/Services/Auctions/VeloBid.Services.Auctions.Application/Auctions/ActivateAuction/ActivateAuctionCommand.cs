using MediatR;
using VeloBid.Services.Auctions.Application.Common.Results;

namespace VeloBid.Services.Auctions.Application.Auctions.ActivateAuction;

public sealed record ActivateAuctionCommand(Guid AuctionId) : IRequest<Result<ActivateAuctionResult>>;