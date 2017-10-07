using System;
using Akka.Event;
using GridDomain.Node;
using GridDomain.Node.Configuration;
using GridDomain.Tests.Acceptance;
using GridDomain.Tests.Acceptance.BalloonDomain;
using GridDomain.Tests.Acceptance.GridConsole;
using GridDomain.Tests.Common;
using GridDomain.Tests.Unit;
using GridDomain.Tools.Connector;
using NBench;
using Serilog.Core;
using Serilog.Events;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace GridDomain.Tests.Stress.NodeCommandExecution {
    public class ConnectorCreationAndChangeWithProjectionInSql : CreationAndChangeWithProjectionInSql
    {
        public ConnectorCreationAndChangeWithProjectionInSql(ITestOutputHelper output) : base(output)
        {

        }

        private static Isolated<Initiator> _isolatedServerNode = new Isolated<Initiator>();
        class Initiator : MarshalByRefObject
        {
            private readonly CreationAndChangeWithProjectionInSql _test;

            public Initiator()
            {
                _test = new CreationAndChangeWithProjectionInSql(new SerilogTestOutput());
            }
            public void CleanUp()
            {
                _test.Cleanup();
            }
            public void Setup()
            {
                _test.OnSetup();
            }
        }

        internal override IGridDomainNode CreateNode()
        {
            var connector = new GridNodeConnector(new StressTestNodeConfiguration());
            connector.Connect()
                     .Wait();

            return connector;
        }

        internal override void OnSetup()
        {
            _isolatedServerNode.Value.Setup();
            base.OnSetup();
        }

        public override void Cleanup()
        {
            _isolatedServerNode.Value.CleanUp();
            //_isolatedServerNode?.Dispose();
            base.Cleanup();
        }
    }
}