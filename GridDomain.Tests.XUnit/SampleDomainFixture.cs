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
        protected override IContainerConfiguration ContainerConfiguration { get; } =
            new SampleDomainContainerConfiguration();

        protected override IMessageRouteMap RouteMap { get; } = new SampleRouteMap();
    }
    
}