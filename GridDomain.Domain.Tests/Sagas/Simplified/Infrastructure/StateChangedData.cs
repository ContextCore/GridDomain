using Automatonymous;
using Automatonymous.Contexts;

namespace GridDomain.Tests.Sagas.Simplified
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