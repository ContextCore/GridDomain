using System;

namespace GridDomain.Node.Actors
{
    class ShutdownRequest
    {
        public Guid ChildId;

        public ShutdownRequest(Guid childId)
        {
            ChildId = childId;
        }
    }
}