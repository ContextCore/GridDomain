using GridDomain.CQRS;
using GridDomain.EventSourcing;
using GridDomain.Processes;

namespace GridDomain.Configuration
{
    public interface IDomainBuilder
    {
        void RegisterProcessManager<TState>(IProcessManagerDependencyFactory<TState> factory)where TState : class, IProcessState;
        void RegisterAggregate<TAggregate>(IAggregateDependencyFactory<TAggregate> factory) where TAggregate : Aggregate;
       
        void RegisterHandler<TMessage, THandler>(IMessageHandlerFactory<TMessage, THandler> factory) where THandler : IHandler<TMessage>;
    }
}