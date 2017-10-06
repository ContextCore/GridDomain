using Akka.Event;
using GridDomain.Node;
using GridDomain.Node.Configuration;
using GridDomain.Tests.Acceptance.BalloonDomain;
using GridDomain.Tests.Acceptance.GridConsole;
using GridDomain.Tools.Connector;
using NBench;
using Serilog.Events;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace GridDomain.Tests.Stress.NodeCommandExecution {
    public class ConnectorCreationAndChangeWithProjectionInSql : CreationAndChangeWithProjectionInSql
    {
        public ConnectorCreationAndChangeWithProjectionInSql(ITestOutputHelper output) : base(output)
        {

        }

        class Initiator
        {
            public Initiator()
            {
                var testOutputHelper = new TestOutputHelper();
                var test = new CreationAndChangeWithProjectionInSql(testOutputHelper);
                test.OnSetup();
                var nodeConfig = new StressTestNodeConfiguration(LogLevel.ErrorLevel);

                var node = new BalloonWithProjectionFixture(test.DbContextOptions)
                           {
                               Output = testOutputHelper,
                               NodeConfig = nodeConfig,
                               LogLevel = LogEventLevel.Error,
                               SystemConfigFactory = () => nodeConfig.ToStandAloneSystemConfig()
                           }.CreateNode().Result;
            }
        }

        private AppDomainIsolated<Initiator> _isolatedServerNode;

        internal override IGridDomainNode CreateNode()
        {
            _isolatedServerNode = new AppDomainIsolated<Initiator>();

            var connector = new GridNodeClient(new StressTestNodeConfiguration(LogLevel.ErrorLevel).Network);
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