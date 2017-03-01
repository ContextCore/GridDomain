using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CommonDomain;
using GridDomain.Common;
using GridDomain.EventSourcing;

namespace GridDomain.CQRS.Messaging.MessageRouting
{
    public class AggregateCommandsHandler<TAggregate> : TypeCatalog<Func<ICommand, TAggregate, Task<TAggregate>>, ICommand>,
                                                        IAggregateCommandsHandler<TAggregate> where TAggregate : Aggregate
    {
        public IReadOnlyCollection<AggregateCommandInfo> RegisteredCommands
        {
            get { return Catalog.Select(h => new AggregateCommandInfo(h.Key)).ToArray(); }
        }

        public Task<TAggregate> ExecuteAsync(TAggregate aggregate, ICommand command)
        {
            return Get(command).Invoke(command, aggregate);
        }

        public override Func<ICommand, TAggregate, Task<TAggregate>> Get(ICommand command)
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
                              return Task.FromResult(a);
                          });
        }

        protected void Map<TCommand>(Func<TCommand, TAggregate> commandExecutor) where TCommand : ICommand
        {
            Add<TCommand>((c, a) =>
                          {
                              var aggregate = commandExecutor((TCommand) c);

                              var eventsToSave = ((IAggregate) aggregate).GetUncommittedEvents()
                                                                         .Cast<DomainEvent>()
                                                                         .ToArray();

                              aggregate.PersistDelegate(eventsToSave,e => {});

                              return Task.FromResult(aggregate);
                          });
        }

        public void Map<TCommand>(Func<TCommand, TAggregate, Task> commandExecutor) where TCommand : ICommand
        {
            Add<TCommand>((c, a) => commandExecutor((TCommand) c, a).ContinueWith(t => a));
        }
    }
}