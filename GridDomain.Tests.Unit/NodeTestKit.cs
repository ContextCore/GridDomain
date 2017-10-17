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
        protected NodeTestKit(NodeTestFixture fixture) : base(fixture.SystemConfigFactory(), fixture.Name)
        {
            Sys.AttachSerilogLogging(fixture.Logger);
            Node = fixture.CreateNode(() => Sys).Result;
        }

        protected GridDomainNode Node { get; }
    }
}