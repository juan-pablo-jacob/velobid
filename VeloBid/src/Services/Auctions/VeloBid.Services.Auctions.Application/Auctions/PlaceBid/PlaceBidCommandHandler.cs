using MediatR;
using VeloBid.Services.Auctions.Application.Abstractions.Clock;
using VeloBid.Services.Auctions.Application.Abstractions.Data;
using VeloBid.Services.Auctions.Application.Abstractions.Persistence;
using VeloBid.Services.Auctions.Application.Common;
using VeloBid.Services.Auctions.Domain.Exceptions;
using VeloBid.Services.Auctions.Domain.ValueObjects;

namespace VeloBid.Services.Auctions.Application.Auctions.PlaceBid;



internal sealed class PlaceBidCommandHandler(
    IAuctionRepository auctionRepository,
    IUnitOfWork unitOfWork,
    IDateTimeProvider dateTimeProvider)
    : IRequestHandler<PlaceBidCommand, Result<PlaceBidResult>>
{
    public async Task<Result<PlaceBidResult>> Handle(
        PlaceBidCommand request,
        CancellationToken cancellationToken)
    {
        var auctionId = AuctionId.From(request.AuctionId);

        var auction = await auctionRepository.GetByIdAsync(
            auctionId,
            cancellationToken);

        if (auction is null)
        {
            return Result<PlaceBidResult>.Failure(
                Error.NotFound($"Auction '{request.AuctionId}' was not found."));
        }

        try
        {
            var nowUtc = dateTimeProvider.UtcNow;

            var bid = auction.PlaceBid(
                UserId.From(request.BidderId),
                Money.Create(request.Amount, request.Currency),
                nowUtc);

            await unitOfWork.SaveChangesAsync(cancellationToken);

            var result = new PlaceBidResult(
                auction.Id.Value,
                bid.Id.Value,
                bid.BidderId.Value,
                bid.Amount.Amount,
                bid.Amount.Currency,
                bid.PlacedAtUtc,
                auction.Period.EndsAtUtc);

            return Result<PlaceBidResult>.Success(result);
        }
        catch (BidTooLowException exception)
        {
            return Result<PlaceBidResult>.Failure(
                Error.Conflict(exception.Message));
        }
        catch (AuctionNotActiveException exception)
        {
            return Result<PlaceBidResult>.Failure(
                Error.Conflict(exception.Message));
        }
        catch (DomainException exception)
        {
            return Result<PlaceBidResult>.Failure(
                Error.Validation(exception.Message));
        }
        catch (ArgumentException exception)
        {
            return Result<PlaceBidResult>.Failure(
                Error.Validation(exception.Message));
        }
        catch (InvalidOperationException exception)
        {
            return Result<PlaceBidResult>.Failure(
                Error.Validation(exception.Message));
        }
    }
}