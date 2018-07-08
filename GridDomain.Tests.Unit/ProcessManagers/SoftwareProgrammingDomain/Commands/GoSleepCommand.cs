using System;
using GridDomain.CQRS;
using GridDomain.ProcessManagers;

namespace GridDomain.Tests.Unit.ProcessManagers.SoftwareProgrammingDomain.Commands
{
    public class GoSleepCommand : Command<ProgrammerAggregate>
    {
        public GoSleepCommand(string personId, string sofaId) : base(personId)
        {
            SofaId = sofaId;
        }

        public string SofaId { get; }
        public string PersonId => AggregateId;
    }
}