using System.Threading.Tasks;
using GridDomain.CQRS;

namespace GridDomain.EventSourcing
{
    public interface IAggregateCommandsHandler<TAggregate> : IAggregateCommandsHandlerDescriptor
    {
        Task<TAggregate> ExecuteAsync(TAggregate aggregate, ICommand command);
    }
}