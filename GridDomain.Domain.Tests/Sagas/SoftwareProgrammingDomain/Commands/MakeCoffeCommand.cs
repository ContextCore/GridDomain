using System;
using GridDomain.CQRS;

namespace GridDomain.Tests.Sagas.SoftwareProgrammingDomain.Commands
{
    internal class MakeCoffeCommand : Command
    {
        public Guid PersonId { get;}

        public Guid CoffeMachineId { get; }
        public MakeCoffeCommand(Guid personId, Guid coffeMachineId)
        {
            PersonId = personId;
            CoffeMachineId = coffeMachineId;
        }
    }
}