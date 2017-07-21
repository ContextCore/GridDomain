
using GridDomain.CQRS;
using GridDomain.EventSourcing.CommonDomain;

namespace GridDomain.EventSourcing.Sagas
{
    public interface ISagaFault
    {
        IFault CommandFault { get; }
        object State { get; }
    }

    public interface ISagaFault<out TState> : ISagaFault where TState : IAggregate
    {
        new TState State { get; }
    }
}