using GridDomain.EventSourcing.Sagas.InstanceSagas;

namespace GridDomain.EventSourcing.Sagas
{
// public interface ISagaFactory<out TSaga, in TState> : IFactory<TSaga, TState> where TSaga : ISaga<TState>
//                                                                               where TState : ISagaState {}
//
// public interface ISagaFactory<TState> : IFactory<ISaga<TState>, TState> where TState : ISagaState {}
//
    public interface ISagaFactory<out TSaga, in TMessage>
    {
        TSaga Create(TMessage message);
    }
}