using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using GridDomain.Common;
using GridDomain.CQRS;

namespace GridDomain.EventSourcing
{
    
    public class AggregateCommandsHandler<TAggregate> : TypeCatalog<Func<ICommand, TAggregate, Task<TAggregate>>, ICommand>,
                                                        IAggregateCommandsHandlerDescriptor,
                                                        IAggregateCommandsHandler<TAggregate> where TAggregate : Aggregate
                                                      
    {
        public IReadOnlyCollection<Type> RegisteredCommands => Catalog.Keys.ToArray();
        public Type AggregateType { get; } = typeof(TAggregate);

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

        private void Map<TCommand>(Func<TCommand,TAggregate, TAggregate> commandExecutor) where TCommand : ICommand
        {
            Add<TCommand>((c, a) =>
            {
                try
                {
                    var aggregate = commandExecutor((TCommand)c, a);
                    return Task.FromResult(aggregate);
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

        protected void Map<TCommand>(Func<TCommand, TAggregate> commandExecutor) where TCommand : ICommand
        {
            Map((Func<TCommand, TAggregate, TAggregate>)((c, a) => commandExecutor(c)));
        }
    }
}