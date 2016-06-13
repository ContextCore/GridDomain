using GridDomain.CQRS;

namespace GridDomain.EventSourcing.Sagas
{
    public interface ISagaFault
    {
        ICommandFault CommandFault { get; }
        object State { get; }
    }
}