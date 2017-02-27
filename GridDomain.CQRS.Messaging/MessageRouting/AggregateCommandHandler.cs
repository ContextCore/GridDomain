using System;
using System.Reflection;
using CommonDomain.Core;

namespace GridDomain.CQRS.Messaging.MessageRouting
{
    public class AggregateCommandHandler<TAggregate> where TAggregate : AggregateBase
    {
        private readonly Func<ICommand, TAggregate, TAggregate> _executor;

        private AggregateCommandHandler(Func<ICommand, TAggregate, TAggregate> executor)
        {
            _executor = executor;
        }

        public static AggregateCommandHandler<TAggregate> New<TCommand>(Action<TCommand, TAggregate> commandExecutor) where TCommand : ICommand
        {
            return new AggregateCommandHandler<TAggregate>((cmd, agr) =>
                {
                    commandExecutor((TCommand)cmd, agr);
                    return agr;
                });
        }

        public static AggregateCommandHandler<TAggregate> New<TCommand>(Func<TCommand, TAggregate> commandExecutor)
        {
            return new AggregateCommandHandler<TAggregate>((cmd, agr) => commandExecutor((TCommand) cmd));
        }

        public TAggregate Execute(TAggregate agr, ICommand command)
        {
            return _executor(command, agr);
        }
    }
}