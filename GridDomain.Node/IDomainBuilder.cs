using GridDomain.Aggregates;

namespace GridDomain.Node
{
    public interface IDomainBuilder
    {
       // void RegisterProcessManager<TState>(IProcessDependencyFactory<TState> processDependenciesfactory)where TState : class, IProcessState;
        void RegisterAggregate<TAggregate>(IAggregateDependencies<TAggregate> factory) where TAggregate : IAggregate;
       // void RegisterHandler<TContext,TMessage, THandler>(IMessageHandlerFactory<TContext,TMessage, THandler> factory) where THandler : IHandler<TMessage>
       //                                                                                              where TMessage : class, IHaveProcessId, IHaveId;
    }
}