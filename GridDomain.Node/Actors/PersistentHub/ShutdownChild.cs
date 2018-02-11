using System;

namespace GridDomain.Node.Actors.PersistentHub
{
    public class ShutdownChild
    {
        public ShutdownChild(string childId)
        {
            ChildId = childId;
        }

        public string ChildId { get; }
    }
}