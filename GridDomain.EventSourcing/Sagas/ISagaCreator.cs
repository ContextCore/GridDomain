
using System;

namespace GridDomain.EventSourcing.Sagas
{
    public interface ISagaCreator<TState> where TState : ISagaState
    {
        ISaga<TState> Create(TState message);
    }

    public interface ISagaCreator<TState, in TStartMessage> where TState : ISagaState
    {
        ISaga<TState> CreateNew(TStartMessage message, Guid? id = null);
    }
}