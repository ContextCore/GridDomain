using System;
using System.Collections.Generic;
using GridDomain.EventSourcing.Sagas.InstanceSagas;

namespace GridDomain.EventSourcing.Sagas
{
    public interface ISaga—reatorCatalog<TState> : ISagaCreator<TState>,
                                                   ISagaCreator<TState, object>
        where TState : ISagaState
    {
        bool CanCreateFrom(object message);
    }
}