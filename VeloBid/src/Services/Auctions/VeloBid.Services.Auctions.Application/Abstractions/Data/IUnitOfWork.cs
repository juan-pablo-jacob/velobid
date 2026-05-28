namespace VeloBid.Services.Auctions.Application.Abstractions.Data;

public interface IUnitOfWork
{
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}