using System;
using System.Threading.Tasks;
using GridDomain.Aggregates;

namespace GridDomain.Domains
{
    /// <summary>
    /// Interface used to build all participaties parties
    /// </summary>
    public interface IDomainBuilder
    {
        Task RegisterAggregate<TAggregate>(IAggregateConfiguration<TAggregate> configuration) where TAggregate : class,IAggregate;
        // void RegisterHandler<TContext,TMessage, THandler>(IMessageHandlerFactory<TContext,TMessage, THandler> factory) where THandler : IHandler<TMessage>
        //                                                                                              where TMessage : class, IHaveProcessId, IHaveId;
        //  Task Register(ActorSystem system, ContainerBuilder container);

        Task<IDomain> Build();

        void RegisterCommandHandler<T>(Func<ICommandHandler<ICommand>, T> proxyBuilder);
        void RegisterCommandsResultAdapter<TAggregate>(ICommandsResultAdapter adapter) where TAggregate:IAggregate;
    }
}


    