using System;

namespace Shop.ReadModel
{
    internal class SkuStockEntryNotFoundException : Exception
    {
        public SkuStockEntryNotFoundException(Guid sourceId)
        {
            SourceId = sourceId;
        }

        public Guid SourceId { get; }
    }
}