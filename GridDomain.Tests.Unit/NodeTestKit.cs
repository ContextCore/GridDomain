using System;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Configuration;
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
        protected NodeTestKit(NodeTestFixture fixture) : base(Config(fixture), fixture.NodeConfig.Name)
        {
            var testClassName = GetType().Name; 
            fixture.LoggerConfiguration.WriteToFile(fixture.NodeConfig.LogLevel, testClassName);
    
            var node = fixture.CreateNode(() => Sys).Result;
            Node = fixture.CreateTestNode(node,this);
        }

        private static Config Config(NodeTestFixture fixture)
        {
            return fixture.ActorSystemConfigBuilder.Build();
        }

        protected ITestGridDomainNode Node { get; }
    }
}