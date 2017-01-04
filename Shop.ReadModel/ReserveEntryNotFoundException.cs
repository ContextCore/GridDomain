using System;

namespace Shop.ReadModel
{
    public class ReserveEntryNotFoundException : Exception
    {
        public Guid ReserveId { get; }

        public ReserveEntryNotFoundException(Guid reserveId)
        {
            ReserveId = reserveId;
        }
    }
}