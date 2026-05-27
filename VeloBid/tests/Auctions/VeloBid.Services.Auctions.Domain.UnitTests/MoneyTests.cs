using VeloBid.Services.Auctions.Domain.Exceptions;
using VeloBid.Services.Auctions.Domain.ValueObjects;

namespace VeloBid.Services.Auctions.Domain.UnitTests;

public sealed class MoneyTests
{
    [Fact]
    public void Create_Should_Create_Money_When_Amount_Is_Greater_Than_Zero()
    {
        // Act
        var money = Money.Create(10.567m, "eur");

        // Assert
        Assert.Equal(10.57m, money.Amount);
        Assert.Equal("EUR", money.Currency);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Create_Should_Throw_When_Amount_Is_Not_Greater_Than_Zero(decimal amount)
    {
        // Act
        var exception = Assert.Throws<InvalidMoneyAmountException>(() =>
            Money.Create(amount, "EUR"));

        // Assert
        Assert.Contains("must be greater than zero", exception.Message);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void Create_Should_Throw_When_Currency_Is_Empty(string currency)
    {
        // Act
        var exception = Assert.Throws<ArgumentException>(() =>
            Money.Create(10m, currency));

        // Assert
        Assert.Equal("currency", exception.ParamName);
    }

    [Fact]
    public void Add_Should_Return_Sum_When_Currency_Is_The_Same()
    {
        // Arrange
        var first = Money.Create(10m, "EUR");
        var second = Money.Create(5m, "EUR");

        // Act
        var result = first.Add(second);

        // Assert
        Assert.Equal(15m, result.Amount);
        Assert.Equal("EUR", result.Currency);
    }

    [Fact]
    public void Add_Should_Throw_When_Currency_Is_Different()
    {
        // Arrange
        var first = Money.Create(10m, "EUR");
        var second = Money.Create(5m, "USD");

        // Act
        var exception = Assert.Throws<InvalidOperationException>(() =>
            first.Add(second));

        // Assert
        Assert.Contains("Currency mismatch", exception.Message);
    }

    [Fact]
    public void IsGreaterThan_Should_Return_True_When_Amount_Is_Greater()
    {
        // Arrange
        var first = Money.Create(11m, "EUR");
        var second = Money.Create(10m, "EUR");

        // Act
        var result = first.IsGreaterThan(second);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void IsGreaterThanOrEqualTo_Should_Return_True_When_Amounts_Are_Equal()
    {
        // Arrange
        var first = Money.Create(10m, "EUR");
        var second = Money.Create(10m, "EUR");

        // Act
        var result = first.IsGreaterThanOrEqualTo(second);

        // Assert
        Assert.True(result);
    }
}