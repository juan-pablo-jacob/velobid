using MediatR;
using VeloBid.Services.Auctions.Application.Abstractions.Clock;
using VeloBid.Services.Auctions.Application.Abstractions.Data;
using VeloBid.Services.Auctions.Application.Abstractions.Persistence;
using VeloBid.Services.Auctions.Application.Common.Results;
using VeloBid.Services.Auctions.Domain.Aggregates;
using VeloBid.Services.Auctions.Domain.Exceptions;
using VeloBid.Services.Auctions.Domain.ValueObjects;

namespace VeloBid.Services.Auctions.Application.Auctions.CreateAuction;

internal sealed class CreateAuctionCommandHandler(
    IAuctionRepository auctionRepository,
    IUnitOfWork unitOfWork,
    IDateTimeProvider dateTimeProvider)
    : IRequestHandler<CreateAuctionCommand, Result<CreateAuctionResult>>
{
    public async Task<Result<CreateAuctionResult>> Handle(
        CreateAuctionCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            var auction = Auction.Create(
                UserId.From(request.SellerId),
                request.Title,
                request.Description,
                Money.Create(request.StartingPrice, request.Currency),
                Money.Create(request.MinimumBidIncrement, request.Currency),
                AuctionPeriod.Create(request.StartsAtUtc, request.EndsAtUtc),
                dateTimeProvider.UtcNow);

            await auctionRepository.AddAsync(auction, cancellationToken);

            await unitOfWork.SaveChangesAsync(cancellationToken);

            var result = new CreateAuctionResult(
                auction.Id.Value,
                auction.Status,
                auction.Period.StartsAtUtc,
                auction.Period.EndsAtUtc);

            return Result<CreateAuctionResult>.Success(result);
        }
        catch (DomainException exception)
        {
            return Result<CreateAuctionResult>.Failure(
                Error.Validation(exception.Message));
        }
        catch (ArgumentException exception)
        {
            return Result<CreateAuctionResult>.Failure(
                Error.Validation(exception.Message));
        }
        catch (InvalidOperationException exception)
        {
            return Result<CreateAuctionResult>.Failure(
                Error.Validation(exception.Message));
        }
    }
}