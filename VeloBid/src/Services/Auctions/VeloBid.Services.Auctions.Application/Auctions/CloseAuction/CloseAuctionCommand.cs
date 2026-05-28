using MediatR;
using VeloBid.Services.Auctions.Application.Common;
using VeloBid.Services.Auctions.Application.Common.Results;

namespace VeloBid.Services.Auctions.Application.Auctions.CloseAuction;

public sealed record CloseAuctionCommand(Guid AuctionId) : IRequest<Result<CloseAuctionResult>>;