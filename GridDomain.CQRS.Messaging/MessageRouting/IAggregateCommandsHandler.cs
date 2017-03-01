using System.Threading.Tasks;

namespace GridDomain.CQRS.Messaging.MessageRouting
{
    public interface IAggregateCommandsHandler<TAggregate>
    {
        Task<TAggregate> ExecuteAsync(TAggregate aggregate, ICommand command);
    }
}