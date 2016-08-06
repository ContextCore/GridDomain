using System;
using CommonDomain;
using CommonDomain.Core;

namespace GridDomain.Tools
{
    public interface IRepository : IDisposable
    {
        void Save<T>(T aggr) where T:IAggregate;
        T Load<T>(Guid id) where T : AggregateBase;

    }
}