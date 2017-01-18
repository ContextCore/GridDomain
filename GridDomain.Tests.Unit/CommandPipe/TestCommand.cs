using GridDomain.CQRS;
using GridDomain.EventSourcing;

namespace GridDomain.Tests.Unit.CommandPipe
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