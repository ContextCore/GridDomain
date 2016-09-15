using System;

namespace GridDomain.Tests.Sagas.SoftwareProgrammingDomain.Commands
{
    public class Slept
    {
        public Guid SofaId { get; }

        public Slept(Guid sofaId)
        {
            SofaId = sofaId;
        }
    }
}