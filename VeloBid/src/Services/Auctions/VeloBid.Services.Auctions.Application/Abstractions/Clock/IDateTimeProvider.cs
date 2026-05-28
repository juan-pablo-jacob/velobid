namespace VeloBid.Services.Auctions.Application.Abstractions.Clock;

/// <summary>
/// For Unit Testing Purposes
/// </summary>
public interface IDateTimeProvider
{
    DateTimeOffset UtcNow { get; }
}