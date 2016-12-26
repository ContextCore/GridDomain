using System.Collections.Generic;

namespace BusinessNews.Node.Endpoints
{
    public static class SecurityDummy
    {
        public static bool CanAccess<T>(string method, IEnumerable<string> claimsAvailable)
        {
            return true;
        }

        public static string[] ListRequiredClaims<T>(string method)
        {
            return new[] {""};
        }
    }
}