using System;
using CommonDomain.Core;

namespace GridDomain.EventSourcing
{
    public interface IAggregateRepository
    {
        T Load<T>(Guid id) where T : AggregateBase;
        void Save(AggregateBase aggregate, Guid commitId);
    }
}