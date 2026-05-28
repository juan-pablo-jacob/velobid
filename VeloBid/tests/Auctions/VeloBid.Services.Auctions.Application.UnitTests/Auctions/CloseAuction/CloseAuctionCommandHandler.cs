using VeloBid.Services.Auctions.Application.Auctions.CloseAuction;
using VeloBid.Services.Auctions.Application.UnitTests.Fakes;
using VeloBid.Services.Auctions.Application.UnitTests.TestData;
using VeloBid.Services.Auctions.Domain.Enums;
using VeloBid.Services.Auctions.Domain.ValueObjects;

namespace VeloBid.Services.Auctions.Application.UnitTests.Auctions.CloseAuction;

public sealed class CloseAuctionCommandHandlerTests
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
            UtcNow = FixedNowUtc.AddHours(4)
        };

        var handler = new CloseAuctionCommandHandler(
            repository,
            unitOfWork,
            dateTimeProvider);

        var command = new CloseAuctionCommand(Guid.NewGuid());

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal("not_found", result.Error.Code);
        Assert.Equal(0, unitOfWork.SaveChangesCallCount);
    }

    [Fact]
    public async Task Handle_Should_Expire_Auction_When_There_Are_No_Bids()
    {
        // Arrange
        var auction = AuctionFactory.CreateActiveAuction(FixedNowUtc);

        var repository = new FakeAuctionRepository();
        repository.Seed(auction);

        var unitOfWork = new FakeUnitOfWork();
        var dateTimeProvider = new FakeDateTimeProvider
        {
            UtcNow = FixedNowUtc.AddHours(4)
        };

        var handler = new CloseAuctionCommandHandler(
            repository,
            unitOfWork,
            dateTimeProvider);

        var command = new CloseAuctionCommand(auction.Id.Value);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(AuctionStatus.Expired, result.Value.Status);
        Assert.Null(result.Value.WinnerId);
        Assert.Null(result.Value.WinningAmount);
        Assert.Null(result.Value.Currency);

        Assert.Equal(AuctionStatus.Expired, auction.Status);
        Assert.Equal(1, unitOfWork.SaveChangesCallCount);
    }

    [Fact]
    public async Task Handle_Should_Complete_Auction_When_There_Is_A_Winning_Bid()
    {
        // Arrange
        var auction = AuctionFactory.CreateActiveAuction(FixedNowUtc);
        var winnerId = UserId.From(Guid.NewGuid());

        auction.PlaceBid(
            winnerId,
            Money.Create(1_000m, "EUR"),
            FixedNowUtc.AddMinutes(1));

        var repository = new FakeAuctionRepository();
        repository.Seed(auction);

        var unitOfWork = new FakeUnitOfWork();
        var dateTimeProvider = new FakeDateTimeProvider
        {
            UtcNow = FixedNowUtc.AddHours(4)
        };

        var handler = new CloseAuctionCommandHandler(
            repository,
            unitOfWork,
            dateTimeProvider);

        var command = new CloseAuctionCommand(auction.Id.Value);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(AuctionStatus.Completed, result.Value.Status);
        Assert.Equal(winnerId.Value, result.Value.WinnerId);
        Assert.Equal(1_000m, result.Value.WinningAmount);
        Assert.Equal("EUR", result.Value.Currency);

        Assert.Equal(AuctionStatus.Completed, auction.Status);
        Assert.Equal(winnerId, auction.WinnerId);
        Assert.Equal(1, unitOfWork.SaveChangesCallCount);
    }

    [Fact]
    public async Task Handle_Should_Return_Conflict_When_Auction_End_Date_Has_Not_Been_Reached()
    {
        // Arrange
        var auction = AuctionFactory.CreateActiveAuction(FixedNowUtc);

        var repository = new FakeAuctionRepository();
        repository.Seed(auction);

        var unitOfWork = new FakeUnitOfWork();
        var dateTimeProvider = new FakeDateTimeProvider
        {
            UtcNow = FixedNowUtc.AddHours(1)
        };

        var handler = new CloseAuctionCommandHandler(
            repository,
            unitOfWork,
            dateTimeProvider);

        var command = new CloseAuctionCommand(auction.Id.Value);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal("conflict", result.Error.Code);
        Assert.Equal(AuctionStatus.Active, auction.Status);
        Assert.Equal(0, unitOfWork.SaveChangesCallCount);
    }
}