using System;
using GridDomain.EventSourcing.CommonDomain;

namespace GridDomain.EventSourcing
{
    public class InvalidDefaultMementoException : Exception
    {
        public InvalidDefaultMementoException()
        {
            
        }
        public InvalidDefaultMementoException(Type type, string id, IMemento snapshot)
            : base("Aggregate cannot be constructed from snapshot by default convention, memento is not IAggregate")
        {
            Snapshot = snapshot;
        }

        public IMemento Snapshot { get; set; }
        public Type Type { get; set; }
        public string Id { get; set; }
    }
}