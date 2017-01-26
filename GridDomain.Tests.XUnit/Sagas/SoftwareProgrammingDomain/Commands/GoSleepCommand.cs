using System;
using GridDomain.CQRS;

namespace GridDomain.Tests.XUnit.Sagas.SoftwareProgrammingDomain.Commands
{
    public class GoSleepCommand: Command
    {
        public Guid SofaId { get; }
        public Guid PersonId { get; }
        public GoSleepCommand(Guid personId, Guid sofaId)
        {
            PersonId = personId;
            SofaId = sofaId;
        }
    }
}