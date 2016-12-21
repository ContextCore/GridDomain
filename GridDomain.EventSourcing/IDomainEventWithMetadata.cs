namespace GridDomain.EventSourcing
{
    public interface IDomainEventWithMetadata : IMessageWithMetadata<ISourcedEvent>
    {
        
    }
}