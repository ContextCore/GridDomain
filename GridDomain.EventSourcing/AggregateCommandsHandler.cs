using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CommonDomain;
using GridDomain.Common;
using GridDomain.CQRS;

namespace GridDomain.EventSourcing
{
    
    public class AggregateCommandsHandler<TAggregate> : TypeCatalog<Func<ICommand, TAggregate, Task<TAggregate>>, ICommand>,
                                                        IAggregateCommandsHandler<TAggregate> where TAggregate : Aggregate
    {
        public IReadOnlyCollection<Type> RegisteredCommands => Catalog.Keys.ToArray();

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
                              try
                              {
                                  commandExecutor((TCommand) c, a);
                                  return Task.FromResult(a);
                              }
                              catch (Exception ex)
                              {
                                  return Task.FromException<TAggregate>(ex);
                              }
                          });
        }

        protected void Map<TCommand>(Func<TCommand, TAggregate> commandExecutor) where TCommand : ICommand
        {
            Add<TCommand>((c, a) =>
                          {
                              try
                              {
                                  var aggregate = commandExecutor((TCommand) c);

                                  var eventsToSave = ((IAggregate) aggregate).GetUncommittedEvents()
                                                                             .Cast<DomainEvent>()
                                                                             .ToArray();

                                  aggregate.PersistEvents(Task.FromResult(eventsToSave), e => { }, () => { }, aggregate);

                                  return Task.FromResult(aggregate);
                              }
                              catch (Exception ex)
                              {
                                  return Task.FromException<TAggregate>(ex);
                              }
                          });
        }
    }
}