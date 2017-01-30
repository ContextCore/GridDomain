using GridDomain.Common;
using GridDomain.CQRS.Messaging;
using GridDomain.Node.Configuration.Akka;
using GridDomain.Tests.Framework;
using GridDomain.Tests.Unit.SampleDomain;

namespace GridDomain.Tests.Unit.Sagas.InstanceSagas
{
    public class SampleDomainCommandExecutionTests : NodeCommandsTest
    {

        protected override IContainerConfiguration CreateConfiguration()
        {
            return new SampleDomainContainerConfiguration();
        }

        protected override IMessageRouteMap CreateMap()
        {
            return new SampleRouteMap();
        }

        public SampleDomainCommandExecutionTests(bool inMemory, AkkaConfiguration config = null) : base(inMemory, config)
        {
        }

        public SampleDomainCommandExecutionTests()
        {
        }
    }
}