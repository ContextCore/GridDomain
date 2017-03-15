using System.Threading.Tasks;
using GridDomain.CQRS;

namespace GridDomain.EventSourcing
{
    public interface IAggregateCommandsHandler<TAggregate>
    {
        Task<TAggregate> ExecuteAsync(TAggregate aggregate, ICommand command);
    }
}