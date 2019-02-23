using System.Linq;
using System.Threading.Tasks;
using GridDomain.Aggregates;

namespace GridDomain.Node
{
    /// <summary>
    /// Interface used to build all participaties parties
    /// </summary>
    public interface IDomainBuilder
    {
        // void RegisterProcessManager<TState>(IProcessDependencyFactory<TState> processDependenciesfactory)where TState : class, IProcessState;
        Task RegisterAggregate<TAggregate>(IAggregateDependencies<TAggregate> factory) where TAggregate : class,IAggregate;
        // void RegisterHandler<TContext,TMessage, THandler>(IMessageHandlerFactory<TContext,TMessage, THandler> factory) where THandler : IHandler<TMessage>
        //                                                                                              where TMessage : class, IHaveProcessId, IHaveId;
        //  Task Register(ActorSystem system, ContainerBuilder container);

        Task<IDomain> Build();

        //  ActorSystem System { get; }
        //  ContainerBuilder Container { get; }
    }
}


    