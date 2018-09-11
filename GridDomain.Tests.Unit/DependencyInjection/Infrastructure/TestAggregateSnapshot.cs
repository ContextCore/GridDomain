using GridDomain.EventSourcing.CommonDomain;

namespace GridDomain.Tests.Unit.DependencyInjection.Infrastructure {
    public class TestAggregateSnapshot : ISnapshot
    {
        public string Id { get; set; }
        public int Version { get; set; }
        public string Value { get; set; }
    }
}