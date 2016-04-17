using System;
using System.Collections.Generic;
using GridDomain.Logging;
using NLog;

namespace GridDomain.CQRS.Messaging.MessageRouting.CommandHandlerBuilders
{
    public class CommandHandler<TAggregate, TCommand> : ICommandHandler<TCommand>, IHandler<TCommand>
        where TCommand : ICommand
    {
        private readonly Func<TCommand, TAggregate> _act;
        private readonly Dictionary<Type, Func<object, Exception, object>> _knownFailuresBuilders;

        private readonly Logger _log = LogManager.GetCurrentClassLogger();
        private readonly IPublisher _publisher;

        public CommandHandler(Func<TCommand, TAggregate> act,
            Dictionary<Type, Func<object, Exception, object>> knownFailuresBuilders,
            IPublisher publisher)
        {
            _publisher = publisher;
            _knownFailuresBuilders = knownFailuresBuilders;
            _act = act;
        }

        public void Handle(TCommand e)
        {
            try
            {
                _act(e);
            }
            catch (Exception ex)
            {
                var failure = new CommandFault<TCommand>(e, ex);

                _log.Info($"При исполнении команды {typeof (TCommand).Name} возникла ошибка " +
                          $"{ex.GetType().Name}, которая будет передана отправителю команды. \r\n Информация:\r\n" +
                          failure.ToPropsString());

                _publisher.Publish(failure);
                //TODO: выбрасывать ошибку выше или нет? 
            }
        }
    }
}