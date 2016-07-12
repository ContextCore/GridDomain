using System;
using System.Diagnostics;
using System.Threading;
using Akka.Actor;
using GridDomain.CQRS.Messaging;
using GridDomain.Node;
using GridDomain.Node.Configuration.Composition;
using GridDomain.Scheduling.Quartz;
using GridDomain.Tests.FutureEvents;
using GridDomain.Tests.SampleDomain;
using Microsoft.Practices.Unity;
using Quartz;

namespace GridDomain.Tests.SynchroniousCommandExecute
{

    public class InMemorySampleDomainTests : SampleDomainCommandExecutionTests
    {
        public InMemorySampleDomainTests() : base(true)
        {
        }
    }

    public class SampleDomainCommandExecutionTests : ExtendedNodeCommandTest
    {
        protected override TimeSpan Timeout => Debugger.IsAttached
            ? TimeSpan.FromMinutes(10)
            : TimeSpan.FromSeconds(5);

        protected override IContainerConfiguration CreateConfiguration()
        {
            return  new CustomContainerConfiguration(
                               c => c.RegisterAggregate<SampleAggregate, TestAggregatesCommandHandler>(),
                               c => c.RegisterInstance<IQuartzConfig>(new InMemoryQuartzConfig()),
                               c => c.RegisterType<AggregateCreatedProjectionBuilder>(),
                               c => c.RegisterType<SampleProjectionBuilder>());
        }

        protected override IMessageRouteMap CreateMap()
        {
            var container = new UnityContainer();
            container.Register(CreateConfiguration());
            return new TestRouteMap(container);

        }
      
        
        public SampleDomainCommandExecutionTests(bool inMemory) : base(inMemory)
        {
        }
    }
}
