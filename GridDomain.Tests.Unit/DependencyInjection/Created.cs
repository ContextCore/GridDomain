using GridDomain.EventSourcing;

namespace GridDomain.Tests.Unit.DependencyInjection {
    internal class Created : DomainEvent
    {
        public int InitialValue { get; }
        public int Produced { get; }

        public Created(string id, int initialValue, int produced):base(id)
        {
            InitialValue = initialValue;
            Produced = produced;
        }
    }
}