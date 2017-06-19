using System;
using System.Collections.Generic;
using GridDomain.EventSourcing.CommonDomain;

namespace GridDomain.EventSourcing
{
    public class AggregatesSnapshotsFactory : AggregateFactory
    {
        private readonly IDictionary<Type, Func<IMemento, Aggregate>> _creators =
            new Dictionary<Type, Func<IMemento, Aggregate>>();

        public void Register<T>(Func<IMemento, T> producer) where T : Aggregate
        {
            Register(typeof(T), m => producer(m));
        }

        public void Register(Type type, Func<IMemento, Aggregate> producer)
        {
            _creators.Add(type, producer);
        }

        protected override Aggregate BuildFromSnapshot(Type type, Guid id, IMemento snapshot)
        {
            Func<IMemento, Aggregate> factory;

            if (_creators.TryGetValue(type, out factory))
                return factory.Invoke(snapshot);

            return base.BuildFromSnapshot(type, id, snapshot);
        }
    }
}