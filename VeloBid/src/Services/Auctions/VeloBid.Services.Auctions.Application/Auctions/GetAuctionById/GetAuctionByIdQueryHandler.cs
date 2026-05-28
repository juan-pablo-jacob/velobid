using MediatR;
using VeloBid.Services.Auctions.Application.Abstractions.Persistence;
using VeloBid.Services.Auctions.Application.Common.Results;
using VeloBid.Services.Auctions.Domain.ValueObjects;

namespace VeloBid.Services.Auctions.Application.Auctions.GetAuctionById;

internal sealed class GetAuctionByIdQueryHandler(IAuctionReadRepository auctionReadRepository)
    : IRequestHandler<GetAuctionByIdQuery, Result<AuctionDetailsDto>>
{
    public async Task<Result<AuctionDetailsDto>> Handle(
        GetAuctionByIdQuery request,
        CancellationToken cancellationToken)
    {
        var auctionId = AuctionId.From(request.AuctionId);

        var auction = await auctionReadRepository.GetByIdAsync(
            auctionId,
            cancellationToken);

        if (auction is null)
        {
            return Result<AuctionDetailsDto>.Failure(
                Error.NotFound($"Auction '{request.AuctionId}' was not found."));
        }

        return Result<AuctionDetailsDto>.Success(auction);
    }
}