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
        protected NodeTestKit(NodeTestFixture fixture) : base(fixture.ConfigBuilder(fixture.NodeConfig), fixture.Name)
        {
            var testClassName = $"Logs/{GetType().Name}.log"; 
            var logger = new XUnitAutoTestLoggerConfiguration(fixture.Output, fixture.NodeConfig.LogLevel, testClassName)
                                                            .CreateLogger();
            
            Node = fixture.CreateNode(() => Sys,logger).Result; 
        }

        protected GridDomainNode Node { get; }
    }
}