using System;
using CommonDomain.Core;
using CommonDomain.Persistence;

namespace GridDomain.EventSourcing
{
    public class NEventStoreRepositoryAdapter : IAggregateRepository
    {
        private readonly IRepository _repo;

        public NEventStoreRepositoryAdapter(IRepository repo)
        {
            _repo = repo;
        }

        public T Load<T>(Guid id) where T : AggregateBase
        {
            return _repo.GetById<T>(id);
        }

        public void Save(AggregateBase aggregate, Guid commitId)
        {
            _repo.Save(aggregate, commitId);
        }
    }
}