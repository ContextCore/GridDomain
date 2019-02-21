using System.Collections.Generic;

namespace GridDomain.Aggregates
{
    public interface IExecuteCommand<TCommand>:ICommandHandler<TCommand,IReadOnlyCollection<DomainEvent>>
    {
    }
}