using System.Reflection;

using Domain.Commands;

using MediatR;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Domain;

public static class Bootstrap
{
    public static IServiceCollection AddCommands(this IServiceCollection services, IConfiguration config)
    {
        Type type = typeof(AddContributionCommandHandler);
        Assembly declaredAssembly = type.Assembly;

        services.AddMediatR(declaredAssembly);

        return services;
    }
}