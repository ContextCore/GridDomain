using System;
using GridDomain.Common;
using GridDomain.CQRS.Messaging;
using GridDomain.Tests.XUnit.SampleDomain;

namespace GridDomain.Tests.XUnit
{
    public class SampleDomainFixture : NodeTestFixture
    {
        public SampleDomainFixture()
        {
            Add(new SampleDomainContainerConfiguration());
            Add(new SampleRouteMap());
        }
    }
    
}