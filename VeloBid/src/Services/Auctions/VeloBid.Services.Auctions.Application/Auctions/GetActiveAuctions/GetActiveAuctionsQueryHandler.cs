using MediatR;
using VeloBid.Services.Auctions.Application.Abstractions.Persistence;

namespace VeloBid.Services.Auctions.Application.Auctions.GetActiveAuctions;


internal sealed class GetActiveAuctionsQueryHandler(IAuctionReadRepository auctionReadRepository)
    : IRequestHandler<GetActiveAuctionsQuery, IReadOnlyCollection<AuctionSummaryDto>>
{
    public async Task<IReadOnlyCollection<AuctionSummaryDto>> Handle(
        GetActiveAuctionsQuery request,
        CancellationToken cancellationToken)
    {
        return await auctionReadRepository.GetActiveAuctionsAsync(cancellationToken);
    }
}