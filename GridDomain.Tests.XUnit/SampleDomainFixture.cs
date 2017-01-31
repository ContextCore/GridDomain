using GridDomain.Common;
using GridDomain.CQRS.Messaging;
using GridDomain.Node.Configuration.Akka;
using GridDomain.Node.Configuration.Composition;
using GridDomain.Tests.XUnit.SampleDomain;
using Xunit;
using Xunit.Abstractions;

namespace GridDomain.Tests.XUnit
{
    public class SampleDomainFixture : NodeTestFixture
    {
        private SampleDomainFixture(IContainerConfiguration config = null, IMessageRouteMap map = null)
            :base(ContainerConfiguration(config), Map(map))
        {
            
        }
        public SampleDomainFixture(): this(null,null)
        {
            
        }

        private static IMessageRouteMap Map(IMessageRouteMap map)
        {
            if (map == null) return new SampleRouteMap();
            return new CompositeRouteMap(map, new SampleRouteMap());
        }

        private static IContainerConfiguration ContainerConfiguration(IContainerConfiguration config)
        {
            if(config == null) return new SampleDomainContainerConfiguration();
            return new CustomContainerConfiguration(new SampleDomainContainerConfiguration(), new SampleDomainContainerConfiguration());
        }

        public static SampleDomainFixture WithMap(IMessageRouteMap map)
        {
            return new SampleDomainFixture(null,map);
        }

        public static SampleDomainFixture WithConfig(IContainerConfiguration config)
        {
            return new SampleDomainFixture(config);
        }
    }
    
}