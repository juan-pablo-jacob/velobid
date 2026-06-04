using MediatR;
using VeloBid.Services.Auctions.API.Contracts;
using VeloBid.Services.Auctions.Application.Auctions.PlaceBid;

namespace VeloBid.Services.Auctions.API.Endpoints;

internal static class BidEndpoints
{
    public static IEndpointRouteBuilder MapBidEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app
            .MapGroup("/api/auctions/{auctionId:guid}/bids")
            .WithTags("Bids");

        group.MapPost("/", PlaceBid)
            .WithName("PlaceBid")
            .WithSummary("Places a bid on an active auction.");

        return app;
    }

    private static async Task<IResult> PlaceBid(
        Guid auctionId,
        PlaceBidRequest request,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var command = new PlaceBidCommand(
            auctionId,
            request.BidderId,
            request.Amount,
            request.Currency);

        var result = await sender.Send(command, cancellationToken);

        return result.IsSuccess
            ? Results.Created($"/api/auctions/{auctionId}/bids/{result.Value!.BidId}", result.Value)
            : result.ToHttpResult();
    }
}