using System;
using GridDomain.Common;
using GridDomain.CQRS.Messaging;
using GridDomain.EventSourcing.Sagas.InstanceSagas;
using GridDomain.Node.Configuration.Composition;
using GridDomain.Tests.Unit.CommandsExecution;
using GridDomain.Tests.Unit.Sagas.SoftwareProgrammingDomain.Events;

namespace GridDomain.Tests.Unit.Sagas.InstanceSagas
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
                c => c.Register(new SoftwareProgrammingSagaContainerConfiguration()),
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