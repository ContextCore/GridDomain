using Akka.Event;
using GridDomain.Node;
using GridDomain.Tools.Connector;
using Xunit.Abstractions;

namespace GridDomain.Tests.Stress.NodeCommandExecution {
    public class ConnectorCreationAndChangeWithProjectionInMem : CreationAndChangeWithProjectionInMem
    {
        public ConnectorCreationAndChangeWithProjectionInMem(ITestOutputHelper output):base(output)
        {
            
        }

        protected override IGridDomainNode CreateNode()
        {
            base.CreateNode();
            var connector = new GridNodeClient(new StressTestNodeConfiguration(LogLevel.ErrorLevel).Network);
            connector.Connect()
                     .Wait();

            return connector;
        }
    }
}