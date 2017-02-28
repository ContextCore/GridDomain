using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Linq.Expressions;

namespace GridDomain.Tools.Persistence.SqlPersistence
{
    public class FakeDbAsyncEnumerable<T> : EnumerableQuery<T>,
                                            IDbAsyncEnumerable<T>,
                                            IQueryable<T>
    {
        public FakeDbAsyncEnumerable(IEnumerable<T> enumerable) : base(enumerable) {}

        public FakeDbAsyncEnumerable(Expression expression) : base(expression) {}

        public IDbAsyncEnumerator<T> GetAsyncEnumerator()
        {
            return new FakeDbAsyncEnumerator<T>(this.AsEnumerable()
                                                    .GetEnumerator());
        }

        IDbAsyncEnumerator IDbAsyncEnumerable.GetAsyncEnumerator()
        {
            return GetAsyncEnumerator();
        }

        IQueryProvider IQueryable.Provider
        {
            get { return new FakeDbAsyncQueryProvider<T>(this); }
        }
    }
}