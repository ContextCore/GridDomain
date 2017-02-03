using System;
using GridDomain.Common;
using GridDomain.CQRS.Messaging;
using GridDomain.Tests.XUnit.SampleDomain;

namespace GridDomain.Tests.XUnit
{
    public class SampleDomainFixture : NodeTestFixture
    {
        public SampleDomainFixture(IContainerConfiguration config = null, IMessageRouteMap map = null, TimeSpan? timeout=null)
            :base(config, map, timeout)
        {
            
        }

        protected override IContainerConfiguration CreateContainerConfiguration()
        {
            return new SampleDomainContainerConfiguration();
        }

        protected override IMessageRouteMap CreateRouteMap()
        {
            return new SampleRouteMap();
        }
    }
    
}