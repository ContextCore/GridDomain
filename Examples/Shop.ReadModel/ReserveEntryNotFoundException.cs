using System;

namespace Shop.ReadModel
{
    public class ReserveEntryNotFoundException : Exception
    {
        public ReserveEntryNotFoundException(Guid reserveId)
        {
            ReserveId = reserveId;
        }

        public Guid ReserveId { get; }
    }
}