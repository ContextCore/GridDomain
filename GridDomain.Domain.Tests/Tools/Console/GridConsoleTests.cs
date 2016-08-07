using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GridDomain.Node;
using GridDomain.Node.AkkaMessaging.Waiting;
using GridDomain.Node.Configuration.Akka;
using GridDomain.Node.Configuration.Composition;
using GridDomain.Node.Configuration.Persistence;
using GridDomain.Tests.Framework.Configuration;
using GridDomain.Tests.SampleDomain;
using GridDomain.Tests.SampleDomain.Commands;
using GridDomain.Tests.SampleDomain.Events;
using GridDomain.Tests.SynchroniousCommandExecute;
using GridDomain.Tools;
using Microsoft.Practices.Unity;
using NUnit.Framework;

namespace GridDomain.Tests.Tools.Console
{
    [TestFixture]
    class GridConsoleTests
    {
        private IAkkaNetworkAddress _serverConfig;
        private GridConsole _console;



        [TestFixtureSetUp]
        public void Given_existing_GridNode()
        {
            var container = new UnityContainer();
            var sampleDomainContainerConfiguration = new SampleDomainContainerConfiguration();
            container.Register(sampleDomainContainerConfiguration);

            var serverConfig = new AutoTestAkkaConfiguration();

            var node = new GridDomainNode(sampleDomainContainerConfiguration,
                                          new SampleRouteMap(container),
                                          () => new [] { serverConfig.CreateInMemorySystem()});

            node.Start(new LocalDbConfiguration());

            _serverConfig = serverConfig.Network;
            _console = When_connect_by_console_with_default_client_configuration(_serverConfig);
        }


        public GridConsole When_connect_by_console_with_default_client_configuration(IAkkaNetworkAddress akkaNetworkAddress)
        {
            var console = new GridConsole(akkaNetworkAddress);
            console.Connect();
            return console;
        }

        [Then]
        public void Can_manual_reconnect_several_times()
        {
            _console.Connect();
        }

        [Then]
        public void NodeController_is_located()
        {
            Assert.NotNull(_console._nodeController);
        }

        [Then]
        public void Console_commands_are_executed()
        {
            var command = new CreateSampleAggregateCommand(42, Guid.NewGuid(), Guid.NewGuid());

            var expect = ExpectedMessage.Once<SampleAggregateCreatedEvent>(e => e.SourceId, command.AggregateId);

            var evt = _console.Execute(command, TimeSpan.FromSeconds(30), expect);
            Assert.AreEqual(command.Parameter.ToString(), evt.Value);
        }

    }
}
