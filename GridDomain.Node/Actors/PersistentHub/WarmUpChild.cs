using System;

namespace GridDomain.Node.Actors.PersistentHub {
    class WarmUpChild
    {
        public WarmUpChild(Guid id)
        {
            Id = id;
        }
        public Guid Id { get; }
    }
}