using System;
using GridDomain.CQRS;

namespace GridDomain.Tests.Unit.Sagas.SoftwareProgrammingDomain.Commands
{
    internal class MakeCoffeCommand : Command
    {
        public MakeCoffeCommand(Guid personId, Guid coffeMachineId) : base(personId)
        {
            CoffeMachineId = coffeMachineId;
        }

        public Guid PersonId => AggregateId;

        public Guid CoffeMachineId { get; }
    }
}