using System;

namespace GridDomain.Node.Actors
{
    public interface IPersistenceEntityActorMaintainanceMessage
    {
        Guid Id { get; }
    }
}