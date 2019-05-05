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
        }

        public IAggregateFactory<Cat> AggregateFactory { get; } = new AggregateFactory<Cat>();
        public IAggregateSettings Settings  => this;
        public ICommandsResultAdapter CommandsResultAdapter { get; } = new CatCommandsResultResultAdapter();
        public TimeSpan MaxInactivityPeriod { get; set; } = TimeSpan.FromSeconds(1);
        public int SnapshotsKeepAmount { get; set; } = 5;
    }
}