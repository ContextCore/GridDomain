using System;
using System.Data.Entity;

namespace GridDomain.CQRS.ReadModel
{
    public class SqlReadModelCreator<T> : IReadModelCreator<T> where T : class
    {
        private readonly Func<DbContext> _contextFactory;

        public SqlReadModelCreator(Func<DbContext> contextCreator)
        {
            _contextFactory = contextCreator;
        }

        public void Add(T entity)
        {
            using (var context = _contextFactory())
            {
                context.Set<T>().Add(entity);
                context.SaveChanges();
            }
        }

        public void Modify(Guid id, Action<T> modificator)
        {
            using (var context = _contextFactory())
            {
                var entity = context.Set<T>().Find(id);
                if (entity == null)
                    throw new CannotFindRecordException();
                modificator(entity);
                context.SaveChanges();
            }
        }
    }
}