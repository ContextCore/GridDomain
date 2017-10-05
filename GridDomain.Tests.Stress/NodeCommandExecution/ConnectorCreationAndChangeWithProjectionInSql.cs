using Akka.Event;
using GridDomain.Node;
using GridDomain.Tests.Acceptance.GridConsole;
using GridDomain.Tools.Connector;
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
                var test = new CreationAndChangeWithProjectionInSql(new TestOutputHelper());
                var node = test.CreateNode();
            }
        }
        private Isolated<Initiator> _isolatedServerNode;

        internal override IGridDomainNode CreateNode()
        {
            _isolatedServerNode = new Isolated<Initiator>();

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