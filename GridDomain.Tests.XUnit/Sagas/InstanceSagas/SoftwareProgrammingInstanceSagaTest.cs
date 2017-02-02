using GridDomain.Common;
using GridDomain.CQRS.Messaging;
using GridDomain.EventSourcing.Sagas.InstanceSagas;
using GridDomain.Node.Configuration.Composition;

namespace GridDomain.Tests.XUnit.Sagas.InstanceSagas
{
    public class SoftwareProgrammingInstanceSagaTest : SampleDomainCommandExecutionTests
    {
        protected override IMessageRouteMap CreateMap()
        {
            return new SoftwareProgrammingSagaRoutes();
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

        protected SoftwareProgrammingInstanceSagaTest(bool inMemory = true) : base(inMemory)
        {
        }
    }
}