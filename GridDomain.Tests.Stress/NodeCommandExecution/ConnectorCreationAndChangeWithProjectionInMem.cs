using System;
using Akka.Event;
using GridDomain.Node;
using GridDomain.Node.Configuration;
using GridDomain.Tests.Acceptance.BalloonDomain;
using GridDomain.Tests.Acceptance.GridConsole;
using GridDomain.Tools.Connector;
using Serilog.Events;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace GridDomain.Tests.Stress.NodeCommandExecution {
    public class ConnectorCreationAndChangeWithProjectionInMem : CreationAndChangeWithProjectionInMem, IDisposable
    {
        private AppDomainIsolated<Initiator> _isolatedServerNode;

        public ConnectorCreationAndChangeWithProjectionInMem(ITestOutputHelper output):base(output)
        {
            
        }

        class Initiator
        {
            public Initiator()
            {
                var testOutputHelper = new TestOutputHelper();
                var test = new CreationAndChangeWithProjectionInMem(testOutputHelper);
                test.OnSetup();
                var nodeConfig = new StressTestAkkaConfiguration(LogLevel.ErrorLevel);

                var node = new BalloonWithProjectionFixture(test.DbContextOptions)
                    {
                        Output = testOutputHelper,
                        AkkaConfig = nodeConfig,
                        LogLevel = LogEventLevel.Error,
                        SystemConfigFactory = () => nodeConfig.ToStandAloneInMemorySystemConfig()
                }.CreateNode().Result;
            }
        }

        internal override IGridDomainNode CreateNode()
        {
            _isolatedServerNode = new AppDomainIsolated<Initiator>();

            var connector = new GridNodeClient(new StressTestAkkaConfiguration().Network);
            connector.Connect()
                     .Wait();

            return connector;
        }

        public void Dispose()
        {
            _isolatedServerNode?.Dispose();
        }
    }
}