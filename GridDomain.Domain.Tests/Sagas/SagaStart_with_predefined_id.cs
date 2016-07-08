using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GridDomain.CQRS.Messaging;
using GridDomain.CQRS.Messaging.MessageRouting;
using GridDomain.EventSourcing.Sagas;
using GridDomain.Node;
using GridDomain.Node.Configuration.Akka;
using GridDomain.Node.Configuration.Persistence;
using GridDomain.Scheduling.Quartz;
using GridDomain.Tests.Framework;
using GridDomain.Tests.Framework.Configuration;
using GridDomain.Tests.FutureEvents;
using GridDomain.Tests.Sagas.SubscriptionRenew;
using GridDomain.Tests.Sagas.SubscriptionRenew.Events;
using Microsoft.Practices.Unity;
using NUnit.Framework;

namespace GridDomain.Tests.Sagas
{

    public class RenewSagaMessageRouteMap : IMessageRouteMap
    {
        public void Register(IMessagesRouter router)
        {
            router.RegisterSaga(SubscriptionRenewSaga.Descriptor);
        }
    }

    [TestFixture]
    public class SagaStart_with_predefined_id : NodeCommandsTest
    {

        [Test]
        public void When_start_message_has_saga_id_Saga_starts_with_it()
        {
            var publisher = GridNode.Container.Resolve<IPublisher>();
            var sagaId = Guid.NewGuid();

            Thread.Sleep(TimeSpan.FromMilliseconds(500));

            publisher.Publish(new SubscriptionExpiredEvent(Guid.NewGuid()).CloneWithSaga(sagaId));

            Thread.Sleep(Debugger.IsAttached ? TimeSpan.FromSeconds(1000): TimeSpan.FromSeconds(1));

            var sagaState = LoadSagaState<SubscriptionRenewSaga,
                                          SubscriptionRenewSagaState,
                                          SubscriptionExpiredEvent>(sagaId);

            Assert.AreEqual(sagaId,sagaState.Id);
            Assert.AreEqual(SubscriptionRenewSaga.States.OfferPaying, sagaState.MachineState);
        }

        public SagaStart_with_predefined_id() : base(new AutoTestAkkaConfiguration().ToStandAloneInMemorySystemConfig(),"TestSagaStart", false)
        {
        }

        protected override TimeSpan Timeout { get; }
        protected override GridDomainNode CreateGridDomainNode(AkkaConfiguration akkaConf, IDbConfiguration dbConfig)
        {

            var config = new CustomContainerConfiguration(container => { 
                    container.RegisterType<ISagaFactory<SubscriptionRenewSaga, SubscriptionRenewSagaState>, SubscriptionRenewSagaFactory>();
                    container.RegisterType<ISagaFactory<SubscriptionRenewSaga, SubscriptionExpiredEvent>, SubscriptionRenewSagaFactory>();
                    container.RegisterType<IEmptySagaFactory<SubscriptionRenewSaga>, SubscriptionRenewSagaFactory>();
                    container.RegisterInstance<IQuartzConfig>(new InMemoryQuartzConfig());
             });

            return new GridDomainNode(config, new RenewSagaMessageRouteMap(),TransportMode.Standalone, Sys);
        }
    }
}
