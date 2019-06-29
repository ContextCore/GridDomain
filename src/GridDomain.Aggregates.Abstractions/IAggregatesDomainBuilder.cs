using System;
using System.Threading.Tasks;
using GridDomain.Abstractions;

namespace GridDomain.Aggregates.Abstractions
{
    /// <summary>
    /// Interface used to build all an domain object parties
    /// </summary>
    public interface IAggregatesDomainBuilder:IDomainPartBuilder
    {
        Task RegisterAggregate<TAggregate>(IAggregateConfiguration<TAggregate> configuration) where TAggregate : class,IAggregate;

        void RegisterCommandHandler<T>(Func<ICommandHandler<ICommand>, T> proxyBuilder);
        void RegisterCommandsResultAdapter<TAggregate>(ICommandsResultAdapter adapter) where TAggregate:IAggregate;
    }


}