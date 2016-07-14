using Automatonymous;
using Automatonymous.Contexts;

namespace GridDomain.Tests.Sagas.Simplified
{
    public class StateChangedData<TSagaState>
    {
        public StateChangedData(State state, Event @event, TSagaState instance)
        {
            State = state;
            Event = @event;
            Instance = instance;
        }
        public TSagaState Instance { get; }
        public State State { get; }
        public Event Event { get; }
    }
}