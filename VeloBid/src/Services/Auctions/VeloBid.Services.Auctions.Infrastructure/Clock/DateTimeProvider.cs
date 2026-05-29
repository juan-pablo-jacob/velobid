using VeloBid.Services.Auctions.Application.Abstractions.Clock;

namespace VeloBid.Services.Auctions.Infrastructure.Clock;

public class DateTimeProvider : IDateTimeProvider
{
    public DateTimeOffset UtcNow => DateTimeOffset.UtcNow;
}