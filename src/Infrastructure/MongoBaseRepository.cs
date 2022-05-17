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

    public async Task<IEnumerable<TModel>> ListAll(CancellationToken cancellationToken)
    {
        IMongoDatabase? db = Client.GetDatabase(DbName);

        IMongoCollection<TModel>? coll = db.GetCollection<TModel>(CollectionName);

        IMongoQueryable<TModel>? docs = coll.AsQueryable().OfType<TModel>();

        return await docs.ToListAsync(cancellationToken);
    }

    public async Task<TModel> FindById(Guid id, CancellationToken token)
    {
        IMongoDatabase? db = Client.GetDatabase(DbName);

        IMongoCollection<TModel>? coll = db.GetCollection<TModel>(CollectionName);

        IMongoQueryable<TModel>? docs =
            coll.AsQueryable().OfType<TModel>().Where(c => c.Id == id);

        return await IAsyncCursorSourceExtensions.FirstAsync(docs, token);
    }

    public async Task<TModel> FindBy(Expression<Func<TModel, bool>> predicate, CancellationToken token)
    {
        IMongoDatabase? db = Client.GetDatabase(DbName);

        IMongoCollection<TModel>? coll = db.GetCollection<TModel>(CollectionName);

        IMongoQueryable<TModel>? docs =
            coll.AsQueryable().OfType<TModel>().Where(predicate);

        return await docs.FirstAsync(token);
    }

    public async Task Update(TModel contribution, CancellationToken token)
    {
        IMongoDatabase? db = Client.GetDatabase(DbName);

        IMongoCollection<TModel>? coll = db.GetCollection<TModel>(CollectionName);

        await coll.ReplaceOneAsync(c => c.Id == contribution.Id, contribution,
            new ReplaceOptions { IsUpsert = false }, token);
    }

    public async Task Delete(TModel entity, CancellationToken token)
    {
        await DeleteById(entity.Id, token);
    }

    public async Task DeleteById(Guid id, CancellationToken token)
    {
        IMongoDatabase? db = Client.GetDatabase(DbName);

        IMongoCollection<TModel>? coll = db.GetCollection<TModel>(CollectionName);

        await coll.DeleteOneAsync(c => c.Id == id, token);
    }

    public async Task<TModel> Add(TModel entity, CancellationToken token)
    {
        IMongoDatabase? db = Client.GetDatabase(DbName);

        IMongoCollection<TModel>? coll = db.GetCollection<TModel>(CollectionName);

        InsertOneOptions options = new() { BypassDocumentValidation = false };

        await coll.InsertOneAsync(entity, options, token);

        return entity;
    }
}