using System;
using GridDomain.Common;
using GridDomain.CQRS.Messaging;
using GridDomain.EventSourcing.Sagas;
using GridDomain.Node.Configuration.Composition;
using GridDomain.Scheduling.Quartz;
using GridDomain.Tests.CommandsExecution;
using GridDomain.Tests.FutureEvents;
using GridDomain.Tests.Sagas.SoftwareProgrammingDomain.Events;
using GridDomain.Tests.Sagas.StateSagas.SampleSaga;
using Microsoft.Practices.Unity;
using NUnit.Framework;

namespace GridDomain.Tests.Sagas.StateSagas
{
    public class SoftwareProgramming_StateSaga_Test : SampleDomainCommandExecutionTests
    {
        public SoftwareProgramming_StateSaga_Test(bool inMemory = true) : base(inMemory)
        {
        }
        protected IPublisher Publisher { get; private set; }

        [SetUp]
        public void InitPublisher()
        {
            Publisher = GridNode.Container.Resolve<IPublisher>();
        }
        protected override IContainerConfiguration CreateConfiguration()
        {
            return 
                
                new CustomContainerConfiguration(container => {
                   container.RegisterStateSaga<SoftwareProgrammingSaga, SoftwareProgrammingSagaState, SoftwareProgrammingSagaFactory, GotTiredEvent>(SoftwareProgrammingSaga.Descriptor);
                   container.RegisterInstance<IQuartzConfig>(new InMemoryQuartzConfig());
            });
        }

        protected override IMessageRouteMap CreateMap()
        {
            return new SoftwareProgrammingSagaRoutemap();
        }
        
    }
}