using System.Collections.Generic;

namespace GridDomain.Aggregates
{
    public interface IAggregate : IEventSourced, ICommandHandler<ICommand, IReadOnlyCollection<IDomainEvent>>
    {
    }
}