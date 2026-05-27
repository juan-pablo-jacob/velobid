using VeloBid.Services.Auctions.Domain.Exceptions;
using VeloBid.Services.Auctions.Domain.ValueObjects;

namespace VeloBid.Services.Auctions.Domain.UnitTests;

public sealed class AuctionPeriodTests
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
    public void Create_Should_Create_AuctionPeriod_When_Duration_Is_Valid()
    {
        // Arrange
        var startsAtUtc = FixedNowUtc;
        var endsAtUtc = FixedNowUtc.AddHours(2);

        // Act
        var period = AuctionPeriod.Create(startsAtUtc, endsAtUtc);

        // Assert
        Assert.Equal(startsAtUtc, period.StartsAtUtc);
        Assert.Equal(endsAtUtc, period.EndsAtUtc);
        Assert.Equal(TimeSpan.FromHours(2), period.Duration);
    }

    [Fact]
    public void Create_Should_Throw_When_Start_Date_Is_After_End_Date()
    {
        // Arrange
        var startsAtUtc = FixedNowUtc.AddHours(2);
        var endsAtUtc = FixedNowUtc;

        // Act
        var exception = Assert.Throws<InvalidAuctionPeriodException>(() =>
            AuctionPeriod.Create(startsAtUtc, endsAtUtc));

        // Assert
        Assert.Contains("Start date must be earlier than end date", exception.Message);
    }

    [Fact]
    public void Create_Should_Throw_When_Duration_Is_Less_Than_One_Hour()
    {
        // Arrange
        var startsAtUtc = FixedNowUtc;
        var endsAtUtc = FixedNowUtc.AddMinutes(59);

        // Act
        var exception = Assert.Throws<InvalidAuctionPeriodException>(() =>
            AuctionPeriod.Create(startsAtUtc, endsAtUtc));

        // Assert
        Assert.Contains("Minimum duration", exception.Message);
    }

    [Fact]
    public void Create_Should_Throw_When_Duration_Is_Greater_Than_Seven_Days()
    {
        // Arrange
        var startsAtUtc = FixedNowUtc;
        var endsAtUtc = FixedNowUtc.AddDays(7).AddSeconds(1);

        // Act
        var exception = Assert.Throws<InvalidAuctionPeriodException>(() =>
            AuctionPeriod.Create(startsAtUtc, endsAtUtc));

        // Assert
        Assert.Contains("Maximum duration", exception.Message);
    }

    [Fact]
    public void HasStarted_Should_Return_True_When_Current_Time_Is_Equal_To_Start_Date()
    {
        // Arrange
        var period = AuctionPeriod.Create(
            FixedNowUtc,
            FixedNowUtc.AddHours(2));

        // Act
        var hasStarted = period.HasStarted(FixedNowUtc);

        // Assert
        Assert.True(hasStarted);
    }

    [Fact]
    public void HasEnded_Should_Return_True_When_Current_Time_Is_Equal_To_End_Date()
    {
        // Arrange
        var period = AuctionPeriod.Create(
            FixedNowUtc,
            FixedNowUtc.AddHours(2));

        // Act
        var hasEnded = period.HasEnded(FixedNowUtc.AddHours(2));

        // Assert
        Assert.True(hasEnded);
    }

    [Fact]
    public void IsWithinAntiSnipingWindow_Should_Return_True_When_Remaining_Time_Is_Two_Minutes()
    {
        // Arrange
        var period = AuctionPeriod.Create(
            FixedNowUtc,
            FixedNowUtc.AddHours(2));

        // Act
        var isWithinAntiSnipingWindow = period.IsWithinAntiSnipingWindow(
            FixedNowUtc.AddHours(2).AddMinutes(-2));

        // Assert
        Assert.True(isWithinAntiSnipingWindow);
    }

    [Fact]
    public void IsWithinAntiSnipingWindow_Should_Return_False_When_Remaining_Time_Is_More_Than_Two_Minutes()
    {
        // Arrange
        var period = AuctionPeriod.Create(
            FixedNowUtc,
            FixedNowUtc.AddHours(2));

        // Act
        var isWithinAntiSnipingWindow = period.IsWithinAntiSnipingWindow(
            FixedNowUtc.AddHours(2).AddMinutes(-2).AddSeconds(-1));

        // Assert
        Assert.False(isWithinAntiSnipingWindow);
    }

    [Fact]
    public void ExtendForAntiSniping_Should_Extend_End_Date_By_Two_Minutes()
    {
        // Arrange
        var period = AuctionPeriod.Create(
            FixedNowUtc,
            FixedNowUtc.AddHours(2));

        // Act
        var extendedPeriod = period.ExtendForAntiSniping();

        // Assert
        Assert.Equal(period.StartsAtUtc, extendedPeriod.StartsAtUtc);
        Assert.Equal(period.EndsAtUtc.AddMinutes(2), extendedPeriod.EndsAtUtc);
    }
}