using System;
using GridDomain.CQRS;

namespace GridDomain.Tests.XUnit.Sagas.SoftwareProgrammingDomain.Commands
{
    public class GoSleepCommand: Command
    {
        public Guid SofaId { get; }
        public Guid PersonId => AggregateId;
        public GoSleepCommand(Guid personId, Guid sofaId):base(personId)
        {
            SofaId = sofaId;
        }
    }
}