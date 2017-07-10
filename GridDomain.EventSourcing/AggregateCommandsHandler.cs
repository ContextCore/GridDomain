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

     
        private void AddCommandExecution<TCommand>(Func<TCommand, TAggregate, Task<TAggregate>> commandExecutor) where TCommand : ICommand
        {
            Add<TCommand>((a, c, p) =>
            {
                Task<TAggregate> commandExecutionTask;
                try
                {
                    commandExecutionTask = commandExecutor((TCommand)c, a);
                }
                catch (Exception ex)
                {
                    return Task.FromException<TAggregate>(ex);
                }

                return commandExecutionTask.ContinueWith(t =>
                                                  {
                                                      var aggregate = t.Result;
                                                      aggregate.Persist = p;
                                                      if (!aggregate.HasUncommitedEvents) return aggregate;
                                                      //for cases when we call Produce and expect events persistence after aggregate methods invocation;
                                                      return p(aggregate).ContinueWith(tr => aggregate).Result;
                                                  });
            });
        }
        /// <summary>
        /// Maps aggregate method returning a Task
        /// </summary>
        /// <typeparam name="TCommand"></typeparam>
        /// <param name="commandExecutor"></param>
        protected void Map<TCommand>(Func<TCommand, TAggregate, Task> commandExecutor) where TCommand : ICommand
        {
            AddCommandExecution<TCommand>((c, a) => commandExecutor(c, a).ContinueWith(t => a));
        }

        /// <summary>
        /// Maps aggregate void method
        /// </summary>
        /// <typeparam name="TCommand"></typeparam>
        /// <param name="commandExecutor"></param>
        public void Map<TCommand>(Action<TCommand, TAggregate> commandExecutor) where TCommand : ICommand
        {
            AddCommandExecution<TCommand>((c, a) =>
                                          {
                                              commandExecutor(c, a);
                                              return Task.FromResult(a);
                                          });
        }

        /// <summary>
        /// Maps aggregate constructor
        /// </summary>
        /// <typeparam name="TCommand"></typeparam>
        /// <param name="commandExecutor"></param>
        protected void Map<TCommand>(Func<TCommand, TAggregate> commandExecutor) where TCommand : ICommand
        {
            AddCommandExecution<TCommand>((c, a) => Task.FromResult(commandExecutor(c)));
        }
    }
}