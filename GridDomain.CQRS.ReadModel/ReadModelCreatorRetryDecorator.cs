using System;
using System.Data.Entity.Infrastructure;

namespace GridDomain.CQRS.ReadModel
{
    public class ReadModelCreatorRetryDecorator<T> : IReadModelCreator<T>
    {
        private readonly IReadModelCreator<T> _origCreator;
        private readonly RetryPolicy<DbUpdateConcurrencyException> _retryPolicy;

        public ReadModelCreatorRetryDecorator(IReadModelCreator<T> origCreator)
        {
            _origCreator = origCreator;
            _retryPolicy = RetryPolicy<DbUpdateConcurrencyException>.DefaultSql();
        }

        public void Add(T entity)
        {
            _retryPolicy.Execute(() => _origCreator.Add(entity));
        }

        public void Modify(Guid id, Action<T> modificator)
        {
            _retryPolicy.Execute(() => _origCreator.Modify(id, modificator));
        }
    }
}