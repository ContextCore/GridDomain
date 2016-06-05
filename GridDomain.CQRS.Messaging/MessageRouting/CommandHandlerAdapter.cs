namespace GridDomain.CQRS.Messaging.MessageRouting
{
    public class CommandHandlerAdapter<TMessage, TCommandHandler> : IHandler<TMessage>
        where TCommandHandler : ICommandHandler<TMessage>
    {
        private readonly TCommandHandler _cmdHandler;

        public CommandHandlerAdapter(TCommandHandler cmdHandler)
        {
            _cmdHandler = cmdHandler;
        }

        public void Handle(TMessage e)
        {
            _cmdHandler.Handle(e);
        }
    }
}