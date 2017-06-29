using System;
using System.Threading.Tasks;
using Akka.TestKit.Xunit2;
using GridDomain.Node;
using GridDomain.Node.Configuration.Akka;
using Xunit.Abstractions;

namespace GridDomain.Tests.Unit
{
    public class NodeTestKit : TestKit
    {
        protected readonly AkkaConfiguration AkkaConfig;

        protected NodeTestKit(ITestOutputHelper output, NodeTestFixture fixture) : base(fixture.GetConfig(), fixture.Name)
        {
            Fixture = fixture;
            Fixture.System = Sys;
            Fixture.Output = output;
            AkkaConfig = fixture.AkkaConfig;
            Node = fixture.CreateNode().Result;
        }

        protected GridDomainNode Node { get; }
        protected NodeTestFixture Fixture { get; }
    }
}