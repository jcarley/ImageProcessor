using Domain.Commands;

using MediatR;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Domain;

public static class Bootstrap
{
    public static IServiceCollection AddCommands(this IServiceCollection services, IConfiguration config)
    {
        services.AddMediatR(nameof(AddContributionCommand).GetType().Assembly);

        return services;
    }
}