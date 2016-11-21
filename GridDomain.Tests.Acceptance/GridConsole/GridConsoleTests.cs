using System;
using System.Threading.Tasks;
using GridDomain.Node;
using GridDomain.Node.AkkaMessaging.Waiting;
using GridDomain.Node.Configuration.Composition;
using GridDomain.Node.Configuration.Persistence;
using GridDomain.Tests.CommandsExecution;
using GridDomain.Tests.SampleDomain;
using GridDomain.Tests.SampleDomain.Commands;
using GridDomain.Tests.SampleDomain.Events;
using GridDomain.Tools.Console;
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
        public void Given_existing_GridNode()
        {
            var container = new UnityContainer();
            var sampleDomainContainerConfiguration = new SampleDomainContainerConfiguration();
            container.Register(sampleDomainContainerConfiguration);

            var serverConfig = new TestGridNodeConfiguration();

            _node = new GridDomainNode(sampleDomainContainerConfiguration,
                                       new SampleRouteMap(container),
                                       () => serverConfig.CreateInMemorySystem());

            _node.Start(new LocalDbConfiguration());


            _connector = new GridNodeConnector(serverConfig.Network);
            _connector.Connect();
        }

        [OneTimeTearDown]
        public void TurnOffNode()
        {
            _node.Stop();
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

            var evt =  await _connector.NewCommandWaiter(TimeSpan.FromDays(1))
                                       .Expect<SampleAggregateCreatedEvent>()
                                       .Create()
                                       .Execute(command);
                                   
            Assert.AreEqual(command.Parameter.ToString(), evt.Message<SampleAggregateCreatedEvent>().Value);
        }

    }
}
