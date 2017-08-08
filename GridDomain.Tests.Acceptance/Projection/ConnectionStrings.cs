using System.Configuration;
using GridDomain.Tests.Common;

namespace GridDomain.Tests.Acceptance.Projection {
    public static class ConnectionStrings
    {
        static ConnectionStrings()
        {
            AutoTestDb = ConfigurationManager.ConnectionStrings["AutoTestDb"]?.ConnectionString
                         ??
                         new AutoTestLocalDbConfiguration().ReadModelConnectionString;
        }

        public static string AutoTestDb { get; }
    }
}