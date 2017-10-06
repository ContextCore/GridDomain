using Akka.Event;
using GridDomain.Node;
using GridDomain.Node.Configuration;
using GridDomain.Node.Persistence.Sql;
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
        //TODO: cleanup node initialization 
        class Initiator
        {
            public Initiator()
            {
                var test = new CreationAndChangeWithProjectionInSql(null);
                test.OnSetup();
                var nodeConfig = new StressTestAkkaConfiguration(LogLevel.ErrorLevel);
                var cfg = new BalloonWithProjectionDomainConfiguration(test.DbContextOptions);

                var node = new GridDomainNode(new[]{cfg}, new DelegateActorSystemFactory(() => nodeConfig.CreateSystem()));
                node.Start().Wait();
            }
        }

        private AppDomainIsolated<Initiator> _isolatedServerNode;

        internal override IGridDomainNode CreateNode()
        {
            _isolatedServerNode = new AppDomainIsolated<Initiator>();

            var connector = new GridNodeClient(new StressTestAkkaConfiguration(LogLevel.ErrorLevel).Network);
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