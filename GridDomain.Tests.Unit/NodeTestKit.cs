using System;
using System.Threading.Tasks;
using Akka.TestKit.Xunit2;
using GridDomain.Node;
using GridDomain.Node.Configuration.Akka;
using Xunit.Abstractions;

namespace GridDomain.Tests.Unit
{
    public abstract class NodeTestKit : TestKit
    {
        private readonly Lazy<Task<GridDomainNode>> _lazyNode;
        protected readonly AkkaConfiguration AkkaConfig;

        protected NodeTestKit(ITestOutputHelper output, NodeTestFixture fixture) : base(fixture.GetConfig(), fixture.Name)
        {
            Fixture = fixture;
            Fixture.System = Sys;
            Fixture.Output = output;
            AkkaConfig = fixture.AkkaConfig;
            _lazyNode = new Lazy<Task<GridDomainNode>>(fixture.CreateNode);
        }

        protected GridDomainNode Node => _lazyNode.Value.Result;
        protected NodeTestFixture Fixture { get; }
    }
}