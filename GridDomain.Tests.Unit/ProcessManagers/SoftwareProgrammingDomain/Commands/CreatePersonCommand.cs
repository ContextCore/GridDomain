using System;
using GridDomain.CQRS;

namespace GridDomain.Tests.Unit.ProcessManagers.SoftwareProgrammingDomain.Commands {
    public class CreatePersonCommand : Command<ProgrammerAggregate>
    {
        public CreatePersonCommand(string aggregateId) : base(aggregateId) { }
    }
}