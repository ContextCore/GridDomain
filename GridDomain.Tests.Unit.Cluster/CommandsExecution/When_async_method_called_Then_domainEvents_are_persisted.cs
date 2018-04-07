using System;
using System.Threading.Tasks;
using GridDomain.CQRS;
using GridDomain.Tests.Unit.BalloonDomain;
using GridDomain.Tests.Unit.BalloonDomain.Commands;
using GridDomain.Tests.Unit.BalloonDomain.Configuration;
using GridDomain.Tests.Unit.BalloonDomain.Events;
using GridDomain.Tests.Unit.Cluster;
using GridDomain.Tests.Unit.CommandsExecution.ExecutionWithErrors;
using Xunit;
using Xunit.Abstractions;

namespace GridDomain.Tests.Unit.CommandsExecution
{
    public class Cluster_When_async_method_called_Then_domainEvents_are_persisted : When_async_method_called_Then_domainEvents_are_persisted
    {
        public Cluster_When_async_method_called_Then_domainEvents_are_persisted(ITestOutputHelper output) :
            base(new NodeTestFixture(output).Clustered()) {}
    }
}