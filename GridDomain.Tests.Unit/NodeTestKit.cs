using System;
using System.Threading.Tasks;
using Akka.TestKit.Xunit2;
using GridDomain.Node;
using GridDomain.Node.Configuration;
using Xunit.Abstractions;

namespace GridDomain.Tests.Unit
{
    public class NodeTestKit : TestKit
    {
        protected NodeTestKit(ITestOutputHelper output, NodeTestFixture fixture) : base(fixture.SystemConfigFactory(), fixture.Name)
        {
            Fixture = fixture;
            Fixture.ActorSystemCreator = () => Sys;
            Fixture.Output = output;
            Node = fixture.CreateNode().Result;
        }

        protected GridDomainNode Node { get; }
        protected NodeTestFixture Fixture { get; }

        protected override void AfterAll()
        {
            Fixture.Dispose();
        }
        
    }
}