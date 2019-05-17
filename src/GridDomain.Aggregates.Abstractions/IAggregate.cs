using System.Collections.Generic;

namespace GridDomain.Aggregates.Abstractions
{
    public interface IAggregate : IEventSourced, ICommandHandler<ICommand, IReadOnlyCollection<IDomainEvent>>
    {
    }
}