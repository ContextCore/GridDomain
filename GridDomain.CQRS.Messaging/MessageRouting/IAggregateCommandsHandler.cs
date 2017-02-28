using System.Threading.Tasks;

namespace GridDomain.CQRS.Messaging.MessageRouting
{
    public interface IAggregateCommandsHandler<TAggregate>
    {
      //  TAggregate Execute(TAggregate aggregate, ICommand command);
        Task<TAggregate> ExecuteAsync(TAggregate aggregate, ICommand command);
    }
}