using System;
using System.Collections.Generic;

namespace GridDomain.EventSourcing.Sagas
{
    public interface ISagaCreatorCatalog<TState> : ISagaCreator<TState>,
                                                   ISagaCreator<TState, object>
        where TState : ISagaState
    {
        bool CanCreateFrom(object message);
    }
}