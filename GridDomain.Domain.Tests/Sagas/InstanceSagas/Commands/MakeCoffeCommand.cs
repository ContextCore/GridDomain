using System;
using GridDomain.CQRS;
using GridDomain.Tests.Sagas.InstanceSagas.Events;

namespace GridDomain.Tests.Sagas.InstanceSagas.Commands
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