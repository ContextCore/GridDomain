using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;

using GridDomain.Common;
using GridDomain.CQRS;
using GridDomain.EventSourcing.CommonDomain;

namespace GridDomain.EventSourcing
{
    public class AggregateCommandsHandler<TAggregate> : TypeCatalog<CommandExecutionDelegate<TAggregate>, ICommand>,
                                                        IAggregateCommandsHandler<TAggregate> where TAggregate : Aggregate
                                                      
    {
        public IReadOnlyCollection<Type> RegisteredCommands => Catalog.Keys.ToArray();
        public Type AggregateType { get; } = typeof(TAggregate);

        public Task<TAggregate> ExecuteAsync(TAggregate aggregate, ICommand command, IEventStore eventStore)
        {
            return Get(command).Invoke(aggregate, command, eventStore);
        }

        public override CommandExecutionDelegate<TAggregate> Get(ICommand command)
        {
            var handler = base.Get(command);
            if (handler == null)
                throw new CannotFindAggregateCommandHandlerExeption(typeof(TAggregate), command.GetType());
            return handler;
        }

        /// <summary>
        /// Maps aggregate method returning a Task
        /// </summary>
        /// <typeparam name="TCommand"></typeparam>
        /// <param name="commandExecutor"></param>
        protected internal void Map<TCommand>(Func<TCommand, TAggregate, Task> commandExecutor) where TCommand : ICommand
        {
            Add<TCommand>(async (a,c,p) => {
                                a.InitEventStore(p);
                                await commandExecutor((TCommand)c, a);
                               //for cases when we call Produce and expect events persistence after aggregate methods invocation;
                                await p.Persist(a);
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
            Add<TCommand>(async (a, c, p) =>
                            {
                                a.InitEventStore(p);
                                commandExecutor((TCommand)c, a);
                                //for cases when we call Produce and expect events persistence after aggregate methods invocation;
                                await p.Persist(a);
                                return a;
                            });
        }

        /// <summary>
        /// Maps aggregate constructor
        /// </summary>
        /// <typeparam name="TCommand"></typeparam>
        /// <param name="aggregateCreator"></param>
        protected internal void Map<TCommand>(Func<TCommand, TAggregate> aggregateCreator) where TCommand : ICommand
        {
            Add<TCommand>(async (a, c, p) =>
                                          {
                                              var aggregate = aggregateCreator((TCommand)c);
                                              aggregate.InitEventStore(p);
                                              await p.Persist(aggregate);
                                              return aggregate;
                                          });
        }
    }
}