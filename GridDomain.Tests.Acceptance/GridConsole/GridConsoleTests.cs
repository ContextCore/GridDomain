using System;
using System.Threading.Tasks;
using GridDomain.Node;
using GridDomain.Node.AkkaMessaging.Waiting;
using GridDomain.Node.Configuration.Composition;
using GridDomain.Tests.Unit;
using GridDomain.Tests.Unit.CommandsExecution;
using GridDomain.Tests.Unit.SampleDomain;
using GridDomain.Tests.Unit.SampleDomain.Commands;
using GridDomain.Tests.Unit.SampleDomain.Events;
using GridDomain.Tools.Connector;
using Microsoft.Practices.Unity;
using NUnit.Framework;

namespace GridDomain.Tests.Acceptance.GridConsole
{
    [TestFixture]
    public class GridConsoleTests
    {
        private GridNodeConnector _connector;
        private GridDomainNode _node;

        [OneTimeSetUp]
        public async Task Given_existing_GridNode()
        {
            var container = new UnityContainer();
            var sampleDomainContainerConfiguration = new SampleDomainContainerConfiguration();
            container.Register(sampleDomainContainerConfiguration);

            var serverConfig = new TestGridNodeConfiguration();

            _node = new GridDomainNode(sampleDomainContainerConfiguration,
                                       new SampleRouteMap(container),
                                       () => serverConfig.CreateInMemorySystem());

            await _node.Start();


            _connector = new GridNodeConnector(serverConfig.Network);
            _connector.Connect();
        }

        [OneTimeTearDown]
        public async Task TurnOffNode()
        {
            await _node.Stop();
        }

        [Then]
        public void Can_manual_reconnect_several_times()
        {
            _connector.Connect();
        }

        [Then]
        public void EventBusForwarder_is_located()
        {
            Assert.NotNull(_connector.EventBusForwarder);
        }

        [Then]
        public async Task Console_commands_are_executed_by_remote_node()
        {
            var command = new CreateSampleAggregateCommand(42, Guid.NewGuid());

            var evt =  await _connector.PrepareCommand(command)
                                       .Expect<SampleAggregateCreatedEvent>()
                                       .Execute();
                                   
            Assert.AreEqual(command.Parameter.ToString(), evt.Message<SampleAggregateCreatedEvent>().Value);
        }

    }
}
