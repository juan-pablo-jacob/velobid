using MediatR;
using VeloBid.Services.Auctions.Application.Abstractions.Clock;
using VeloBid.Services.Auctions.Application.Abstractions.Data;
using VeloBid.Services.Auctions.Application.Abstractions.Persistence;
using VeloBid.Services.Auctions.Application.Common.Results;
using VeloBid.Services.Auctions.Domain.ValueObjects;

namespace VeloBid.Services.Auctions.Application.Auctions.ActivateAuction;

internal sealed class ActivateAuctionCommandHandler(
    IAuctionRepository auctionRepository,
    IUnitOfWork unitOfWork,
    IDateTimeProvider dateTimeProvider)
    : IRequestHandler<ActivateAuctionCommand, Result<ActivateAuctionResult>>
{
    public async Task<Result<ActivateAuctionResult>> Handle(
        ActivateAuctionCommand request,
        CancellationToken cancellationToken)
    {
        var auctionId = AuctionId.From(request.AuctionId);

        var auction = await auctionRepository.GetByIdAsync(
            auctionId,
            cancellationToken);

        if (auction is null)
        {
            return Result<ActivateAuctionResult>.Failure(
                Error.NotFound($"Auction '{request.AuctionId}' was not found."));
        }

        try
        {
            auction.Activate(dateTimeProvider.UtcNow);

            await unitOfWork.SaveChangesAsync(cancellationToken);

            var result = new ActivateAuctionResult(
                auction.Id.Value,
                auction.Status,
                auction.WinnerId?.Value,
                auction.WinningAmount?.Amount,
                auction.WinningAmount?.Currency);

            return Result<ActivateAuctionResult>.Success(result);
        }
        catch (InvalidOperationException exception)
        {
            return Result<ActivateAuctionResult>.Failure(
                Error.Conflict(exception.Message));
        }
    }
}