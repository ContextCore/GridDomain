namespace GridDomain.CQRS.Messaging.MessageRouting
{
    public interface IAggregateCommandsHandler<TAggregate>
    {
        TAggregate Execute(TAggregate aggregate, ICommand command);
    }
}