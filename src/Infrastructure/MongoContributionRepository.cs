using Domain.Entities;
using Domain.Interfaces;

using MongoDB.Driver;

namespace Infrastructure;

public class MongoContributionRepository : MongoBaseRepository<Contribution>, IContributionRepository
{
    public MongoContributionRepository(string dbName, IClientSessionHandle clientSessionHandle)
        : base(clientSessionHandle)
    {
        DbName = dbName;
    }

    protected override string DbName { get; }

    protected override string CollectionName => "contributions";
}