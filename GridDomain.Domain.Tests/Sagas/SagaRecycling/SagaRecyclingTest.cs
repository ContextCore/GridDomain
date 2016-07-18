using System;
using System.Diagnostics;
using System.Threading;
using GridDomain.CQRS.Messaging;
using GridDomain.EventSourcing.Sagas;
using GridDomain.Node;
using GridDomain.Node.Configuration.Akka;
using GridDomain.Node.Configuration.Persistence;
using GridDomain.Scheduling.Quartz;
using GridDomain.Tests.Framework;
using GridDomain.Tests.Framework.Configuration;
using GridDomain.Tests.FutureEvents;
using GridDomain.Tests.Sagas.SagaRecycling.Saga;
using Microsoft.Practices.Unity;
using NUnit.Framework;

namespace GridDomain.Tests.Sagas.SagaRecycling
{
    [TestFixture]
    public class SagaRecyclingTest : NodeCommandsTest
    {

        [Test]
        public void SagaRecycling()
        {
            var publisher = GridNode.Container.Resolve<IPublisher>();
            var sagaId = Guid.NewGuid();

            Thread.Sleep(TimeSpan.FromMilliseconds(500));

            publisher.Publish(new StartEvent(Guid.NewGuid()).CloneWithSaga(sagaId));

            Thread.Sleep(TimeSpan.FromMilliseconds(3000));
            var sagaState = LoadSagaState<SagaForRecycling,
                                          State,
                                          StartEvent>(sagaId);
            Thread.Sleep(TimeSpan.FromMilliseconds(10000));
            Assert.AreEqual(sagaId, sagaState.Id);
            Assert.AreEqual(States.Created, sagaState.MachineState);
        }

        public SagaRecyclingTest() : base(new AutoTestAkkaConfiguration().ToStandAloneInMemorySystemConfig(), "TestSagaStart", false)
        {
        }

        protected override TimeSpan Timeout { get; }
        protected override GridDomainNode CreateGridDomainNode(AkkaConfiguration akkaConf, IDbConfiguration dbConfig)
        {

            var config = new CustomContainerConfiguration(container =>
            {
                container.RegisterType<ISagaFactory<SagaForRecycling, StartEvent>, SagaForRecyclingFactory>();
                container.RegisterType<ISagaFactory<SagaForRecycling, State>, SagaForRecyclingFactory>();
                container.RegisterType<IEmptySagaFactory<SagaForRecycling>, SagaForRecyclingFactory>();
                container.RegisterInstance<IQuartzConfig>(new InMemoryQuartzConfig());
            });

            return new GridDomainNode(config, new SagaForRecyclingRouteMap(), TransportMode.Standalone, Sys);
        }
    }
}
