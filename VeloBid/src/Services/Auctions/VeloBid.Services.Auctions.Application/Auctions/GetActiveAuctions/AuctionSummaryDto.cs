using VeloBid.Services.Auctions.Domain.Enums;

namespace VeloBid.Services.Auctions.Application.Auctions.GetActiveAuctions;

public sealed class AuctionSummaryDto
{
    public Guid Id { get; init; }

    public string Title { get; init; } = string.Empty;

    public AuctionStatus Status { get; init; }

    public decimal CurrentPrice { get; init; }

    public string Currency { get; init; } = string.Empty;

    public DateTime EndsAtUtc { get; init; }

    public int TotalBids { get; init; }
}