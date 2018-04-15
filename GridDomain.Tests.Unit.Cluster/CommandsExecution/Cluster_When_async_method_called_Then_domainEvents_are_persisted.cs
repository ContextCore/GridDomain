using GridDomain.Tests.Unit.CommandsExecution;
using Xunit.Abstractions;

namespace GridDomain.Tests.Unit.Cluster.CommandsExecution
{
    public class Cluster_When_async_method_called_Then_domainEvents_are_persisted : When_async_method_called_Then_domainEvents_are_persisted
    {
        public Cluster_When_async_method_called_Then_domainEvents_are_persisted(ITestOutputHelper output) :
            base(new NodeTestFixture(output).Clustered()) {}
    }
}