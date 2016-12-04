using System;

namespace GridDomain.Tests.LooseCommandOnPoolResize
{
    internal class Stop
    {
        public Stop(Guid id)
        {
            Id = id;
        }

        public Guid Id { get; }
    }
}