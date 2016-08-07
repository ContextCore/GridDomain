using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GridDomain.Node;
using GridDomain.Node.AkkaMessaging.Waiting;
using GridDomain.Node.Configuration.Akka;
using GridDomain.Tests.Framework.Configuration;
using GridDomain.Tests.SampleDomain.Commands;
using GridDomain.Tests.SampleDomain.Events;
using GridDomain.Tests.SynchroniousCommandExecute;
using GridDomain.Tools;
using NUnit.Framework;

namespace GridDomain.Tests.Tools.Console
{
    [TestFixture]
    class GridConsoleTests : PersistentSampleDomainTests
    {
        private IAkkaNetworkAddress _serverConfig;
        private GridConsole _console;

        [TestFixtureSetUp]
        public void Given_existing_GridNode()
        {
            _serverConfig = new AutoTestAkkaConfiguration().Network;
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
            var expect = ExpectedMessage.Once<SampleAggregateCreatedEvent>(e => e.SourceId, command.Id);

            var evt = _console.Execute(command, expect).Result;
            Assert.AreEqual(command.Parameter, evt.Value);
        }

    }
}
