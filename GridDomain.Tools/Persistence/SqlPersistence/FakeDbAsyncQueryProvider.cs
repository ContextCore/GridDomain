using System.Linq;

namespace GridDomain.Tools.Persistence.SqlPersistence
{
  
    public class FakeDbAsyncQueryProvider<TEntity> : System.Data.Entity.Infrastructure.IDbAsyncQueryProvider
    {
        private readonly IQueryProvider _inner;

        public FakeDbAsyncQueryProvider(IQueryProvider inner)
        {
            _inner = inner;
        }

        public IQueryable CreateQuery(System.Linq.Expressions.Expression expression)
        {
            return new FakeDbAsyncEnumerable<TEntity>(expression);
        }

        public IQueryable<TElement> CreateQuery<TElement>(System.Linq.Expressions.Expression expression)
        {
            return new FakeDbAsyncEnumerable<TElement>(expression);
        }

        public object Execute(System.Linq.Expressions.Expression expression)
        {
            return _inner.Execute(expression);
        }

        public TResult Execute<TResult>(System.Linq.Expressions.Expression expression)
        {
            return _inner.Execute<TResult>(expression);
        }

        public System.Threading.Tasks.Task<object> ExecuteAsync(System.Linq.Expressions.Expression expression, System.Threading.CancellationToken cancellationToken)
        {
            return System.Threading.Tasks.Task.FromResult(Execute(expression));
        }

        public System.Threading.Tasks.Task<TResult> ExecuteAsync<TResult>(System.Linq.Expressions.Expression expression, System.Threading.CancellationToken cancellationToken)
        {
            return System.Threading.Tasks.Task.FromResult(Execute<TResult>(expression));
        }
    }
}