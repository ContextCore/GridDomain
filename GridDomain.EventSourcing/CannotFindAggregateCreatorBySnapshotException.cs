using System;

namespace GridDomain.EventSourcing
{
    public class CannotFindAggregateCreatorBySnapshotException : Exception
    {
        public CannotFindAggregateCreatorBySnapshotException(Type type)
        {
            Type = type;
        }

        public Type Type { get; set; }
    }
}