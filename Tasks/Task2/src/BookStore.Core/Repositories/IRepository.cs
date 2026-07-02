using BookStore.Core.Entities;

namespace BookStore.Core.Repositories;

public interface IRepository<T> where T : IEntity
{
    T Add(T entity);
    bool Remove(Guid id);
    T? GetById(Guid id);
    IReadOnlyList<T> GetAll();
    IEnumerable<T> Find(Func<T, bool> predicate);
    int Count { get; }
}
