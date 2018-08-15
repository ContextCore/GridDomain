using GridDomain.Common;
using GridDomain.CQRS;
using GridDomain.EventSourcing;
using GridDomain.EventSourcing.CommonDomain;
using GridDomain.ProcessManagers;

namespace GridDomain.Configuration
{
    public interface IDomainBuilder
    {
        void RegisterProcessManager<TState>(IProcessDependencyFactory<TState> processDependenciesfactory)where TState : class, IProcessState;
        void RegisterAggregate<TAggregate>(IAggregateDependencies<TAggregate> factory) where TAggregate : Aggregate;
        void RegisterHandler<TContext,TMessage, THandler>(IMessageHandlerFactory<TContext,TMessage, THandler> factory) where THandler : IHandler<TMessage>
                                                                                                     where TMessage : class, IHaveProcessId, IHaveId;
    }
}