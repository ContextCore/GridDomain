using System;
using System.Threading.Tasks;
using GridDomain.Common;
using GridDomain.CQRS.Messaging;
using GridDomain.EventSourcing;
using GridDomain.EventSourcing.Adapters;
using GridDomain.Node.AkkaMessaging;
using GridDomain.Node.Configuration.Composition;
using GridDomain.Scheduling.Quartz;
using GridDomain.Tests.Framework.Configuration;
using GridDomain.Tests.XUnit;
using GridDomain.Tests.XUnit.EventsUpgrade;
using GridDomain.Tests.XUnit.EventsUpgrade.Domain;
using GridDomain.Tools.Repositories.AggregateRepositories;
using GridDomain.Tools.Repositories.EventRepositories;
using Microsoft.Practices.Unity;
using Xunit.Abstractions;

namespace GridDomain.Tests.Acceptance.XUnit.EventsUpgrade
{
   
    public class Given_aggregate_with_upgraded_event_with_new_field_Persistent: Given_aggregate_with_upgraded_event_with_new_field
    {

        class Fixture : NodeTestFixture
        {
            public Fixture() 
            {
                Add(new CustomContainerConfiguration(
                    c => c.RegisterAggregate<BalanceAggregate, BalanceAggregatesCommandHandler>()));
                Add(new BalanceRouteMap());
                InMemory = false;
            }
        }

        //protected override async Task SaveToJournal<TAggregate>(Guid id, params DomainEvent[] messages)
        //{
        //    using (var eventsRepo = ActorSystemEventRepository.New(new AutoTestAkkaConfiguration(), new EventsAdaptersCatalog()))
        //    {
        //        var persistId = AggregateActorName.New<BalanceAggregate>(id).Name;
        //        await eventsRepo.Save(persistId, messages);
        //    }
        //}
        

        //public override T LoadAggregate<T>(Guid id)
        //{
        //    var eventsRepo = ActorSystemEventRepository.New(new AutoTestAkkaConfiguration(),Node.EventsAdaptersCatalog);
        //    using (var repo = new AggregateRepository(eventsRepo))
        //    {
        //       return repo.LoadAggregate<T>(id);
        //    }
        //}
        public Given_aggregate_with_upgraded_event_with_new_field_Persistent(ITestOutputHelper output) : base(output) {}
    }
}