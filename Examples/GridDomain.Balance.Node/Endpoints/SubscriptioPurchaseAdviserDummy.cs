using System;

namespace BusinessNews.Node.Endpoints
{
    public static class SubscriptioPurchaseAdviserDummy
    {
        public static bool TryGetPurchaseURL(Guid businessId, string[] claims, out string url)
        {
            url = "";
            return true;
        }
    }
}