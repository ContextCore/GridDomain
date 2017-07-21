using System.Collections.Generic;
using GridDomain.CQRS;

namespace GridDomain.Processes
{
    public class ProcessResult<TState>
    {
        public TState State { get; }
        public IReadOnlyCollection<Command> ProducedCommands { get; }

        public ProcessResult(TState state, IReadOnlyCollection<Command> producedCommands)
        {
            State = state;
            ProducedCommands = producedCommands;
        }
    }
}