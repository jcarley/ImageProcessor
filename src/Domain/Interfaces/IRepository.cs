using System.Linq.Expressions;

namespace Domain.Interfaces;

public interface IRepository<T>
{
    Task<IEnumerable<T>> ListAll(CancellationToken token);
    Task<T> FindById(Guid id, CancellationToken token);
    Task<T> FindBy(Expression<Func<T, bool>> predicate, CancellationToken token);
    Task<T> Add(T entity, CancellationToken token);
    Task Update(T contribution, CancellationToken token);
    Task Delete(T contribution, CancellationToken token);
    Task DeleteById(Guid id, CancellationToken token);
}