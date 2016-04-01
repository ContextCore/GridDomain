using System;
using System.Collections.Generic;
using CommonDomain;

namespace GridDomain.CQRS.Messaging.MessageRouting.CommandHandlerBuilders
{
    public class FailureBuilder<TCommand,TAggregate> where TCommand: ICommand
        where TAggregate:IAggregate
    {
        private readonly Func<TCommand, TAggregate> _act;
        private readonly Dictionary<Type, Func<object, Exception, object>> _knownExceptions = new Dictionary<Type, Func<object, Exception, object>>();
        private readonly IPublisher _publisher;

        public FailureBuilder(Func<TCommand,TAggregate> act,
            IPublisher publisher)
        {
            _publisher = publisher;
            _act = act;
        }

        public FailureBuilder<TCommand,TAggregate> KnownFailure<TException>()
        {
            _knownExceptions.Add(typeof(TException),
                (msg, ex) =>
                    new CommandFailure<TCommand,TException>((TCommand)msg, (TException)(object)ex));

            return this;
        }

        public IHandler<TCommand> Create()
        {
            return new CommandHandler<TAggregate, TCommand>(_act,
                                                            _knownExceptions,
                                                            _publisher);    
        }
    }
}