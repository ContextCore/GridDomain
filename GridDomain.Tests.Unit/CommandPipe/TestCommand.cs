using System;
using GridDomain.CQRS;

namespace GridDomain.Tests.Unit.CommandPipe
{
    internal class TestCommand : Command
    {
        public TestCommand(string aggregateId):base(aggregateId)
        {

        }
    }
}