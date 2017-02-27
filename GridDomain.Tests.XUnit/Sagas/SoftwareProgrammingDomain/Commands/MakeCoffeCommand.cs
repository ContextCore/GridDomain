using System;
using GridDomain.CQRS;

namespace GridDomain.Tests.XUnit.Sagas.SoftwareProgrammingDomain.Commands
{
    internal class MakeCoffeCommand : Command
    {
        public Guid PersonId => AggregateId;

        public Guid CoffeMachineId { get; }
        public MakeCoffeCommand(Guid personId, Guid coffeMachineId):base(personId)
        {
            CoffeMachineId = coffeMachineId;
        }
    }
}