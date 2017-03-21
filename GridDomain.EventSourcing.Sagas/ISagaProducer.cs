using System;
using System.Collections.Generic;
using GridDomain.EventSourcing.Sagas.InstanceSagas;

namespace GridDomain.EventSourcing.Sagas
{
    public interface ISagaProducer<TState> where TState : ISagaState
    {
        ISagaDescriptor Descriptor { get; }

        //TODO: extract to separate type? 
        IReadOnlyCollection<Type> KnownDataTypes { get; }
        ISaga<TState> Create(object message);
    }
}