using System;
using System.Diagnostics;
using System.Threading;
using Akka.Actor;
using Akka.TestKit.NUnit;
using GridDomain.Common;
using GridDomain.CQRS.Messaging;
using GridDomain.EventSourcing.Sagas.InstanceSagas;
using GridDomain.Node;
using GridDomain.Node.Actors;
using GridDomain.Node.Configuration.Composition;
using GridDomain.Node.Configuration.Persistence;
using GridDomain.Tests.Aggregate_Sagas_actor_lifetime.Infrastructure;
using GridDomain.Tests.Framework.Configuration;
using GridDomain.Tests.Sagas.InstanceSagas;
using GridDomain.Tests.Sagas.SoftwareProgrammingDomain.Events;
using GridDomain.Tests.Sagas.StateSagas.SampleSaga;
using NUnit.Framework;
using Microsoft.Practices.Unity;

namespace GridDomain.Tests.Aggregate_Sagas_actor_lifetime
{
    class PersistentHub_children_lifetime_test: TestKit
    {
        protected IPersistentActorTestsInfrastructure Infrastructure;
        private readonly PersistentHubTestsStatus.PersistenceCase _case;
        public PersistentHub_children_lifetime_test(PersistentHubTestsStatus.PersistenceCase @case):
            base(new AutoTestAkkaConfiguration().ToStandAloneInMemorySystemConfig(),"PersistentHubSystem")
        {
            _case = @case;
        }

        protected IContainerConfiguration CreateConfiguration()
        {
            return  new CustomContainerConfiguration(
                                                     c => c.RegisterInstance(TestActor),
                                                     c => c.RegisterStateSaga<Sagas.StateSagas.SampleSaga.SoftwareProgrammingSaga,
                                                                              SoftwareProgrammingSagaState,
                                                                              GotTiredEvent,
                                                                              Sagas.StateSagas.SampleSaga.SoftwareProgrammingSagaFactory>(),
                                                     c => c.RegisterSaga<Sagas.InstanceSagas.SoftwareProgrammingSaga,
                                                                            SoftwareProgrammingSagaData,
                                                                            GotTiredEvent,
                                                                            Sagas.InstanceSagas.SoftwareProgrammingSagaFactory>(),
                                                     c => c.RegisterAggregate<SagaDataAggregate<SoftwareProgrammingSagaData>,
                                                                              SagaDataAggregateCommandsHandlerDummy<SoftwareProgrammingSagaData>>()
                    );
        }

        protected void When_hub_creates_a_child()
        {
            HubRef.Tell(Infrastructure.ChildCreateMessage);
            Thread.Sleep(200);
        }

        protected void And_it_is_not_active_until_lifetime_period_is_expired()
        {
            Thread.Sleep(3000);
        }

        protected void And_command_for_child_is_sent()
        {
            HubRef.Tell(Infrastructure.ChildActivateMessage);
            Thread.Sleep(100);
        }

        protected PersistentHubActor Hub;
        protected IActorRef HubRef;
        private GridDomainNode _gridDomainNode;
        protected ChildInfo Child => Hub.Children[Infrastructure.ChildId];


        [SetUp]
        public void Clear_child_lifetimes()
        {
            _gridDomainNode = new GridDomainNode(CreateConfiguration(),new SoftwareProgrammingSagaRoutes(),() => new []{Sys});
            _gridDomainNode.Start(new LocalDbConfiguration());

            switch (_case)
            {
                    case PersistentHubTestsStatus.PersistenceCase.Aggregate:
                    Infrastructure = new AggregatePersistedHub_Infrastructure(_gridDomainNode.System);
                    break;

                     case PersistentHubTestsStatus.PersistenceCase.IstanceSaga:
                         Infrastructure = new InstanceSagaPersistedHub_Infrastructure(_gridDomainNode.System);
                         break;

                     case PersistentHubTestsStatus.PersistenceCase.StateSaga:
                         Infrastructure = new StateSagaPersistedHub_Infrastructure(_gridDomainNode.System);
                         break;
                     default: 
                    throw new UnknownCaseException();
            }
            var actorOfAsTestActorRef = ActorOfAsTestActorRef<PersistentHubActor>(Infrastructure.HubProps, "TestHub_" + Guid.NewGuid());
            Hub = actorOfAsTestActorRef.UnderlyingActor;
            HubRef = actorOfAsTestActorRef.Ref;
        }

        [TearDown]
        public void Down()
        {
            _gridDomainNode.Stop();
        }
    }
}