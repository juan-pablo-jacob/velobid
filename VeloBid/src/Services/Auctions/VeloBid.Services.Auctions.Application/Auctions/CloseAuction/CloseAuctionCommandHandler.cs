using MediatR;
using VeloBid.Services.Auctions.Application.Abstractions.Clock;
using VeloBid.Services.Auctions.Application.Abstractions.Data;
using VeloBid.Services.Auctions.Application.Abstractions.Persistence;
using VeloBid.Services.Auctions.Application.Common;
using VeloBid.Services.Auctions.Domain.ValueObjects;

namespace VeloBid.Services.Auctions.Application.Auctions.CloseAuction;

internal sealed class CloseAuctionCommandHandler(
    IAuctionRepository auctionRepository,
    IUnitOfWork unitOfWork,
    IDateTimeProvider dateTimeProvider)
    : IRequestHandler<CloseAuctionCommand, Result<CloseAuctionResult>>
{
    public async Task<Result<CloseAuctionResult>> Handle(
        CloseAuctionCommand request,
        CancellationToken cancellationToken)
    {
        var auctionId = AuctionId.From(request.AuctionId);

        var auction = await auctionRepository.GetByIdAsync(
            auctionId,
            cancellationToken);

        if (auction is null)
        {
            return Result<CloseAuctionResult>.Failure(
                Error.NotFound($"Auction '{request.AuctionId}' was not found."));
        }

        try
        {
            auction.Close(dateTimeProvider.UtcNow);

            await unitOfWork.SaveChangesAsync(cancellationToken);

            var result = new CloseAuctionResult(
                auction.Id.Value,
                auction.Status,
                auction.WinnerId?.Value,
                auction.WinningAmount?.Amount,
                auction.WinningAmount?.Currency);

            return Result<CloseAuctionResult>.Success(result);
        }
        catch (InvalidOperationException exception)
        {
            return Result<CloseAuctionResult>.Failure(
                Error.Conflict(exception.Message));
        }
    }
}