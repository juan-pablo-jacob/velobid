using VeloBid.Services.Auctions.Application.Auctions.GetActiveAuctions;
using VeloBid.Services.Auctions.Application.UnitTests.Fakes;
using VeloBid.Services.Auctions.Domain.Enums;

namespace VeloBid.Services.Auctions.Application.UnitTests.Auctions.GetActiveAuctions;

public sealed class GetActiveAuctionsQueryHandlerTests
{
    [Fact]
    public async Task Handle_Should_Return_Active_Auctions()
    {
        // Arrange
        var readRepository = new FakeAuctionReadRepository();

        var activeAuctions = new List<AuctionSummaryDto>
        {
            new(
                Guid.NewGuid(),
                "Carbon road bike",
                AuctionStatus.Active,
                1_000m,
                "EUR",
                DateTimeOffset.UtcNow.AddHours(2),
                3),
            new(
                Guid.NewGuid(),
                "Gravel bike",
                AuctionStatus.Active,
                750m,
                "EUR",
                DateTimeOffset.UtcNow.AddHours(4),
                1)
        };

        readRepository.SeedActiveAuctions(activeAuctions);

        var handler = new GetActiveAuctionsQueryHandler(readRepository);

        var query = new GetActiveAuctionsQuery();

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.Equal(2, result.Count);
        Assert.All(result, auction =>
            Assert.Equal(AuctionStatus.Active, auction.Status));
    }

    [Fact]
    public async Task Handle_Should_Return_Empty_Collection_When_There_Are_No_Active_Auctions()
    {
        // Arrange
        var readRepository = new FakeAuctionReadRepository();

        var handler = new GetActiveAuctionsQueryHandler(readRepository);

        var query = new GetActiveAuctionsQuery();

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.Empty(result);
    }
}