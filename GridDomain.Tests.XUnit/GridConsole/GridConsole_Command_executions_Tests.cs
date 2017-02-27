using System;
using System.Threading.Tasks;
using GridDomain.CQRS;
using GridDomain.Node;
using GridDomain.Node.AkkaMessaging.Waiting;
using GridDomain.Node.Configuration.Composition;
using GridDomain.Tests.XUnit.SampleDomain;
using GridDomain.Tests.XUnit.SampleDomain.Commands;
using GridDomain.Tests.XUnit.SampleDomain.Events;
using GridDomain.Tools.Connector;
using Microsoft.Practices.Unity;
using Xunit;

namespace GridDomain.Tests.XUnit.GridConsole
{
    public class GridConsole_Command_executions_Tests : IDisposable
    {
        private GridNodeConnector _connector;
        private GridDomainNode _serverNode;

        [Fact]
        public async Task Given_existing_GridNode()
        {
            var container = new UnityContainer();
            var sampleDomainContainerConfiguration = new SampleDomainContainerConfiguration();
            container.Register(sampleDomainContainerConfiguration);
           
            var serverConfig = new TestGridNodeConfiguration();
           
            var settings = new NodeSettings(sampleDomainContainerConfiguration,
                                            new SampleRouteMap(),
                                            () => new [] {serverConfig.CreateInMemorySystem()});
           
            _serverNode = new GridDomainNode(settings);
           
            await _serverNode.Start();

            _connector = new GridNodeConnector(serverConfig.Network,null,TimeSpan.FromSeconds(5));

            await _connector.Connect();
            //Console_commands_are_executed_by_remote_node()
            var command = new CreateSampleAggregateCommand(42, Guid.NewGuid());

            var evt =  await _connector.Prepare(command)
                                       .Expect<SampleAggregateCreatedEvent>()
                                       .Execute();
                                   
            Assert.Equal(command.Parameter.ToString(), evt.Message<SampleAggregateCreatedEvent>().Value);
        }

        public void Dispose()
        {
            _serverNode.Dispose();
            _connector.Dispose();
        }
    }
}
