using VeloBid.Services.Auctions.Domain.Aggregates;
using VeloBid.Services.Auctions.Domain.Enums;
using VeloBid.Services.Auctions.Domain.Events;
using VeloBid.Services.Auctions.Domain.Exceptions;
using VeloBid.Services.Auctions.Domain.ValueObjects;

namespace VeloBid.Services.Auctions.Domain.UnitTests;

public sealed class AuctionTests
{
    private static readonly DateTimeOffset FixedNowUtc = new(
        2026,
        01,
        01,
        12,
        00,
        00,
        TimeSpan.Zero);

    [Fact]
    public void Create_Should_Create_Scheduled_Auction_When_Start_Date_Is_In_The_Future()
    {
        // Arrange
        var sellerId = NewUserId();
        var period = AuctionPeriod.Create(
            FixedNowUtc.AddHours(1),
            FixedNowUtc.AddHours(3));

        // Act
        var auction = Auction.Create(
            sellerId,
            "Carbon road bike",
            "A lightweight carbon road bike.",
            Money.Create(1_000m, "EUR"),
            Money.Create(10m, "EUR"),
            period,
            FixedNowUtc);

        // Assert
        Assert.Equal(AuctionStatus.Scheduled, auction.Status);
        Assert.Equal(sellerId, auction.SellerId);
        Assert.Equal("Carbon road bike", auction.Title);
        Assert.Equal("A lightweight carbon road bike.", auction.Description);
        Assert.Equal(1_000m, auction.StartingPrice.Amount);
        Assert.Equal(10m, auction.MinimumBidIncrement.Amount);
        Assert.Empty(auction.Bids);
        Assert.Null(auction.WinnerId);
        Assert.Null(auction.WinningAmount);

        var domainEvent = Assert.Single(auction.DomainEvents);
        var auctionCreated = Assert.IsType<AuctionCreatedDomainEvent>(domainEvent);
        Assert.Equal(auction.Id, auctionCreated.AuctionId);
        Assert.Equal(AuctionStatus.Scheduled, auctionCreated.InitialStatus);
    }

    [Fact]
    public void Create_Should_Create_Draft_Auction_When_Start_Date_Is_Not_In_The_Future()
    {
        // Arrange
        var sellerId = NewUserId();
        var period = AuctionPeriod.Create(
            FixedNowUtc,
            FixedNowUtc.AddHours(3));

        // Act
        var auction = Auction.Create(
            sellerId,
            "Gravel bike",
            "A reliable gravel bike.",
            Money.Create(900m, "EUR"),
            Money.Create(10m, "EUR"),
            period,
            FixedNowUtc);

        // Assert
        Assert.Equal(AuctionStatus.Draft, auction.Status);

        var domainEvent = Assert.Single(auction.DomainEvents);
        var auctionCreated = Assert.IsType<AuctionCreatedDomainEvent>(domainEvent);
        Assert.Equal(AuctionStatus.Draft, auctionCreated.InitialStatus);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void Create_Should_Throw_When_Title_Is_Empty(string title)
    {
        // Arrange
        var period = AuctionPeriod.Create(
            FixedNowUtc,
            FixedNowUtc.AddHours(2));

        // Act
        var exception = Assert.Throws<ArgumentException>(() =>
            Auction.Create(
                NewUserId(),
                title,
                "Description",
                Money.Create(100m, "EUR"),
                Money.Create(5m, "EUR"),
                period,
                FixedNowUtc));

        // Assert
        Assert.Equal("title", exception.ParamName);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void Create_Should_Throw_When_Description_Is_Empty(string description)
    {
        // Arrange
        var period = AuctionPeriod.Create(
            FixedNowUtc,
            FixedNowUtc.AddHours(2));

        // Act
        var exception = Assert.Throws<ArgumentException>(() =>
            Auction.Create(
                NewUserId(),
                "Auction title",
                description,
                Money.Create(100m, "EUR"),
                Money.Create(5m, "EUR"),
                period,
                FixedNowUtc));

        // Assert
        Assert.Equal("description", exception.ParamName);
    }

    [Fact]
    public void Activate_Should_Transition_Auction_To_Active_When_Start_Date_Has_Been_Reached()
    {
        // Arrange
        var auction = CreateScheduledAuction();

        // Act
        auction.Activate(FixedNowUtc.AddHours(1));

        // Assert
        Assert.Equal(AuctionStatus.Active, auction.Status);
        Assert.Equal(FixedNowUtc.AddHours(1), auction.ActivatedAtUtc);

        Assert.Contains(
            auction.DomainEvents,
            domainEvent => domainEvent is AuctionActivatedDomainEvent);
    }

    [Fact]
    public void Activate_Should_Throw_When_Start_Date_Has_Not_Been_Reached()
    {
        // Arrange
        var auction = CreateScheduledAuction();

        // Act
        var exception = Assert.Throws<InvalidOperationException>(() =>
            auction.Activate(FixedNowUtc.AddMinutes(30)));

        // Assert
        Assert.Contains("cannot be activated before its start date", exception.Message);
        Assert.Equal(AuctionStatus.Scheduled, auction.Status);
        Assert.Null(auction.ActivatedAtUtc);
    }

    [Fact]
    public void PlaceBid_Should_Throw_When_Auction_Is_Not_Active()
    {
        // Arrange
        var auction = CreateScheduledAuction();

        // Act
        var exception = Assert.Throws<AuctionNotActiveException>(() =>
            auction.PlaceBid(
                NewUserId(),
                Money.Create(1_000m, "EUR"),
                FixedNowUtc.AddHours(1)));

        // Assert
        Assert.Contains("is not active", exception.Message);
        Assert.Empty(auction.Bids);
    }

    [Fact]
    public void PlaceBid_Should_Throw_When_Bidder_Is_The_Seller()
    {
        // Arrange
        var sellerId = NewUserId();
        var auction = CreateActiveAuction(sellerId);

        // Act
        var exception = Assert.Throws<SellerCannotBidOnOwnAuctionException>(() =>
            auction.PlaceBid(
                sellerId,
                Money.Create(1_000m, "EUR"),
                FixedNowUtc.AddHours(1).AddMinutes(1)));

        // Assert
        Assert.Contains("cannot bid on their own auction", exception.Message);
        Assert.Empty(auction.Bids);
    }

    [Fact]
    public void PlaceBid_Should_Accept_First_Bid_When_Amount_Equals_Starting_Price()
    {
        // Arrange
        var auction = CreateActiveAuction();

        // Act
        var bid = auction.PlaceBid(
            NewUserId(),
            Money.Create(1_000m, "EUR"),
            FixedNowUtc.AddHours(1).AddMinutes(1));

        // Assert
        Assert.Single(auction.Bids);
        Assert.Equal(bid, auction.HighestBid);
        Assert.Equal(1_000m, auction.CurrentPrice.Amount);
        Assert.Equal("EUR", auction.CurrentPrice.Currency);

        var bidPlaced = Assert.IsType<BidPlacedDomainEvent>(
            auction.DomainEvents.Last());

        Assert.Equal(auction.Id, bidPlaced.AuctionId);
        Assert.Equal(1_000m, bidPlaced.Amount.Amount);
        Assert.False(bidPlaced.AuctionWasExtended);
    }

    [Fact]
    public void PlaceBid_Should_Throw_When_Second_Bid_Is_Not_Strictly_Greater_Than_Highest_Bid_Plus_Minimum_Increment()
    {
        // Arrange
        var auction = CreateActiveAuction();

        auction.PlaceBid(
            NewUserId(),
            Money.Create(1_000m, "EUR"),
            FixedNowUtc.AddHours(1).AddMinutes(1));

        // highest bid = 1000
        // minimum increment = 10
        // required by current rule = strictly greater than 1010
        var insufficientBid = Money.Create(1_010m, "EUR");

        // Act
        var exception = Assert.Throws<BidTooLowException>(() =>
            auction.PlaceBid(
                NewUserId(),
                insufficientBid,
                FixedNowUtc.AddHours(1).AddMinutes(2)));

        // Assert
        Assert.Contains("is too low", exception.Message);
        Assert.Single(auction.Bids);
        Assert.Equal(1_000m, auction.CurrentPrice.Amount);
    }

    [Fact]
    public void PlaceBid_Should_Accept_Second_Bid_When_Amount_Is_Strictly_Greater_Than_Highest_Bid_Plus_Minimum_Increment()
    {
        // Arrange
        var auction = CreateActiveAuction();

        auction.PlaceBid(
            NewUserId(),
            Money.Create(1_000m, "EUR"),
            FixedNowUtc.AddHours(1).AddMinutes(1));

        // Act
        var secondBid = auction.PlaceBid(
            NewUserId(),
            Money.Create(1_010.01m, "EUR"),
            FixedNowUtc.AddHours(1).AddMinutes(2));

        // Assert
        Assert.Equal(2, auction.Bids.Count);
        Assert.Equal(secondBid, auction.HighestBid);
        Assert.Equal(1_010.01m, auction.CurrentPrice.Amount);
    }

    [Fact]
    public void PlaceBid_Should_Extend_Auction_When_Bid_Is_Placed_Within_Last_Two_Minutes()
    {
        // Arrange
        var auction = CreateActiveAuction();

        var originalEndsAtUtc = auction.Period.EndsAtUtc;
        var bidPlacedAtUtc = originalEndsAtUtc.AddMinutes(-1);

        // Act
        auction.PlaceBid(
            NewUserId(),
            Money.Create(1_000m, "EUR"),
            bidPlacedAtUtc);

        // Assert
        Assert.Equal(originalEndsAtUtc.AddMinutes(2), auction.Period.EndsAtUtc);

        var bidPlaced = Assert.IsType<BidPlacedDomainEvent>(
            auction.DomainEvents.Last());

        Assert.True(bidPlaced.AuctionWasExtended);
        Assert.Equal(originalEndsAtUtc.AddMinutes(2), bidPlaced.AuctionEndsAtUtc);
    }

    [Fact]
    public void Close_Should_Expire_Auction_When_There_Are_No_Bids()
    {
        // Arrange
        var auction = CreateActiveAuction();

        // Act
        auction.Close(FixedNowUtc.AddHours(4));

        // Assert
        Assert.Equal(AuctionStatus.Expired, auction.Status);
        Assert.Equal(FixedNowUtc.AddHours(4), auction.ExpiredAtUtc);
        Assert.Null(auction.WinnerId);
        Assert.Null(auction.WinningAmount);

        Assert.Contains(
            auction.DomainEvents,
            domainEvent => domainEvent is AuctionExpiredDomainEvent);
    }

    [Fact]
    public void Close_Should_Complete_Auction_And_Determine_Winner_When_There_Are_Bids()
    {
        // Arrange
        var auction = CreateActiveAuction();

        var firstBidderId = NewUserId();
        var secondBidderId = NewUserId();

        auction.PlaceBid(
            firstBidderId,
            Money.Create(1_000m, "EUR"),
            FixedNowUtc.AddHours(1).AddMinutes(1));

        auction.PlaceBid(
            secondBidderId,
            Money.Create(1_020m, "EUR"),
            FixedNowUtc.AddHours(1).AddMinutes(2));

        // Act
        auction.Close(FixedNowUtc.AddHours(4));

        // Assert
        Assert.Equal(AuctionStatus.Completed, auction.Status);
        Assert.Equal(FixedNowUtc.AddHours(4), auction.CompletedAtUtc);
        Assert.Equal(secondBidderId, auction.WinnerId);
        Assert.NotNull(auction.WinningAmount);
        Assert.Equal(1_020m, auction.WinningAmount.Amount);

        var completedEvent = auction.DomainEvents
            .OfType<AuctionCompletedDomainEvent>()
            .Single();

        Assert.Equal(auction.Id, completedEvent.AuctionId);
        Assert.Equal(secondBidderId, completedEvent.WinnerId);
        Assert.Equal(1_020m, completedEvent.WinningAmount.Amount);
    }

    private static Auction CreateScheduledAuction()
    {
        return Auction.Create(
            NewUserId(),
            "Carbon road bike",
            "A lightweight carbon road bike.",
            Money.Create(1_000m, "EUR"),
            Money.Create(10m, "EUR"),
            AuctionPeriod.Create(
                FixedNowUtc.AddHours(1),
                FixedNowUtc.AddHours(4)),
            FixedNowUtc);
    }

    private static Auction CreateActiveAuction()
    {
        return CreateActiveAuction(NewUserId());
    }

    private static Auction CreateActiveAuction(UserId sellerId)
    {
        var auction = Auction.Create(
            sellerId,
            "Carbon road bike",
            "A lightweight carbon road bike.",
            Money.Create(1_000m, "EUR"),
            Money.Create(10m, "EUR"),
            AuctionPeriod.Create(
                FixedNowUtc,
                FixedNowUtc.AddHours(3)),
            FixedNowUtc);

        auction.Activate(FixedNowUtc);

        return auction;
    }

    private static UserId NewUserId()
    {
        return UserId.From(Guid.NewGuid());
    }
}