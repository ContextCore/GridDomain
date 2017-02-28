using System;
using System.Collections.Generic;
using System.Linq;
using CommonDomain.Core;
using GridDomain.Common;

namespace GridDomain.CQRS.Messaging.MessageRouting
{
    public class AggregateCommandsHandler<TAggregate> : TypeCatalog<Func<ICommand, TAggregate, TAggregate>, ICommand>,
                                                        IAggregateCommandsHandler<TAggregate>
        where TAggregate : AggregateBase
    {
        public IReadOnlyCollection<AggregateCommandInfo> RegisteredCommands
        {
            get { return Catalog.Select(h => new AggregateCommandInfo(h.Key)).ToArray(); }
        }

        public TAggregate Execute(TAggregate aggregate, ICommand command)
        {
            return Get(command).Invoke(command, aggregate);
        }

        public override Func<ICommand, TAggregate, TAggregate> Get(ICommand command)
        {
            var handler = base.Get(command);
            if (handler == null)
                throw new CannotFindAggregateCommandHandlerExeption(typeof(TAggregate), command.GetType());
            return handler;
        }

        public void Map<TCommand>(Action<TCommand, TAggregate> commandExecutor) where TCommand : ICommand
        {
            Add<TCommand>((c, a) =>
                          {
                              commandExecutor((TCommand) c, a);
                              return a;
                          });
        }

        protected void Map<TCommand>(Func<TCommand, TAggregate> commandExecutor) where TCommand : ICommand
        {
            Add<TCommand>((c, a) => commandExecutor((TCommand) c));
        }
    }
}