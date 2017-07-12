using GridDomain.CQRS;
using GridDomain.EventSourcing;
using GridDomain.EventSourcing.Sagas.InstanceSagas;

namespace GridDomain.Node.Configuration.Composition
{
    public interface IDomainBuilder
    {
        void RegisterSaga<TState, TProcess>(ISagaDependencyFactory<TProcess, TState> factory) where TProcess : Process<TState>
                                                                                              where TState : class, ISagaState;
        void RegisterAggregate<TAggregate>(IAggregateDependencyFactory<TAggregate> factory) where TAggregate : Aggregate;
       
        void RegisterHandler<TMessage, THandler>(IMessageHandlerFactory<TMessage, THandler> factory) where THandler : IHandler<TMessage>;
    }
}