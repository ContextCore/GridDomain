using GridDomain.Common;
using GridDomain.CQRS.Messaging;
using GridDomain.Node.Configuration.Composition;
using GridDomain.Scheduling.Quartz;
using GridDomain.Tests.Unit.CommandsExecution;
using GridDomain.Tests.Unit.Sagas.SoftwareProgrammingDomain.Events;
using GridDomain.Tests.Unit.Sagas.StateSagas.SampleSaga;
using Microsoft.Practices.Unity;

namespace GridDomain.Tests.Unit.Sagas.StateSagas
{
    public class SoftwareProgrammingStateSagaTest : SampleDomainCommandExecutionTests
    {
        public SoftwareProgrammingStateSagaTest(bool inMemory = true) : base(inMemory)
        {
        }
     
        protected override IContainerConfiguration CreateConfiguration()
        {
            return 
                new CustomContainerConfiguration(container => {
                   container.RegisterStateSaga<SoftwareProgrammingSaga, 
                                               SoftwareProgrammingSagaState, 
                                               SoftwareProgrammingSagaFactory, 
                                               GotTiredEvent>(SoftwareProgrammingSaga.Descriptor);
                   container.RegisterInstance<IQuartzConfig>(new InMemoryQuartzConfig());
            });
        }

        protected override IMessageRouteMap CreateMap()
        {
            return new SoftwareProgrammingSagaRoutemap();
        }
        
    }
}