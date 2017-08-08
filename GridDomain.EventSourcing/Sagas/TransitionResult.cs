using System.Collections.Generic;
using GridDomain.CQRS;

namespace GridDomain.EventSourcing.Sagas
{
    public class TransitionResult<TState>
    {
        public TState State { get; }
        public IReadOnlyCollection<Command> ProducedCommands { get; }

        public TransitionResult(TState state, IReadOnlyCollection<Command> producedCommands)
        {
            State = state;
            ProducedCommands = producedCommands;
        }
    }
}