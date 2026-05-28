using VeloBid.Services.Auctions.Application.Auctions.GetAuctionById;
using VeloBid.Services.Auctions.Application.UnitTests.Fakes;
using VeloBid.Services.Auctions.Domain.Enums;

namespace VeloBid.Services.Auctions.Application.UnitTests.Auctions.GetAuctionById;

public sealed class GetAuctionByIdQueryHandlerTests
{
    [Fact]
    public async Task Handle_Should_Return_NotFound_When_Auction_Does_Not_Exist()
    {
        // Arrange
        var readRepository = new FakeAuctionReadRepository();

        var handler = new GetAuctionByIdQueryHandler(readRepository);

        var query = new GetAuctionByIdQuery(Guid.NewGuid());

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal("not_found", result.Error.Code);
    }

    [Fact]
    public async Task Handle_Should_Return_Auction_Details_When_Auction_Exists()
    {
        // Arrange
        var readRepository = new FakeAuctionReadRepository();

        var auctionId = Guid.NewGuid();
        var sellerId = Guid.NewGuid();

        var auction = new AuctionDetailsDto(
            auctionId,
            sellerId,
            "Carbon road bike",
            "A lightweight carbon road bike.",
            AuctionStatus.Active,
            1_000m,
            1_250m,
            "EUR",
            DateTimeOffset.UtcNow.AddHours(-1),
            DateTimeOffset.UtcNow.AddHours(2),
            null,
            null);

        readRepository.Seed(auction);

        var handler = new GetAuctionByIdQueryHandler(readRepository);

        var query = new GetAuctionByIdQuery(auctionId);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(auctionId, result.Value.Id);
        Assert.Equal(sellerId, result.Value.SellerId);
        Assert.Equal("Carbon road bike", result.Value.Title);
        Assert.Equal(AuctionStatus.Active, result.Value.Status);
        Assert.Equal(1_250m, result.Value.CurrentPrice);
    }
}