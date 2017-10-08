using System;
using GridDomain.CQRS;

namespace GridDomain.Tests.Unit.ProcessManagers.SoftwareProgrammingDomain.Commands {
    public class CreatePersonCommand : Command
    {
        public CreatePersonCommand(Guid aggregateId) : base(aggregateId) { }
    }
}