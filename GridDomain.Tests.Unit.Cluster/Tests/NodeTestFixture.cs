using System;
using GridDomain.Configuration;
using GridDomain.Node.Cluster;
using GridDomain.Node.Configuration;
using Xunit.Abstractions;

namespace GridDomain.Tests.Unit.Cluster.Tests
{
    public class ClusterNodeTestFixture : NodeTestFixture
    {
        public ClusterNodeTestFixture(ITestOutputHelper output, IDomainConfiguration domainConfiguration) : this(output, new []{domainConfiguration}) { }
        public ClusterNodeTestFixture(ITestOutputHelper output, IDomainConfiguration[] domainConfiguration = null, TimeSpan? defaultTimeout = null) : this(output, null,null, domainConfiguration,defaultTimeout) { }
        public ClusterNodeTestFixture(ITestOutputHelper output, NodeConfiguration cfg, Func<NodeConfiguration, string> systemConfigFactorry = null, IDomainConfiguration[] domainConfiguration = null, TimeSpan? defaultTimeout = null) : 
            base(output, cfg, systemConfigFactorry ?? (n => n.ToClusterConfig()), domainConfiguration, defaultTimeout) { }
    }
}