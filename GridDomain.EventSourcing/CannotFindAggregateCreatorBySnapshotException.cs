using System;

namespace GridDomain.EventSourcing
{
    public class CannotFindAggregateCreatorBySnapshotException : Exception
    {
        public Type Type { get; set; }

        public CannotFindAggregateCreatorBySnapshotException(Type type)
        {
            Type = type;
        }
    }
}