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
using GridDomain.Tests.Unit.BalloonDomain.Configuration;
using GridDomain.Tests.Unit.BalloonDomain.ProjectionBuilders;
using GridDomain.Tests.Unit.Cluster;
using GridDomain.Tests.Unit.CommandsExecution.ExecutionWithErrors;
using Xunit;
using Xunit.Abstractions;

namespace GridDomain.Tests.Unit.CommandsExecution
{
    public class Cluster_When_awaiting_command_execution_without_prepare : When_awaiting_command_execution_without_prepare
    {
        public Cluster_When_awaiting_command_execution_without_prepare(ITestOutputHelper output) 
            : base(new NodeTestFixture(output).Clustered()) { }
    }
}
