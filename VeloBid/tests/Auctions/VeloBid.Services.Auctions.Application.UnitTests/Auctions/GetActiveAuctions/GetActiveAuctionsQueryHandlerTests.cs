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
            new()
            {
                Id = Guid.NewGuid(),
                Title = "Carbon road bike",
                Status = AuctionStatus.Active,
                CurrentPrice = 1_000m,
                Currency = "EUR",
                EndsAtUtc = DateTime.UtcNow.AddHours(2),
                TotalBids = 3
            },
            new()
            {
                Id = Guid.NewGuid(),
                Title = "Gravel bike",
                Status = AuctionStatus.Active,
                CurrentPrice = 750m,
                Currency = "EUR",
                EndsAtUtc = DateTime.UtcNow.AddHours(4),
                TotalBids = 1
            }
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