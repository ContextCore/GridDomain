using CommonDomain.Persistence;

namespace GridDomain.CQRS.Messaging.MessageRouting.CommandHandlerBuilders
{
    public class CommandHandlerBuilder
    {
        private readonly IRepository _repository;
        private readonly IPublisher _publisher;

        public CommandHandlerBuilder(IRepository repository,
                                     IPublisher publisher)
        {
            _publisher = publisher;
            _repository = repository;
        }

        public MessageProcessBuilder<TCommand> WhenRecieved<TCommand>() where TCommand:ICommand
        {
            return new MessageProcessBuilder<TCommand>(_repository, _publisher);
        }

      
    }
}