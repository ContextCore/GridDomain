using System;
using System.Collections.Generic;
using CommonDomain;

namespace GridDomain.EventSourcing
{
    public class AggregatesSnapshotsFactory : AggregateFactory
    {
        private readonly IDictionary<Type,Func<IMemento, IAggregate>> _creators = new Dictionary<Type, Func<IMemento, IAggregate>>();
        public void Register<T>(Func<IMemento, T> producer) where T : IAggregate
        {
            Register(typeof(T), m=> producer(m));
        }

        public void Register(Type type,Func<IMemento, IAggregate> producer) 
        {
            _creators.Add(type, producer);
        }

        protected override IAggregate BuildFromSnapshot(Type type, Guid id, IMemento snapshot)
        {
            Func<IMemento, IAggregate> factory;

            if (!_creators.TryGetValue(type, out factory))
                throw new CannotFindAggregateCreatorBySnapshotException(type);

            return factory.Invoke(snapshot);
        }
    }
}