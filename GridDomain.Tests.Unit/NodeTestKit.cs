using System;
using System.Threading.Tasks;
using Akka.TestKit.Xunit2;
using GridDomain.Node;
using GridDomain.Node.Configuration;
using GridDomain.Transport.Extension;
using Serilog;
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
            Node = CreateNode(fixture, logger);
        }

        protected virtual GridDomainNode CreateNode(NodeTestFixture fixture, ILogger logger)
        {
            Sys.InitLocalTransportExtension();
            return (GridDomainNode)fixture.CreateNode(() => Sys,logger).Result;
        }
        protected GridDomainNode Node { get; }
    }
}