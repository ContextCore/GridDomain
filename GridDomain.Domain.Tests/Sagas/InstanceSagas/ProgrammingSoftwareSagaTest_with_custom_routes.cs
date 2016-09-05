using System;
using System.Diagnostics;
using GridDomain.Common;
using GridDomain.CQRS.Messaging;
using GridDomain.EventSourcing.Sagas.InstanceSagas;
using GridDomain.Node.Configuration.Composition;
using GridDomain.Tests.Sagas.SoftwareProgrammingDomain.Events;
using GridDomain.Tests.SynchroniousCommandExecute;

namespace GridDomain.Tests.Sagas.InstanceSagas
{
    public class ProgrammingSoftwareSagaTest_with_custom_routes : SampleDomainCommandExecutionTests
    {
        protected override IMessageRouteMap CreateMap()
        {
            return new CustomRoutesSoftwareProgrammingSagaMap();
        }

        protected override TimeSpan Timeout => TimeSpan.FromSeconds(2);

        protected override IContainerConfiguration CreateConfiguration()
        {
            var baseConf = base.CreateConfiguration();

            return new CustomContainerConfiguration(
                c => c.RegisterSaga<CustomRoutesSoftwareProgrammingSaga,
                                    SoftwareProgrammingSagaData,
                                    CustomRoutesSoftwareProgrammingSagaFactory,
                                    SleptWellEvent,
                                    GotTiredEvent>(CustomRoutesSoftwareProgrammingSaga.Descriptor),

                c => c.Register(baseConf),
                c => c.RegisterAggregate<SagaDataAggregate<SoftwareProgrammingSagaData>,
                    SagaDataAggregateCommandsHandlerDummy<SoftwareProgrammingSagaData>>()
                );
        }

        public ProgrammingSoftwareSagaTest_with_custom_routes(bool inMemory = true): base(inMemory)
        {
        }
    }
}