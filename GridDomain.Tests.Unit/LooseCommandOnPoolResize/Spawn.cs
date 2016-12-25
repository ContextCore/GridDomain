using System;

namespace GridDomain.Tests.Unit.LooseCommandOnPoolResize
{
    internal class Spawn
    {
        public Spawn(Guid id)
        {
            Id = id;
        }

        public Guid Id { get; }
    }
}