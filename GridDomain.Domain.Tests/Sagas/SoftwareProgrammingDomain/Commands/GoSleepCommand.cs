using System;
using GridDomain.CQRS;
using Microsoft.Practices.Unity;

namespace GridDomain.Tests.Sagas.SoftwareProgrammingDomain.Commands
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