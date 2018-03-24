using System;
using System.Threading.Tasks;
using Akka.TestKit.Xunit2;
using GridDomain.Node;
using GridDomain.Node.Configuration;
using GridDomain.Transport.Extension;
using Xunit.Abstractions;

namespace GridDomain.Tests.Unit
{
    public class NodeTestKit : TestKit
    {
        protected NodeTestKit(NodeTestFixture fixture) : base(fixture.SystemConfig.Value, fixture.Name)
        {
            var testClassName = $"Logs/{GetType().Name}.log"; 
            var logger = new XUnitAutoTestLoggerConfiguration(fixture.Output, fixture.NodeConfig.LogLevel, testClassName)
                                    .CreateLogger();
            Sys.AttachSerilogLogging(logger);
            Sys.InitLocalTransportExtension();
            Node = fixture.CreateNode(() => Sys,logger).Result;
        }

        protected GridDomainNode Node { get; }
    }
}