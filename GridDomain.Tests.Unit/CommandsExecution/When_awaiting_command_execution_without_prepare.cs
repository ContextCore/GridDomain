using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Akka.Actor;
using GridDomain.Tests.Common;
using GridDomain.Tests.Unit.BalloonDomain;
using GridDomain.Tests.Unit.BalloonDomain.Commands;
using GridDomain.Tools.Repositories.EventRepositories;
using GridDomain.CQRS;
using GridDomain.Node.Configuration.Composition;
using GridDomain.Tests.Unit.BalloonDomain.ProjectionBuilders;
using Xunit;
using Xunit.Abstractions;

namespace GridDomain.Tests.Unit.CommandsExecution
{
    public class When_awaiting_command_execution_without_prepare : BalloonDomainCommandExecutionTests
    {
        public When_awaiting_command_execution_without_prepare(ITestOutputHelper output) : base(output) { }

        [Fact]
        public async Task Then_command_executed_aggregate_is_persisted()
        {
            var aggregateId = Guid.NewGuid();
            await Node.Execute(new InflateCopyCommand(100, aggregateId),
                               new WriteTitleCommand(200, aggregateId));

            var aggregate = await Node.LoadAggregate<Balloon>(aggregateId);
            Assert.Equal("200", aggregate.Title);
        }
    }
}
