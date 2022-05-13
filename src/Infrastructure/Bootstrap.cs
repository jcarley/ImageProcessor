using Domain.Entities;
using Domain.Interfaces;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using MongoDB.Bson.Serialization;
using MongoDB.Driver;

namespace Infrastructure;

public static class Bootstrap
{
    public static IServiceCollection AddContributionRepository(this IServiceCollection services)
    {
        services.AddTransient<IUnitOfWork, MongoUnitOfWork>();

        return services;
    }

    public static IServiceCollection RegisterMongo(this IServiceCollection services, IConfiguration config)
    {
        BsonClassMap.RegisterClassMap<Contribution>(cm =>
        {
            cm.AutoMap();
            cm.SetIsRootClass(true);
        });

        IConfigurationSection? mongoDbSettings = config.GetSection("MongoDB");

        if (mongoDbSettings == null)
        {
            throw new Exception("Missing MongoDB Settings in config");
        }

        string username = mongoDbSettings["MongoDBUser"];
        string password = mongoDbSettings["MongoDBPassword"];
        string authSource = mongoDbSettings["MongoDBAuthSource"];

        Console.WriteLine($"***MONGO USER:{username}***");
        Console.WriteLine($"***MONGO PWD:{password}***");

        services.AddSingleton(s =>
            new MongoClient(
                $"mongodb://{username}:{password}@localhost:27017/?authSource={authSource}&readPreference=primary&ssl=false"));

        return services;
    }
}