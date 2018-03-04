using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GridDomain.CQRS;
using GridDomain.EventSourcing.CommonDomain;



namespace GridDomain.EventSourcing {

    //marker interface to simplify navigation from command to its aggregate

    public abstract class CommandAggregate : Aggregate, IAggregateCommandsHandler<CommandAggregate>
    {
        protected CommandAggregate(string id) : base(id)
        {
            AggregateType = GetType();
        }
        public Type AggregateType { get; }
        public abstract IReadOnlyCollection<Type> RegisteredCommands { get; }

        protected abstract Task<IAggregate> Execute(ICommand cmd);

        public async Task<CommandAggregate> ExecuteAsync(CommandAggregate aggregate, ICommand command)
        {
            return (CommandAggregate) await Execute(command);
        }
    }
}