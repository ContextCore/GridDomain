using CommonDomain;
using GridDomain.CQRS;

namespace GridDomain.EventSourcing.Sagas
{
    public interface ISagaFault
    {
        ICommandFault CommandFault { get; }
        object State { get; }
    }
    public interface ISagaFault<out TState> : ISagaFault
                                   where TState : IAggregate
    {
        new TState State { get; }
    }
}