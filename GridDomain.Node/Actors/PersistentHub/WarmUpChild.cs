using System;

namespace GridDomain.Node.Actors.PersistentHub {
    class WarmUpChild
    {
        public WarmUpChild(string id)
        {
            Id = id;
        }
        public string Id { get; }
    }
}