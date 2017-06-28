using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GridDomain.CQRS.Messaging;
using GridDomain.Node.Configuration.Composition;
using GridDomain.Tests.Unit.FutureEvents.Infrastructure;

namespace GridDomain.Tests.Unit.DependencyInjection.FutureEvents.Configuration
{
    class FutureAggregateDependenciesFactory : DefaultAggregateDependencyFactory<FutureEventsAggregate>
    {
        public FutureAggregateDependenciesFactory():base(() => new FutureEventsAggregatesCommandHandler())
        {
        }
    }

    class FutureAggregateDomainConfiguration : IDomainConfiguration
    {
        public void Register(IDomainBuilder builder)
        {
            builder.RegisterAggregate(new FutureAggregateDependenciesFactory());
        }
    }
}
