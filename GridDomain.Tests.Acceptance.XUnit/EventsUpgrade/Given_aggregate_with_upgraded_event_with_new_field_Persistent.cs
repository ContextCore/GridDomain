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
using Xunit;
using Xunit.Abstractions;

namespace GridDomain.Tests.Acceptance.XUnit.EventsUpgrade
{
   
    public class Given_aggregate_with_upgraded_event_with_new_field_Persistent: Given_aggregate_with_upgraded_event_with_new_field
    {
        public Given_aggregate_with_upgraded_event_with_new_field_Persistent(ITestOutputHelper helper):
            base(helper, new EventsUpgradeFixture {InMemory = false})
        {
            
        }
    }
}