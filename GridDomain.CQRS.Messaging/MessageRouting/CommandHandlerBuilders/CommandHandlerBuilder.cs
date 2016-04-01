using CommonDomain.Persistence;

namespace GridDomain.CQRS.Messaging.MessageRouting.CommandHandlerBuilders
{
    public class CommandHandlerBuilder
    {
        private readonly IPublisher _publisher;
        private readonly IRepository _repository;

        public CommandHandlerBuilder(IRepository repository,
            IPublisher publisher)
        {
            _publisher = publisher;
            _repository = repository;
        }

        public MessageProcessBuilder<TCommand> WhenRecieved<TCommand>() where TCommand : ICommand
        {
            return new MessageProcessBuilder<TCommand>(_repository, _publisher);
        }
    }
}