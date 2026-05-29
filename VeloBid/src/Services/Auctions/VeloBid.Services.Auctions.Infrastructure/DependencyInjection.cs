using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using VeloBid.Services.Auctions.Application.Abstractions.Clock;
using VeloBid.Services.Auctions.Application.Abstractions.Data;
using VeloBid.Services.Auctions.Application.Abstractions.Persistence;
using VeloBid.Services.Auctions.Infrastructure.Clock;
using VeloBid.Services.Auctions.Infrastructure.Persistence;
using VeloBid.Services.Auctions.Infrastructure.Persistence.Repositories;

namespace VeloBid.Services.Auctions.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddAuctionsInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<AuctionsDbContext>(options =>
        {
            options.UseNpgsql(configuration.GetConnectionString("auctions-db"));
        });

        services.AddScoped<IAuctionRepository, AuctionRepository>();
        services.AddScoped<IAuctionReadRepository, AuctionReadRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddSingleton<IDateTimeProvider, DateTimeProvider>();

        return services;
    }
}