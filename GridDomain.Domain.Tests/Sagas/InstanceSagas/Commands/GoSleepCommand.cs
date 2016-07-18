using System;
using GridDomain.CQRS;

namespace GridDomain.Tests.Sagas.InstanceSagas.Commands
{
    internal class GoSleepCommand: Command
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