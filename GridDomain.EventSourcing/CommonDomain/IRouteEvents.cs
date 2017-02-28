using System;

namespace GridDomain.EventSourcing.CommonDomain
{
    public interface IRouteEvents {
        void Register<T>(Action<T> handler);
        void Register(IAggregate aggregate);

        void Dispatch(object eventMessage);
    }
}