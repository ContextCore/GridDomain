using Automatonymous;

namespace GridDomain.Tests.Sagas.Simplified
{
    public interface ISagaState<TState>
    {
        TState CurrentState { get; set; }
    }
}