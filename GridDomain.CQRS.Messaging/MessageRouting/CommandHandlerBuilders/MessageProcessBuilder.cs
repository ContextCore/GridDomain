using CommonDomain;
using CommonDomain.Persistence;

namespace GridDomain.CQRS.Messaging.MessageRouting.CommandHandlerBuilders
{
    public class MessageProcessBuilder<TCommand> where TCommand : ICommand
    {
        private readonly IPublisher _publisher;
        private readonly IRepository _repository;

        public MessageProcessBuilder(IRepository repository, IPublisher publisher)
        {
            _publisher = publisher;
            _repository = repository;
        }

        public AggregateCreationBuilder<TCommand, TAggregate> ForAggregate<TAggregate>()
            where TAggregate : class, IAggregate
        {
            return new AggregateCreationBuilder<TCommand, TAggregate>(_repository, _publisher);
        }
    }
}