using System;

namespace Shop.ReadModel
{
    internal class SkuStockEntryNotFoundException : Exception
    {
        public Guid SourceId { get; }


        public SkuStockEntryNotFoundException(Guid sourceId)
        {
            this.SourceId = sourceId;
        }

    }
}