using System;
using GridDomain.CQRS;
using GridDomain.EventSourcing;

namespace GridDomain.Tests.XUnit.CommandPipe
{
    internal class TestCommand : Command
    {
        public TestCommand(Guid aggregateId):base(aggregateId)
        {

        }
    }
}