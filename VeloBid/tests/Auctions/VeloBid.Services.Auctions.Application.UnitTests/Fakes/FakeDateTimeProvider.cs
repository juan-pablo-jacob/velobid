using VeloBid.Services.Auctions.Application.Abstractions.Clock;

namespace VeloBid.Services.Auctions.Application.UnitTests.Fakes;

internal sealed class FakeDateTimeProvider : IDateTimeProvider
{
    public DateTimeOffset UtcNow { get; set; }
}