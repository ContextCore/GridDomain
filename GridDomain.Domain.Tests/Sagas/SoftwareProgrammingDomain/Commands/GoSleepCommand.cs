using System;
using GridDomain.CQRS;

namespace GridDomain.Tests.Sagas.SoftwareProgrammingDomain.Commands
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

    internal class GoToWorkCommand : Command
    {
        public Guid PersonId { get; }
        public GoToWorkCommand(Guid personId)
        {
            PersonId = personId;
        }
    }
}