using Automatonymous;

namespace GridDomain.EventSourcing.Sagas
{
    public class StateChangedData<TSagaState>
    {
        public StateChangedData(State state, TSagaState instance)
        {
            State = state;
            Instance = instance;
        }

        public TSagaState Instance { get; }
        public State State { get; }
    }
}