using System;
using GridDomain.CQRS;

namespace GridDomain.Tests.Unit.Sagas.SoftwareProgrammingDomain.Commands
{
    public class GoSleepCommand : Command
    {
        public GoSleepCommand(Guid personId, Guid sofaId) : base(personId)
        {
            SofaId = sofaId;
        }

        public Guid SofaId { get; }
        public Guid PersonId => AggregateId;
    }
}