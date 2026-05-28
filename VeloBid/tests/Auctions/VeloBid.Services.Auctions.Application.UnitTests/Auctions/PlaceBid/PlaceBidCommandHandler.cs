using VeloBid.Services.Auctions.Application.Auctions.PlaceBid;
using VeloBid.Services.Auctions.Application.UnitTests.Fakes;
using VeloBid.Services.Auctions.Application.UnitTests.TestData;
using VeloBid.Services.Auctions.Domain.ValueObjects;

namespace VeloBid.Services.Auctions.Application.UnitTests.Auctions.PlaceBid;

public sealed class PlaceBidCommandHandlerTests
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
    public async Task Handle_Should_Return_NotFound_When_Auction_Does_Not_Exist()
    {
        // Arrange
        var repository = new FakeAuctionRepository();
        var unitOfWork = new FakeUnitOfWork();
        var dateTimeProvider = new FakeDateTimeProvider
        {
            UtcNow = FixedNowUtc
        };

        var handler = new PlaceBidCommandHandler(
            repository,
            unitOfWork,
            dateTimeProvider);

        var command = new PlaceBidCommand(
            Guid.NewGuid(),
            Guid.NewGuid(),
            1_000m,
            "EUR");

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal("not_found", result.Error.Code);
        Assert.Equal(0, unitOfWork.SaveChangesCallCount);
    }

    [Fact]
    public async Task Handle_Should_Place_Bid_And_Save_Changes()
    {
        // Arrange
        var repository = new FakeAuctionRepository();
        var unitOfWork = new FakeUnitOfWork();
        var dateTimeProvider = new FakeDateTimeProvider
        {
            UtcNow = FixedNowUtc.AddMinutes(5)
        };

        var auction = AuctionFactory.CreateActiveAuction(FixedNowUtc);

        repository.Seed(auction);

        var handler = new PlaceBidCommandHandler(
            repository,
            unitOfWork,
            dateTimeProvider);

        var bidderId = Guid.NewGuid();

        var command = new PlaceBidCommand(
            auction.Id.Value,
            bidderId,
            1_000m,
            "EUR");

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(auction.Id.Value, result.Value.AuctionId);
        Assert.Equal(bidderId, result.Value.BidderId);
        Assert.Equal(1_000m, result.Value.Amount);
        Assert.Equal("EUR", result.Value.Currency);
        Assert.Equal(dateTimeProvider.UtcNow, result.Value.PlacedAtUtc);

        Assert.Single(auction.Bids);
        Assert.Equal(1, unitOfWork.SaveChangesCallCount);
    }

    [Fact]
    public async Task Handle_Should_Return_Conflict_When_Auction_Is_Not_Active()
    {
        // Arrange
        var repository = new FakeAuctionRepository();
        var unitOfWork = new FakeUnitOfWork();
        var dateTimeProvider = new FakeDateTimeProvider
        {
            UtcNow = FixedNowUtc.AddHours(1)
        };

        var auction = AuctionFactory.CreateScheduledAuction(FixedNowUtc);

        repository.Seed(auction);

        var handler = new PlaceBidCommandHandler(
            repository,
            unitOfWork,
            dateTimeProvider);

        var command = new PlaceBidCommand(
            auction.Id.Value,
            Guid.NewGuid(),
            1_000m,
            "EUR");

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal("conflict", result.Error.Code);
        Assert.Empty(auction.Bids);
        Assert.Equal(0, unitOfWork.SaveChangesCallCount);
    }

    [Fact]
    public async Task Handle_Should_Return_Validation_Error_When_Bidder_Is_Seller()
    {
        // Arrange
        var sellerId = UserId.From(Guid.NewGuid());
        var auction = AuctionFactory.CreateActiveAuction(FixedNowUtc, sellerId);

        var repository = new FakeAuctionRepository();
        repository.Seed(auction);

        var unitOfWork = new FakeUnitOfWork();
        var dateTimeProvider = new FakeDateTimeProvider
        {
            UtcNow = FixedNowUtc.AddMinutes(5)
        };

        var handler = new PlaceBidCommandHandler(
            repository,
            unitOfWork,
            dateTimeProvider);

        var command = new PlaceBidCommand(
            auction.Id.Value,
            sellerId.Value,
            1_000m,
            "EUR");

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal("validation_error", result.Error.Code);
        Assert.Empty(auction.Bids);
        Assert.Equal(0, unitOfWork.SaveChangesCallCount);
    }

    [Fact]
    public async Task Handle_Should_Return_Conflict_When_Bid_Is_Too_Low()
    {
        // Arrange
        var auction = AuctionFactory.CreateActiveAuction(FixedNowUtc);

        auction.PlaceBid(
            UserId.From(Guid.NewGuid()),
            Money.Create(1_000m, "EUR"),
            FixedNowUtc.AddMinutes(1));

        var repository = new FakeAuctionRepository();
        repository.Seed(auction);

        var unitOfWork = new FakeUnitOfWork();
        var dateTimeProvider = new FakeDateTimeProvider
        {
            UtcNow = FixedNowUtc.AddMinutes(2)
        };

        var handler = new PlaceBidCommandHandler(
            repository,
            unitOfWork,
            dateTimeProvider);

        var command = new PlaceBidCommand(
            auction.Id.Value,
            Guid.NewGuid(),
            1_010m,
            "EUR");

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal("conflict", result.Error.Code);
        Assert.Single(auction.Bids);
        Assert.Equal(0, unitOfWork.SaveChangesCallCount);
    }
}