using System;
using System.Threading.Tasks;
using GridDomain.Common;
using GridDomain.Configuration;
using GridDomain.Configuration.MessageRouting;
using GridDomain.CQRS;
using GridDomain.Tests.Unit.BalloonDomain.Commands;
using GridDomain.Tests.Unit.BalloonDomain.Configuration;
using GridDomain.Tests.Unit.BalloonDomain.Events;
using GridDomain.Tests.Unit.Cluster;
using Xunit;
using Xunit.Abstractions;

namespace GridDomain.Tests.Unit.CommandsExecution {
    public class Cluster_When_awaiting_command_execution_without_prepare_and_counting : When_awaiting_command_execution_without_prepare_and_counting
    {
        public Cluster_When_awaiting_command_execution_without_prepare_and_counting(ITestOutputHelper output) :
            base(new NodeTestFixture(output).Clustered()){ }
    }
}