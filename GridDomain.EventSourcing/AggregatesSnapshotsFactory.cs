using System;
using System.Collections.Generic;
using GridDomain.EventSourcing.CommonDomain;

namespace GridDomain.EventSourcing
{
    public class AggregatesSnapshotsFactory : AggregateFactory
    {
        private readonly IDictionary<Type, Func<IMemento, Aggregate>> _creators =
            new Dictionary<Type, Func<IMemento, Aggregate>>();

        public static AggregatesSnapshotsFactory New<T>(Func<IMemento, T> producer) where T : Aggregate
        {
            var factory = new AggregatesSnapshotsFactory();
            factory.Register(producer);
            return factory;
        }

        protected void Register<T>(Func<IMemento, T> producer) where T : Aggregate
        {
            Register(typeof(T), producer);
        }

        private void Register(Type type, Func<IMemento, Aggregate> producer)
        {
            _creators.Add(type, producer);
        }

        protected override IAggregate BuildFromSnapshot(Type type, Guid id, IMemento snapshot)
        {
            return _creators.TryGetValue(type, out var factory) ? 
                factory.Invoke(snapshot) : 
                base.BuildFromSnapshot(type, id, snapshot);
        }
    }
}