using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;

using GridDomain.Common;
using GridDomain.CQRS;

namespace GridDomain.EventSourcing
{
    public class AggregateCommandsHandler<TAggregate> : TypeCatalog<CommandExecutionDelegate<TAggregate>, ICommand>,
                                                        IAggregateCommandsHandler<TAggregate> where TAggregate : Aggregate
                                                      
    {
        public IReadOnlyCollection<Type> RegisteredCommands => Catalog.Keys.ToArray();
        public Type AggregateType { get; } = typeof(TAggregate);

        public async Task ExecuteAsync(TAggregate aggregate, ICommand command, PersistenceDelegate persistenceDelegate)
        {
            try
            {
                await Get(command).Invoke(aggregate, command, persistenceDelegate);
            }
            catch (Exception ex)
            {
               throw new CommandExecutionFailedException(command, ex);
            }

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
            Add<TCommand>(async (a, c, p) =>
            {
                TAggregate aggregate = await commandExecutor((TCommand)c, a);
                if(!a.Equals(aggregate))
                    aggregate.SetPersistProvider(p);
                if (!aggregate.HasUncommitedEvents) return aggregate;
                //for cases when we call Produce and expect events persistence after aggregate methods invocation;
                await p(aggregate);
                return aggregate;
            });
        }
        /// <summary>
        /// Maps aggregate method returning a Task
        /// </summary>
        /// <typeparam name="TCommand"></typeparam>
        /// <param name="commandExecutor"></param>
        protected internal void Map<TCommand>(Func<TCommand, TAggregate, Task> commandExecutor) where TCommand : ICommand
        {
            AddCommandExecution<TCommand>(async (c, a) =>
                                          {
                                              await commandExecutor(c, a);
                                              return a;
                                          });
        }

        /// <summary>
        /// Maps aggregate void method
        /// </summary>
        /// <typeparam name="TCommand"></typeparam>
        /// <param name="commandExecutor"></param>
        protected internal void Map<TCommand>(Action<TCommand, TAggregate> commandExecutor) where TCommand : ICommand
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
        protected internal void Map<TCommand>(Func<TCommand, TAggregate> commandExecutor) where TCommand : ICommand
        {
            AddCommandExecution<TCommand>((c, a) => Task.FromResult(commandExecutor(c)));
        }
    }
}