using BookStore.Core.Entities;
using System.Collections.Concurrent;

namespace BookStore.Core.Repositories;

public sealed class InMemoryRepository<T> : IRepository<T> where T : IEntity
{
    private readonly ConcurrentDictionary<Guid, T> _items = new();

    public T Add(T entity)
    {
        if (!_items.TryAdd(entity.Id, entity))
            throw new InvalidOperationException($"An entity with id {entity.Id} already exists.");
        return entity;
    }

    public bool Remove(Guid id) => _items.TryRemove(id, out _);

    public T? GetById(Guid id) => _items.TryGetValue(id, out var item) ? item : default;

    public IReadOnlyList<T> GetAll() => _items.Values.ToList().AsReadOnly();

    public IEnumerable<T> Find(Func<T, bool> predicate) => _items.Values.Where(predicate).ToList();

    public int Count => _items.Count;
}
