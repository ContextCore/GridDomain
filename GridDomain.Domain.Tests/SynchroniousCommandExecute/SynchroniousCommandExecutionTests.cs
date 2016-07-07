using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GridDomain.CQRS;
using GridDomain.Node;
using GridDomain.Node.AkkaMessaging.Waiting;
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
            var syncCommand = new LongOperationCommand(42, Guid.NewGuid());
            GridNode.ConfirmedExecute(syncCommand, 
                                      ExpectedMessage.Once<AggregateChangedEvent>(nameof(AggregateChangedEvent.SourceId)),
                                      Timeout);
            var aggregate = LoadAggregate<SampleAggregate>(syncCommand.AggregateId);
            Assert.AreEqual(syncCommand.Parameter, aggregate.Value);
        }


        [Then]
        public void When_command_executed_asynchroniosly_Then_aggregate_doesnt_have_events_after_finish()
        {
            var  syncCommand = new LongOperationCommand(42, Guid.NewGuid());
            GridNode.Execute(syncCommand);
            var aggregate = LoadAggregate<SampleAggregate>(syncCommand.AggregateId);
            Assert.AreNotEqual(syncCommand.Parameter, aggregate.Value);
        }
    }



    public class ChangeAggregateCommand : Command
    {
        public int Parameter { get; }

        public Guid AggregateId { get; }

        public ChangeAggregateCommand(Guid aggregateId, int parameter)
        {
            AggregateId = aggregateId;
            Parameter = parameter;
        }
    }
}
