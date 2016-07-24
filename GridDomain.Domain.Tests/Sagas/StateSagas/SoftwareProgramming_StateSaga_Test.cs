using System;
using GridDomain.CQRS.Messaging;
using GridDomain.EventSourcing.Sagas;
using GridDomain.Node.Configuration.Composition;
using GridDomain.Scheduling.Quartz;
using GridDomain.Tests.FutureEvents;
using GridDomain.Tests.Sagas.SoftwareProgrammingDomain.Events;
using GridDomain.Tests.Sagas.StateSagas.SampleSaga;
using GridDomain.Tests.SynchroniousCommandExecute;
using Microsoft.Practices.Unity;

namespace GridDomain.Tests.Sagas.StateSagas
{
    public class SoftwareProgramming_StateSaga_Test : SampleDomainCommandExecutionTests
    {
        public SoftwareProgramming_StateSaga_Test(bool inMemory = true) : base(inMemory)
        {
        }

        protected override IContainerConfiguration CreateConfiguration()
        {
            return  new CustomContainerConfiguration(container => {
                                                                      container.RegisterType<ISagaFactory<SoftwareProgrammingSaga, SoftwareProgrammingSagaState>, SoftwareProgrammingSagaFactory>();
                                                                      container.RegisterType<ISagaFactory<SoftwareProgrammingSaga, GotTiredEvent>, SoftwareProgrammingSagaFactory>();
                                                                      container.RegisterType<ISagaFactory<SoftwareProgrammingSaga, Guid>, SoftwareProgrammingSagaFactory>();
                                                                      container.RegisterInstance<IQuartzConfig>(new InMemoryQuartzConfig());
            });
        }

        protected override IMessageRouteMap CreateMap()
        {
            return new SoftwareProgrammingSagaRoutemap();
        }
        
    }
}