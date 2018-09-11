using System;
using GridDomain.CQRS;
using GridDomain.ProcessManagers;

namespace GridDomain.Tests.Unit.ProcessManagers.SoftwareProgrammingDomain.Commands
{
    public class MakeCoffeCommand : Command<CoffeMachineAggregate>
    {
        public MakeCoffeCommand(string personId, string coffeMachineId) : base(personId)
        {
            CoffeMachineId = coffeMachineId;
        }

        public string PersonId => AggregateId;

        public string CoffeMachineId { get; }
    }
}