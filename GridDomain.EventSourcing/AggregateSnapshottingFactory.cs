using System;
using CommonDomain;

namespace GridDomain.EventSourcing
{
    public class AggregateSnapshottingFactory<T> : AggregateFactory where T : IAggregate
    {
        private readonly Func<IMemento, T> _creator;

        public AggregateSnapshottingFactory(Func<IMemento, T> creator)
        {
            _creator = creator;
        }

        protected override IAggregate BuildFromSnapshot(Type type, Guid id, IMemento snapshot)
        {
            return _creator(snapshot);
        }
    }
}