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

        protected NodeTestKit(ITestOutputHelper output, NodeTestFixture fixture) : base(fixture.SystemConfigFactory(), fixture.Name)
        {
            Fixture = fixture;
            Fixture.ActorSystemCreator = () => Sys;
            Fixture.Output = output;
            AkkaConfig = fixture.AkkaConfig;
            Node = fixture.CreateNode().Result;
        }

        protected GridDomainNode Node { get; }
        protected NodeTestFixture Fixture { get; }

        protected override void AfterAll()
        {
            var memoryInBytes = System.Diagnostics.Process.GetCurrentProcess().VirtualMemorySize64;
            
            Fixture.Output.WriteLine($"Total memory consumtion after test {GetType().Name}: {memoryInBytes / 1024000} Mb");
        }
    }
}