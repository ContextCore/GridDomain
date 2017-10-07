using System;
using System.Collections.Generic;
using Akka.Event;
using GridDomain.Node;
using GridDomain.Node.Configuration;
using GridDomain.Tests.Acceptance.BalloonDomain;
using GridDomain.Tests.Acceptance.GridConsole;
using GridDomain.Tests.Common;
using GridDomain.Tests.Unit;
using GridDomain.Tools.Connector;
using NBench;
using NBench.Metrics.Counters;
using NBench.Util;
using Serilog.Events;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace GridDomain.Tests.Stress.NodeCommandExecution
{
    public class ConnectorCreationAndChangeWithProjectionInMem : CreationAndChangeWithProjectionInMem
    {
        public ConnectorCreationAndChangeWithProjectionInMem(ITestOutputHelper output):base(output)
        {
        }

        private Isolated<Initiator> _isolatedServerNode;
        class Initiator : MarshalByRefObject
        {
            private readonly CreationAndChangeWithProjectionInMem _test;

            public Initiator()
            {
                _test = new CreationAndChangeWithProjectionInMem(new SerilogTestOutput());
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
            connector.Connect().Wait();
            return connector;
        }

        internal override void OnSetup()
        {
            _isolatedServerNode = new Isolated<Initiator>();
            _isolatedServerNode.Value.Setup();
            base.OnSetup();
        }

        public override void Cleanup()
        {
            _isolatedServerNode.Value.CleanUp();
            //TODO: think how to 
            _isolatedServerNode?.Dispose();
            base.Cleanup();
        }
    }
}