using System;

namespace GridDomain.CQRS.ReadModel
{
    public interface IReadModelCreator<T>
    {
        void Add(T entity);
        void Modify(Guid id, Action<T> modificator);
    }
}