using System.Linq.Expressions;

namespace Domain.Interfaces;

public interface IRepository<T>
{
    Task<IEnumerable<T>> ListAll();
    Task<T> FindById(Guid id);
    Task<T> FindBy(Expression<Func<T, bool>> predicate);
    Task Add(T contribution);
    Task Update(T contribution);
    Task Delete(T contribution);
    Task DeleteById(Guid id);
}