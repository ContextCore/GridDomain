using System;
using GridDomain.CQRS;
using GridDomain.Tests.Unit.BalloonDomain.Commands;
using GridDomain.Tests.Unit.BalloonDomain.Configuration;
using GridDomain.Tests.Unit.BalloonDomain.Events;
using GridDomain.Tests.Unit.Cluster;
using GridDomain.Tests.Unit.CommandsExecution.ExecutionWithErrors;
using Xunit;
using Xunit.Abstractions;

namespace GridDomain.Tests.Unit.CommandsExecution
{
    public class Cluster_SyncExecute_with_timeout : SyncExecute_with_timeout
    {
        public Cluster_SyncExecute_with_timeout(ITestOutputHelper output) : base(new NodeTestFixture(output).Clustered()) {}
      
    }
}