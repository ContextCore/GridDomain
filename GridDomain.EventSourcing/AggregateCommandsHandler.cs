using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using GridDomain.Common;
using GridDomain.CQRS;

namespace GridDomain.EventSourcing
{

    public delegate Task<TAggregate> CommandExecutionDelegate<TAggregate>(TAggregate agr, ICommand cmd, PersistenceDelegate persistenceDelegate) where TAggregate : Aggregate;

    public class AggregateCommandsHandler<TAggregate> : TypeCatalog<CommandExecutionDelegate<TAggregate>, ICommand>,
                                                        IAggregateCommandsHandler<TAggregate> where TAggregate : Aggregate
                                                      
    {
        public IReadOnlyCollection<Type> RegisteredCommands => Catalog.Keys.ToArray();
        public Type AggregateType { get; } = typeof(TAggregate);

        public Task ExecuteAsync(TAggregate aggregate, ICommand command, PersistenceDelegate persistenceDelegate)
        {
            return Get(command).Invoke(aggregate, command, persistenceDelegate);
        }

        public override CommandExecutionDelegate<TAggregate> Get(ICommand command)
        {
            var handler = base.Get(command);
            if (handler == null)
                throw new CannotFindAggregateCommandHandlerExeption(typeof(TAggregate), command.GetType());
            return handler;
        }

        protected void Map<TCommand>(Func<TCommand, TAggregate, Task> commandExecutor) where TCommand : ICommand
        {
            Add<TCommand>(async (a, c, p) =>
                          {
                              await commandExecutor((TCommand)c, a);
                              return a;
                          });
        }

        private void Map<TCommand>(Func<TCommand, TAggregate, TAggregate> commandExecutor) where TCommand : ICommand
        {
            Add<TCommand>((a, c, p) =>
            {
                try
                {
                    return Task.FromResult(commandExecutor((TCommand)c, a));
                }
                catch (Exception ex)
                {
                    return Task.FromException<TAggregate>(ex);
                }
            });
        }

        public void Map<TCommand>(Action<TCommand, TAggregate> commandExecutor) where TCommand : ICommand
        {
            Map((Func<TCommand, TAggregate, TAggregate>)((c, a) =>
                                                         {
                                                             commandExecutor(c, a);
                                                             return a;
                                                         }));
        }

        /// <summary>
        /// Special case for constructors - we should invoke persist externally, due to events 
        /// are raised in constructor when aggregate persist delegate is not set yet
        /// </summary>
        /// <typeparam name="TCommand"></typeparam>
        /// <param name="commandExecutor"></param>
        protected void Map<TCommand>(Func<TCommand, TAggregate> commandExecutor) where TCommand : ICommand
        {
            Add<TCommand>(async (a, c, p) =>
                          {
                              var agr = commandExecutor((TCommand)c);
                              agr.Persist = p;
                              await p(agr);
                              return agr;
                          });
        }
    }
}