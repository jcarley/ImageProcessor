using Domain.Entities;
using Domain.Interfaces;

using MongoDB.Driver;

namespace Infrastructure;

public class MongoContributionRepository : MongoBaseRepository<Contribution>, IContributionRepository
{
    public MongoContributionRepository(IClientSessionHandle clientSessionHandle)
        : base(clientSessionHandle)
    {
    }

    protected override string DbName => "image_processor";
    protected override string CollectionName => "contributions";
}