using System;
using Akka.Actor;
using GridDomain.Common;
using GridDomain.CQRS.Messaging;
using GridDomain.EventSourcing;
using GridDomain.EventSourcing.Adapters;
using GridDomain.Node;
using GridDomain.Node.AkkaMessaging;
using GridDomain.Node.Configuration.Composition;
using GridDomain.Scheduling.Quartz;
using GridDomain.Tests.Framework.Configuration;
using GridDomain.Tests.Unit.EventsUpgrade;
using GridDomain.Tests.Unit.EventsUpgrade.Domain;
using GridDomain.Tools.Repositories;
using GridDomain.Tools.Repositories.AggregateRepositories;
using GridDomain.Tools.Repositories.EventRepositories;
using Microsoft.Practices.Unity;
using NUnit.Framework;

namespace GridDomain.Tests.Acceptance.EventsUpgrade
{
    [TestFixture]
    public class Given_aggregate_with_upgraded_event_with_new_field_Persistent: Given_aggregate_with_upgraded_event_with_new_field
    {

        protected override bool ClearDataOnStart { get; } = true;

        public Given_aggregate_with_upgraded_event_with_new_field_Persistent():base(false)
        {
            
        }
        protected override IContainerConfiguration CreateConfiguration()
        {
            return new CustomContainerConfiguration(c => c.RegisterAggregate<BalanceAggregate,BalanceAggregatesCommandHandler>(),
                                                    c => c.RegisterInstance<IQuartzConfig>(new PersistedQuartzConfig()));
        }

        protected override IMessageRouteMap CreateMap()
        {
            return new BalanceRouteMap();
        }

        protected override void SaveInJournal<TAggregate>(Guid id, params DomainEvent[] messages)
        {
            using (var eventsRepo = ActorSystemEventRepository.New(new AutoTestAkkaConfiguration(), new EventsAdaptersCatalog()))
            {
                var persistId = AggregateActorName.New<BalanceAggregate>(id).Name;
                eventsRepo.Save(persistId, messages);
            }
        }
        

        public override T LoadAggregate<T>(Guid id)
        {
            var eventsRepo = ActorSystemEventRepository.New(new AutoTestAkkaConfiguration(),GridNode.EventsAdaptersCatalog);
            using (var repo = new AggregateRepository(eventsRepo))
            {
               return repo.LoadAggregate<T>(id);
            }
        }
    }
}