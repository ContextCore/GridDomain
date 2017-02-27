using System;
using GridDomain.CQRS;

namespace GridDomain.Tests.XUnit.SampleDomain.Commands
{
    public class AlwaysFaultCommand : Command
    {
        public AlwaysFaultCommand(Guid aggregateId):base(aggregateId)
        {
        }
    }
}