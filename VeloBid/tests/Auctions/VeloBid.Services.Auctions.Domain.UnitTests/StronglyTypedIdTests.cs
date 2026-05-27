using VeloBid.Services.Auctions.Domain.ValueObjects;

namespace VeloBid.Services.Auctions.Domain.UnitTests;

public sealed class StronglyTypedIdTests
{
    [Fact]
    public void AuctionId_New_Should_Create_Non_Empty_Id()
    {
        // Act
        var id = AuctionId.New();

        // Assert
        Assert.NotEqual(Guid.Empty, id.Value);
    }

    [Fact]
    public void AuctionId_From_Should_Throw_When_Value_Is_Empty()
    {
        // Act
        var exception = Assert.Throws<ArgumentException>(() =>
            AuctionId.From(Guid.Empty));

        // Assert
        Assert.Equal("value", exception.ParamName);
    }

    [Fact]
    public void BidId_New_Should_Create_Non_Empty_Id()
    {
        // Act
        var id = BidId.New();

        // Assert
        Assert.NotEqual(Guid.Empty, id.Value);
    }

    [Fact]
    public void BidId_From_Should_Throw_When_Value_Is_Empty()
    {
        // Act
        var exception = Assert.Throws<ArgumentException>(() =>
            BidId.From(Guid.Empty));

        // Assert
        Assert.Equal("value", exception.ParamName);
    }

    [Fact]
    public void UserId_From_Should_Create_Id_When_Value_Is_Not_Empty()
    {
        // Arrange
        var value = Guid.NewGuid();

        // Act
        var id = UserId.From(value);

        // Assert
        Assert.Equal(value, id.Value);
    }

    [Fact]
    public void UserId_From_Should_Throw_When_Value_Is_Empty()
    {
        // Act
        var exception = Assert.Throws<ArgumentException>(() =>
            UserId.From(Guid.Empty));

        // Assert
        Assert.Equal("value", exception.ParamName);
    }
}