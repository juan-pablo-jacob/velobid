using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using VeloBid.Services.Auctions.Application.Behaviors;

namespace VeloBid.Services.Auctions.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddAuctionsApplication(this IServiceCollection services)
    {
        var assembly = typeof(DependencyInjection).Assembly;

        services.AddMediatR(configuration =>
        {
            configuration.RegisterServicesFromAssembly(assembly);
        });

        services.AddValidatorsFromAssembly(assembly);

        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

        return services;
    }
}