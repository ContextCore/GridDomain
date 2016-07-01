using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GridDomain.Node;
using GridDomain.Node.Configuration.Akka;
using GridDomain.Node.Configuration.Composition;
using GridDomain.Node.Configuration.Persistence;
using GridDomain.Tests.Framework;
using GridDomain.Tests.Framework.Configuration;
using GridDomain.Tests.SyncProjection.SampleDomain;
using Microsoft.Practices.Unity;
using NUnit.Framework;
using UnityServiceLocator = GridDomain.Node.UnityServiceLocator;

namespace GridDomain.Tests
{
    [TestFixture]
    class SynchroniousCommandExecutionTests : NodeCommandsTest
    {
        private ChangeAggregateWaitableCommand _syncCommand;

        public SynchroniousCommandExecutionTests():base(new AutoTestAkkaConfiguration().ToStandAloneInMemorySystemConfig(), "SyncExecution", false)
        {
        }

        protected override TimeSpan Timeout => TimeSpan.FromMinutes(1);

        protected override GridDomainNode CreateGridDomainNode(AkkaConfiguration akkaConf, IDbConfiguration dbConfig)
        {
            var container = new UnityContainer();
            var system = ActorSystemFactory.CreateActorSystem(akkaConf);
            CompositionRoot.Init(container, system, dbConfig, TransportMode.Standalone);
            container.RegisterAggregate<SampleAggregate, TestAggregatesCommandHandler>();

            return new GridDomainNode(container,
                                      new TestRouteMap(new UnityServiceLocator(container)),
                                      TransportMode.Standalone, system);
        }

       
        [Then]
        public void When_command_executed_synchroniosly_Then_aggregate_already_has_events_after_finish()
        {
            _syncCommand = new ChangeAggregateWaitableCommand(42, Guid.NewGuid());
            GridNode.ExecuteWithConfirmation(_syncCommand);
      
            var aggregate = LoadAggregate<SampleAggregate>(_syncCommand.AggregateId);
            Assert.AreEqual(_syncCommand.Parameter, aggregate.Value);
        }


        [Then]
        public void When_command_executed_asynchroniosly_Then_aggregate_doesnt_have_events_after_finish()
        {
            _syncCommand = new ChangeAggregateWaitableCommand(42, Guid.NewGuid());
            GridNode.Execute(_syncCommand);

            var aggregate = LoadAggregate<SampleAggregate>(_syncCommand.AggregateId);
            Assert.AreNotEqual(_syncCommand.Parameter, aggregate.Value);
        }
    }

    public class ChangeAggregateWaitableCommand : CommandWithKnownResult
    {
        public int Parameter { get; }

        public Guid AggregateId { get; }

        public ChangeAggregateWaitableCommand(int param, Guid aggregateId) : base(typeof(AggregateChangedEvent))
        {
            Parameter = param;
            AggregateId = aggregateId;
        }
    }
}
