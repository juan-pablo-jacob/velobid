using MediatR;
using VeloBid.Services.Auctions.API.Contracts;
using VeloBid.Services.Auctions.Application.Auctions.ActivateAuction;
using VeloBid.Services.Auctions.Application.Auctions.CloseAuction;
using VeloBid.Services.Auctions.Application.Auctions.CreateAuction;
using VeloBid.Services.Auctions.Application.Auctions.GetActiveAuctions;
using VeloBid.Services.Auctions.Application.Auctions.GetAuctionById;

namespace VeloBid.Services.Auctions.API.Endpoints;

internal static class AuctionEndpoints
{
    public static IEndpointRouteBuilder MapAuctionEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app
            .MapGroup("/api/auctions")
            .WithTags("Auctions");

        group.MapPost("/", CreateAuction)
            .WithName("CreateAuction")
            .WithSummary("Creates a new auction.");

        group.MapGet("/{auctionId:guid}", GetAuctionById)
            .WithName("GetAuctionById")
            .WithSummary("Gets auction details by id.");
        
        group.MapPost("/{auctionId:guid}/activate", ActivateAuction)
            .WithName("ActivateAuction")
            .WithSummary("Activates an auction.");

        group.MapGet("/active", GetActiveAuctions)
            .WithName("GetActiveAuctions")
            .WithSummary("Gets active auctions.");

        group.MapPost("/{auctionId:guid}/close", CloseAuction)
            .WithName("CloseAuction")
            .WithSummary("Closes an auction.");

        return app;
    }

    private static async Task<IResult> CreateAuction(
        CreateAuctionRequest request,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var command = new CreateAuctionCommand(
            request.SellerId,
            request.Title,
            request.Description,
            request.StartingPrice,
            request.MinimumBidIncrement,
            request.Currency,
            request.StartsAtUtc,
            request.EndsAtUtc);

        var result = await sender.Send(command, cancellationToken);

        return result.IsSuccess
            ? Results.Created($"/api/auctions/{result.Value!.AuctionId}", result.Value)
            : result.ToHttpResult();
    }

    private static async Task<IResult> GetAuctionById(
        Guid auctionId,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var query = new GetAuctionByIdQuery(auctionId);

        var result = await sender.Send(query, cancellationToken);

        return result.ToHttpResult();
    }

    private static async Task<IResult> GetActiveAuctions(
        ISender sender,
        CancellationToken cancellationToken)
    {
        var query = new GetActiveAuctionsQuery();

        var result = await sender.Send(query, cancellationToken);

        return Results.Ok(result);
    }

    private static async Task<IResult> ActivateAuction(
        Guid auctionId,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var command = new ActivateAuctionCommand(auctionId);

        var result = await sender.Send(command, cancellationToken);

        return result.ToHttpResult();
    }

    private static async Task<IResult> CloseAuction(
        Guid auctionId,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var command = new CloseAuctionCommand(auctionId);

        var result = await sender.Send(command, cancellationToken);

        return result.ToHttpResult();
    }
}