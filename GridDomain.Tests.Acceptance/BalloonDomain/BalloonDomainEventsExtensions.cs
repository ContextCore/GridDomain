using GridDomain.Tests.XUnit.BalloonDomain.Events;

namespace GridDomain.Tests.Acceptance.XUnit.BalloonDomain
{
    public static class BalloonDomainEventsExtensions
    {
        public static BalloonCatalogItem ToCatalogItem(this BalloonCreated e)
        {
            return new BalloonCatalogItem()
                   {
                       BalloonId = e.SourceId,
                       LastChanged = e.CreatedTime,
                       Title = e.Value
                   };
        }

        public static BalloonCatalogItem ToCatalogItem(this BalloonTitleChanged e)
        {
            return new BalloonCatalogItem()
                   {
                       BalloonId = e.SourceId,
                       LastChanged = e.CreatedTime,
                       Title = e.Value
                   };
        }
    }
}