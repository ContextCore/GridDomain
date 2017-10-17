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
        void RegisterAggregate<TAggregate>(IAggregateDependencyFactory<TAggregate> factory) where TAggregate : Aggregate;
        void RegisterHandler<TMessage, THandler>(IMessageHandlerFactory<TMessage, THandler> factory) where THandler : IHandler<TMessage>
                                                                                                     where TMessage : class, IHaveProcessId, IHaveId;
    }
}