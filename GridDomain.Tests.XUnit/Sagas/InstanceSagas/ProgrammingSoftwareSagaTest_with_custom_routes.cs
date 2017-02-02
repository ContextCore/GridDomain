using GridDomain.Common;
using GridDomain.CQRS.Messaging;
using GridDomain.EventSourcing.Sagas.InstanceSagas;
using GridDomain.Node.Configuration.Composition;

namespace GridDomain.Tests.XUnit.Sagas.InstanceSagas
{
    public class ProgrammingSoftwareSagaTest_with_custom_routes : SampleDomainCommandExecutionTests
    {
        protected override IMessageRouteMap CreateMap()
        {
            return new CustomRoutesSoftwareProgrammingSagaMap();
        }


        protected override IContainerConfiguration CreateConfiguration()
        {
            var baseConf = base.CreateConfiguration();

            return new CustomContainerConfiguration(
                c => c.Register(new SoftwareProgrammingSagaContainerConfiguration()),
                c => c.Register(baseConf),
                c => c.RegisterAggregate<SagaStateAggregate<SoftwareProgrammingSagaData>,
                                         SagaDataAggregateCommandsHandlerDummy<SoftwareProgrammingSagaData>>()
                );
        }

        public ProgrammingSoftwareSagaTest_with_custom_routes(bool inMemory = true): base(inMemory)
        {
        }
    }
}