using System.Linq.Expressions;

using Domain.Interfaces;

using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace Infrastructure;

public abstract class MongoBaseRepository<TModel> : IRepository<TModel> where TModel : IIdentity
{
    private readonly IClientSessionHandle _clientSessionHandle;

    protected MongoBaseRepository(IClientSessionHandle clientSessionHandle)
    {
        _clientSessionHandle = clientSessionHandle;
    }

    private IMongoClient Client => _clientSessionHandle.Client;

    protected abstract string DbName { get; }

    protected abstract string CollectionName { get; }

    public async Task<IEnumerable<TModel>> ListAll()
    {
        IMongoDatabase? db = Client.GetDatabase(DbName);

        IMongoCollection<TModel>? coll = db.GetCollection<TModel>(CollectionName);

        IMongoQueryable<TModel>? docs = coll.AsQueryable().OfType<TModel>();

        return await docs.ToListAsync();
    }

    public async Task<TModel> FindById(Guid id)
    {
        IMongoDatabase? db = Client.GetDatabase(DbName);

        IMongoCollection<TModel>? coll = db.GetCollection<TModel>(CollectionName);

        IMongoQueryable<TModel>? docs = coll.AsQueryable().OfType<TModel>()
            .Where(c => c.Id == id);

        return await docs.FirstAsync();
    }

    public async Task<TModel> FindBy(Expression<Func<TModel, bool>> predicate)
    {
        IMongoDatabase? db = Client.GetDatabase(DbName);

        IMongoCollection<TModel>? coll = db.GetCollection<TModel>(CollectionName);

        IMongoQueryable<TModel>? docs = coll.AsQueryable().OfType<TModel>()
            .Where(predicate);

        return await docs.FirstAsync();
    }

    public async Task Add(TModel contribution)
    {
        // This is only an example of how to do multi-document transactions in Mongo.  Single
        // document inserts/updates are atomic out of the box.
        IClientSessionHandle? clientSessionHandle = await Client.StartSessionAsync();

        await clientSessionHandle.WithTransactionAsync(async (session, cancellationToken) =>
        {
            IMongoClient? client = session.Client;

            IMongoDatabase? db = client.GetDatabase(DbName);

            IMongoCollection<TModel>? coll = db.GetCollection<TModel>(CollectionName);

            InsertOneOptions options = new() { BypassDocumentValidation = false };

            await coll.InsertOneAsync(contribution, options, cancellationToken);

            return contribution;
        });
    }

    public async Task Update(TModel contribution)
    {
        IMongoDatabase? db = Client.GetDatabase(DbName);

        IMongoCollection<TModel>? coll = db.GetCollection<TModel>(CollectionName);

        await coll.ReplaceOneAsync(c => c.Id == contribution.Id, contribution,
            new ReplaceOptions { IsUpsert = false });
    }

    public async Task Delete(TModel entity)
    {
        await DeleteById(entity.Id);
    }

    public async Task DeleteById(Guid id)
    {
        IMongoDatabase? db = Client.GetDatabase(DbName);

        IMongoCollection<TModel>? coll = db.GetCollection<TModel>(CollectionName);

        await coll.DeleteOneAsync(c => c.Id == id);
    }
}