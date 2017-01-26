using System;
using GridDomain.CQRS;

namespace GridDomain.Tests.XUnit.Sagas.SoftwareProgrammingDomain.Commands
{
    internal class GoToWorkCommand : Command
    {
        public Guid PersonId { get; }
        public GoToWorkCommand(Guid personId)
        {
            PersonId = personId;
        }
    }
}