using Domain.Entities;
using Domain.Interfaces;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using MongoDB.Bson.Serialization;

namespace Infrastructure;

public static class Bootstrap
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
    {
        services.AddTransient<IUnitOfWork, MongoUnitOfWork>();
        services.AddTransient<IEventPublisher, MongoDbTransactionalPublisher>();

        return services;
    }

    public static IServiceCollection RegisterMongo(this IServiceCollection services, IConfiguration config)
    {
        BsonClassMap.RegisterClassMap<Contribution>(cm =>
        {
            cm.AutoMap();
            cm.SetIsRootClass(true);
        });

        services.Configure<MongoDBSettings>(config.GetSection("MongoDB"));

        return services;
    }
}