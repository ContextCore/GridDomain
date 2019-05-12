using System;
using System.Threading.Tasks;
using GridDomain.Aggregates;
using GridDomain.Domains;

namespace GridDomain.Node.Tests
{
    public class CatDomainConfiguration : IDomainConfiguration, IAggregateConfiguration<Cat>, IAggregateSettings
    {
        public async Task Register(IDomainBuilder builder)
        {
            await builder.RegisterAggregate(this);
            builder.RegisterCommandHandler(handler => new CatCommandsHandler(handler));
            builder.RegisterCommandsResultAdapter<Cat>(new CatCommandsResultAdapter());
        }

        public IAggregateFactory<Cat> AggregateFactory { get; } = new AggregateFactory<Cat>();
        public IAggregateSettings Settings  => this;
        public TimeSpan MaxInactivityPeriod { get; set; } = TimeSpan.FromSeconds(1);
        public int SnapshotsKeepAmount { get; set; } = 5;
        public string HostRole { get; } = null;
    }
}