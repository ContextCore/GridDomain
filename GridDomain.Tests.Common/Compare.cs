using System.Linq;
using KellermanSoftware.CompareNetObjects;

namespace GridDomain.Tests.Common {
    public static class Compare
    {
        public static CompareLogic Ignore(params string[] members)
        {
            return new CompareLogic(
                new ComparisonConfig
                {
                    MembersToIgnore = members.ToList()
                } );
        }
    }
}