using GridDomain.CQRS;
using GridDomain.EventSourcing;

namespace GridDomain.Tests.XUnit.CommandPipe
{
    class TestCommand : Command
    {
        public TestCommand(DomainEvent fromEvent)
        {
            FromEvent = fromEvent;
        }

        public DomainEvent FromEvent { get; }
    }
}