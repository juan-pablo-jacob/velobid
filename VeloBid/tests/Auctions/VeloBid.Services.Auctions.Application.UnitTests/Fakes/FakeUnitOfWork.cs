using VeloBid.Services.Auctions.Application.Abstractions.Data;

namespace VeloBid.Services.Auctions.Application.UnitTests.Fakes;

internal sealed class FakeUnitOfWork : IUnitOfWork
{
    public int SaveChangesCallCount { get; private set; }

    public Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        SaveChangesCallCount++;

        return Task.CompletedTask;
    }
}