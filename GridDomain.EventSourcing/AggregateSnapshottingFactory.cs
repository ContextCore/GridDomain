using System;
using GridDomain.EventSourcing.CommonDomain;

namespace GridDomain.EventSourcing
{
    public class AggregateSnapshottingFactory<T> : AggregateFactory where T : Aggregate
    {
        private readonly Func<IMemento, T> _creator;

        public AggregateSnapshottingFactory(Func<IMemento, T> creator=null)
        {
            _creator = creator;
        }

        protected override Aggregate BuildFromSnapshot(Type type, Guid id, IMemento snapshot)
        {
            return _creator == null ? base.BuildFromSnapshot(type, id, snapshot) : _creator(snapshot);
        }
    }
}