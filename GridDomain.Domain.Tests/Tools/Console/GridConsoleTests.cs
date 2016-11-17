using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GridDomain.CQRS;
using GridDomain.Node;
using GridDomain.Node.AkkaMessaging.Waiting;
using GridDomain.Node.Configuration.Akka;
using GridDomain.Node.Configuration.Composition;
using GridDomain.Node.Configuration.Persistence;
using GridDomain.Tests.CommandsExecution;
using GridDomain.Tests.SampleDomain;
using GridDomain.Tests.SampleDomain.Commands;
using GridDomain.Tests.SampleDomain.Events;
using GridDomain.Tools;
using GridDomain.Tools.Console;
using Microsoft.Practices.Unity;
using NUnit.Framework;

namespace GridDomain.Tests.Tools.Console
{
    [TestFixture]
   // [Ignore("Console is not relevant now")]
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
        public void NodeController_is_located()
        {
            Assert.NotNull(_connector.EventBusForwarder);
        }

        [Then]
        public void Console_commands_are_executed_by_remote_node()
        {
            var command = new CreateSampleAggregateCommand(42, Guid.NewGuid());

            var evt =  _connector.NewCommandWaiter(TimeSpan.FromDays(1))
                                    .Expect<SampleAggregateCreatedEvent>()
                                    .Create()
                                    .Execute(command)
                                    .Result;

            Assert.AreEqual(command.Parameter.ToString(), evt.Message<SampleAggregateCreatedEvent>().Value);
        }

    }
}
