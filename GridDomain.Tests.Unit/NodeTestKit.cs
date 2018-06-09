using System;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.TestKit.Xunit2;
using GridDomain.Node;
using GridDomain.Node.Configuration;
using GridDomain.Tests.Common;
using GridDomain.Transport.Extension;
using Serilog;
using Xunit.Abstractions;

namespace GridDomain.Tests.Unit
{
    public class NodeTestKit : TestKit
    {
        protected NodeTestKit(NodeTestFixture fixture) : base(fixture.ActorSystemConfigBuilder.Build(), fixture.Name)
        {
            var testClassName = GetType().Name; 
            var logger = new XUnitAutoTestLoggerConfiguration(fixture.Output, fixture.NodeConfig.LogLevel, testClassName)
                                                            .CreateLogger();
            
            var node = fixture.CreateNode(() => Sys,logger).Result;
            Node = fixture.CreateTestNode(node,this);
        }

        protected ITestGridDomainNode Node { get; }
    }
}