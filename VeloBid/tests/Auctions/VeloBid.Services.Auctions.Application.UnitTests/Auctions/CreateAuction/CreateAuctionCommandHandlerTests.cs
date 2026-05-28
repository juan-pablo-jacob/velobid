using VeloBid.Services.Auctions.Application.Auctions.CreateAuction;
using VeloBid.Services.Auctions.Application.UnitTests.Fakes;
using VeloBid.Services.Auctions.Domain.Enums;

namespace VeloBid.Services.Auctions.Application.UnitTests.Auctions.CreateAuction;

public sealed class CreateAuctionCommandHandlerTests
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
    public async Task Handle_Should_Create_Auction_And_Save_Changes()
    {
        // Arrange
        var repository = new FakeAuctionRepository();
        var unitOfWork = new FakeUnitOfWork();
        var dateTimeProvider = new FakeDateTimeProvider
        {
            UtcNow = FixedNowUtc
        };

        var handler = new CreateAuctionCommandHandler(
            repository,
            unitOfWork,
            dateTimeProvider);

        var command = new CreateAuctionCommand(
            Guid.NewGuid(),
            "Carbon road bike",
            "A lightweight carbon road bike.",
            1_000m,
            10m,
            "EUR",
            FixedNowUtc.AddHours(1),
            FixedNowUtc.AddHours(4));

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(AuctionStatus.Scheduled, result.Value.Status);
        Assert.Equal(FixedNowUtc.AddHours(1), result.Value.StartsAtUtc);
        Assert.Equal(FixedNowUtc.AddHours(4), result.Value.EndsAtUtc);

        Assert.Equal(1, repository.AddCallCount);
        Assert.Equal(1, unitOfWork.SaveChangesCallCount);
        Assert.NotNull(repository.LastAddedAuction);
    }

    [Fact]
    public async Task Handle_Should_Return_Validation_Error_When_Auction_Period_Is_Invalid()
    {
        // Arrange
        var repository = new FakeAuctionRepository();
        var unitOfWork = new FakeUnitOfWork();
        var dateTimeProvider = new FakeDateTimeProvider
        {
            UtcNow = FixedNowUtc
        };

        var handler = new CreateAuctionCommandHandler(
            repository,
            unitOfWork,
            dateTimeProvider);

        var command = new CreateAuctionCommand(
            Guid.NewGuid(),
            "Carbon road bike",
            "A lightweight carbon road bike.",
            1_000m,
            10m,
            "EUR",
            FixedNowUtc.AddHours(1),
            FixedNowUtc.AddMinutes(30));

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal("validation_error", result.Error.Code);
        Assert.Equal(0, repository.AddCallCount);
        Assert.Equal(0, unitOfWork.SaveChangesCallCount);
    }

    [Fact]
    public async Task Handle_Should_Return_Validation_Error_When_Starting_Price_Is_Invalid()
    {
        // Arrange
        var repository = new FakeAuctionRepository();
        var unitOfWork = new FakeUnitOfWork();
        var dateTimeProvider = new FakeDateTimeProvider
        {
            UtcNow = FixedNowUtc
        };

        var handler = new CreateAuctionCommandHandler(
            repository,
            unitOfWork,
            dateTimeProvider);

        var command = new CreateAuctionCommand(
            Guid.NewGuid(),
            "Carbon road bike",
            "A lightweight carbon road bike.",
            0m,
            10m,
            "EUR",
            FixedNowUtc.AddHours(1),
            FixedNowUtc.AddHours(4));

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal("validation_error", result.Error.Code);
        Assert.Equal(0, repository.AddCallCount);
        Assert.Equal(0, unitOfWork.SaveChangesCallCount);
    }
}