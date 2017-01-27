using GridDomain.Common;
using GridDomain.CQRS.Messaging;
using GridDomain.Node.Configuration.Akka;
using GridDomain.Tests.XUnit.SampleDomain;
using Xunit;
using Xunit.Abstractions;

namespace GridDomain.Tests.XUnit
{
    public class SampleDomainFixture : NodeTestFixture
    {
        private readonly IContainerConfiguration _containerConfiguration1 = new SampleDomainContainerConfiguration();
        private readonly IMessageRouteMap _routeMap1 = new SampleRouteMap();

        protected override IContainerConfiguration CreateContainerConfiguration()
        {
            return _containerConfiguration1;
        }

        protected override IMessageRouteMap CreateRouteMap()
        {
            return _routeMap1;
        }
    }
    
}