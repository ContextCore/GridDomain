using System;
using System.Threading.Tasks;
using GridDomain.Aggregates;
using GridDomain.Aggregates.Abstractions;
using GridDomain.EventHandlers;

namespace GridDomain.Domains
{
    /// <summary>
    /// Interface used to build all an domain object parties
    /// </summary>
    public interface IDomainBuilder
    {
        Task RegisterAggregate<TAggregate>(IAggregateConfiguration<TAggregate> configuration) where TAggregate : class,IAggregate;

        //void RegisterEventHandler<TEvent, THandler>() where THandler : IEventHandler<TEvent>;

        Task<IDomain> Build();

        void RegisterCommandHandler<T>(Func<ICommandHandler<ICommand>, T> proxyBuilder);
        void RegisterCommandsResultAdapter<TAggregate>(ICommandsResultAdapter adapter) where TAggregate:IAggregate;
    }
}


    