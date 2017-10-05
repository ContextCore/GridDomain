using System;
using Akka.Event;
using GridDomain.Node;
using GridDomain.Tests.Acceptance.GridConsole;
using GridDomain.Tools.Connector;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace GridDomain.Tests.Stress.NodeCommandExecution {
    public class ConnectorCreationAndChangeWithProjectionInMem : CreationAndChangeWithProjectionInMem, IDisposable
    {
        private Isolated<Initiator> _isolatedServerNode;

        public ConnectorCreationAndChangeWithProjectionInMem(ITestOutputHelper output):base(output)
        {
            
        }

        class Initiator
        {
            public Initiator()
            {
                var test = new CreationAndChangeWithProjectionInMem(new TestOutputHelper());
                test.OnSetup();
                var node = test.CreateNode();
            }
        }

        internal override IGridDomainNode CreateNode()
        {
            _isolatedServerNode = new Isolated<Initiator>();

            var connector = new GridNodeClient(new StressTestNodeConfiguration().Network);
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