using Automatonymous;

namespace GridDomain.Tests.Sagas.Simplified
{
    public interface ISagaProgress
    {
        State CurrentState { get; set; }
    }
}