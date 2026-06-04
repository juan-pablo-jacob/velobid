using VeloBid.Services.Auctions.Domain.Enums;

namespace VeloBid.Services.Auctions.Application.Auctions.GetAuctionById;

public sealed class AuctionDetailsDto
{
    public Guid Id { get; init; }

    public Guid SellerId { get; init; }

    public string Title { get; init; } = string.Empty;

    public string Description { get; init; } = string.Empty;

    public AuctionStatus Status { get; init; }

    public decimal StartingPrice { get; init; }

    public decimal CurrentPrice { get; init; }

    public string Currency { get; init; } = string.Empty;

    public DateTime StartsAtUtc { get; init; }

    public DateTime EndsAtUtc { get; init; }

    public Guid? WinnerId { get; init; }

    public decimal? WinningAmount { get; init; }
}