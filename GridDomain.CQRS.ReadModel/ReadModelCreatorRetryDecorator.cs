using System;
using System.Data.Entity.Infrastructure;
using Polly;

namespace GridDomain.CQRS.ReadModel
{
    public class ReadModelCreatorRetryDecorator<T> : IReadModelCreator<T>
    {
        private readonly IReadModelCreator<T> _origCreator;

        public ReadModelCreatorRetryDecorator(IReadModelCreator<T> origCreator)
        {
            _origCreator = origCreator;
        }

        public void Add(T entity)
        {
            Policy.Handle<DbUpdateConcurrencyException>()
                  .Retry(5)
                  .Execute(() => _origCreator.Add(entity));
        }

        public void Modify(Guid id, Action<T> modificator)
        {
            Policy.Handle<DbUpdateConcurrencyException>()
                  .Retry(5)
                  .Execute(() => _origCreator.Modify(id, modificator));
        }
    }
}