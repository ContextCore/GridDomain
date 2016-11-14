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
    [Ignore("Console is not relevant now")]
    public class GridConsoleTests
    {
        private GridConsole _console;
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
            _console = new GridConsole(serverConfig.Network);
            _console.Connect();
        }

        [OneTimeTearDown]
        public void TurnOffNode()
        {
            _node.Stop();
        }

        [Then]
        public void Can_manual_reconnect_several_times()
        {
            _console.Connect();
        }

        [Then]
        public void NodeController_is_located()
        {
            Assert.NotNull(_console.NodeController);
        }

        [Then]
        public async Task Console_commands_are_executed()
        {
            var command = new CreateSampleAggregateCommand(42, Guid.NewGuid());

            var expect = Expect.Message<SampleAggregateCreatedEvent>(e => e.SourceId, command.AggregateId);

            var evt = await _console.Execute(CommandPlan.New(command, TimeSpan.FromSeconds(30), expect));
            Assert.AreEqual(command.Parameter.ToString(), evt.Value);
        }

    }
}
