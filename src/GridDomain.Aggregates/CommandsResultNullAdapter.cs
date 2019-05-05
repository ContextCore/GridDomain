using System.Collections.Generic;

namespace GridDomain.Aggregates
{
    public class CommandsResultNullAdapter : ICommandsResultAdapter
    {
        public object Adapt(object command, IReadOnlyCollection<IDomainEvent> result)
        {
            return null;
        }
        
        public static CommandsResultNullAdapter Instance { get; } = new CommandsResultNullAdapter();
    }
}