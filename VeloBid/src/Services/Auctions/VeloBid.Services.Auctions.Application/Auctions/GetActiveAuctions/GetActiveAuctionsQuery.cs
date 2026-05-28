using MediatR;

namespace VeloBid.Services.Auctions.Application.Auctions.GetActiveAuctions;

public sealed record GetActiveAuctionsQuery()
    : IRequest<IReadOnlyCollection<AuctionSummaryDto>>;