using VeloBid.Services.Auctions.Domain.Entities;
using VeloBid.Services.Auctions.Domain.Enums;
using VeloBid.Services.Auctions.Domain.Events;
using VeloBid.Services.Auctions.Domain.Exceptions;
using VeloBid.Services.Auctions.Domain.Primitives;
using VeloBid.Services.Auctions.Domain.ValueObjects;

namespace VeloBid.Services.Auctions.Domain.Aggregates;

public sealed class Auction : AggregateRoot<AuctionId>
{
    private readonly List<Bid> _bids = [];

    private Auction(
        AuctionId id,
        UserId sellerId,
        string title,
        string description,
        Money startingPrice,
        Money minimumBidIncrement,
        AuctionPeriod period,
        AuctionStatus status,
        DateTimeOffset createdAtUtc)
        : base(id)
    {
        SellerId = sellerId;
        Title = title;
        Description = description;
        StartingPrice = startingPrice;
        MinimumBidIncrement = minimumBidIncrement;
        Period = period;
        Status = status;
        CreatedAtUtc = createdAtUtc;
    }

    public UserId SellerId { get; }

    public string Title { get; }

    public string Description { get; }

    public Money StartingPrice { get; }

    public Money MinimumBidIncrement { get; }

    public AuctionPeriod Period { get; private set; }

    public AuctionStatus Status { get; private set; }

    public DateTimeOffset CreatedAtUtc { get; }

    public DateTimeOffset? ActivatedAtUtc { get; private set; }

    public DateTimeOffset? CompletedAtUtc { get; private set; }

    public DateTimeOffset? ExpiredAtUtc { get; private set; }

    public UserId? WinnerId { get; private set; }

    public Money? WinningAmount { get; private set; }

    public IReadOnlyCollection<Bid> Bids => _bids.AsReadOnly();

    public Money CurrentPrice => HighestBid?.Amount ?? StartingPrice;

    public Bid? HighestBid => _bids
        .OrderByDescending(bid => bid.Amount.Amount)
        .ThenBy(bid => bid.PlacedAtUtc)
        .FirstOrDefault();

    public static Auction Create(
        UserId sellerId,
        string title,
        string description,
        Money startingPrice,
        Money minimumBidIncrement,
        AuctionPeriod period,
        DateTimeOffset nowUtc)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            throw new ArgumentException("Auction title is required.", nameof(title));
        }

        if (string.IsNullOrWhiteSpace(description))
        {
            throw new ArgumentException("Auction description is required.", nameof(description));
        }

        startingPrice.EnsureSameCurrency(minimumBidIncrement);

        var initialStatus = period.StartsAtUtc > nowUtc
            ? AuctionStatus.Scheduled
            : AuctionStatus.Draft;

        var auction = new Auction(
            AuctionId.New(),
            sellerId,
            title.Trim(),
            description.Trim(),
            startingPrice,
            minimumBidIncrement,
            period,
            initialStatus,
            nowUtc);

        auction.RaiseDomainEvent(new AuctionCreatedDomainEvent(
            auction.Id,
            auction.SellerId,
            auction.Status,
            auction.Period.StartsAtUtc,
            auction.Period.EndsAtUtc,
            nowUtc));

        return auction;
    }

    public void Activate(DateTimeOffset nowUtc)
    {
        if (Status == AuctionStatus.Active)
        {
            return;
        }

        if (Status is AuctionStatus.Completed or AuctionStatus.Expired or AuctionStatus.Canceled)
        {
            throw new InvalidOperationException($"Auction '{Id}' cannot be activated from status '{Status}'.");
        }

        if (!Period.HasStarted(nowUtc))
        {
            throw new InvalidOperationException($"Auction '{Id}' cannot be activated before its start date.");
        }

        Status = AuctionStatus.Active;
        ActivatedAtUtc = nowUtc;

        RaiseDomainEvent(new AuctionActivatedDomainEvent(
            Id,
            nowUtc));
    }

    public Bid PlaceBid(
        UserId bidderId,
        Money amount,
        DateTimeOffset nowUtc)
    {
        if (bidderId == SellerId)
        {
            throw new SellerCannotBidOnOwnAuctionException(SellerId.Value, Id.Value);
        }

        if (Status != AuctionStatus.Active)
        {
            throw new AuctionNotActiveException(Id.Value);
        }

        if (Period.HasEnded(nowUtc))
        {
            throw new AuctionNotActiveException(Id.Value);
        }

        amount.EnsureSameCurrency(StartingPrice);

        if (HighestBid is null)
        {
            if (!amount.IsGreaterThanOrEqualTo(StartingPrice))
            {
                throw new BidTooLowException(
                    amount.Amount,
                    StartingPrice.Amount,
                    amount.Currency);
            }
        }
        else
        {
            var minimumRequiredBid = HighestBid.Amount.Add(MinimumBidIncrement);

            if (!amount.IsGreaterThan(minimumRequiredBid))
            {
                throw new BidTooLowException(
                    amount.Amount,
                    minimumRequiredBid.Amount,
                    amount.Currency);
            }
        }

        var bid = Bid.Create(
            Id,
            bidderId,
            amount,
            nowUtc);

        _bids.Add(bid);

        var wasExtended = false;

        if (Period.IsWithinAntiSnipingWindow(nowUtc))
        {
            Period = Period.ExtendForAntiSniping();
            wasExtended = true;
        }

        RaiseDomainEvent(new BidPlacedDomainEvent(
            Id,
            bidderId,
            amount,
            nowUtc,
            Period.EndsAtUtc,
            wasExtended,
            nowUtc));

        return bid;
    }

    public void Close(DateTimeOffset nowUtc)
    {
        if (Status is AuctionStatus.Completed or AuctionStatus.Expired)
        {
            return;
        }

        if (Status == AuctionStatus.Canceled)
        {
            throw new InvalidOperationException($"Auction '{Id}' cannot be closed because it is cancelled.");
        }

        if (!Period.HasEnded(nowUtc))
        {
            throw new InvalidOperationException($"Auction '{Id}' cannot be closed before its end date.");
        }

        var highestBid = HighestBid;

        if (highestBid is null)
        {
            Status = AuctionStatus.Expired;
            ExpiredAtUtc = nowUtc;

            RaiseDomainEvent(new AuctionExpiredDomainEvent(
                Id,
                nowUtc,
                nowUtc));

            return;
        }

        Status = AuctionStatus.Completed;
        CompletedAtUtc = nowUtc;
        WinnerId = highestBid.BidderId;
        WinningAmount = highestBid.Amount;

        RaiseDomainEvent(new AuctionCompletedDomainEvent(
            Id,
            highestBid.BidderId,
            highestBid.Amount,
            nowUtc,
            nowUtc));
    }

    public void Cancel(DateTimeOffset nowUtc)
    {
        if (Status is AuctionStatus.Completed or AuctionStatus.Expired)
        {
            throw new InvalidOperationException($"Auction '{Id}' cannot be cancelled from status '{Status}'.");
        }

        Status = AuctionStatus.Canceled;
    }
}