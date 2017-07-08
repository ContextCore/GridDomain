using System;

namespace GridDomain.Node.Actors.PersistentHub
{
    public class ShutdownChild
    {
        public ShutdownChild(Guid childId)
        {
            ChildId = childId;
        }

        public Guid ChildId { get; }
    }
}