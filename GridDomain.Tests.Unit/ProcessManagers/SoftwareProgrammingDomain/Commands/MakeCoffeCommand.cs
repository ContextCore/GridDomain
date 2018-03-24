using System;
using GridDomain.CQRS;

namespace GridDomain.Tests.Unit.ProcessManagers.SoftwareProgrammingDomain.Commands
{
    internal class MakeCoffeCommand : Command<CoffeMachineAggregate>
    {
        public MakeCoffeCommand(string personId, string coffeMachineId) : base(personId)
        {
            CoffeMachineId = coffeMachineId;
        }

        public string PersonId => AggregateId;

        public string CoffeMachineId { get; }
    }
}