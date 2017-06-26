using System.Diagnostics;

namespace GridDomain.Tests.Unit.EventsUpgrade.Domain.ProjectionBuilders
{
    public class BalanceAggregateChangedProjectionBuilder
    {
        public static int ProjectionGroupHashCode;
        private static readonly Stopwatch Watch = new Stopwatch();

        static BalanceAggregateChangedProjectionBuilder()
        {
            Watch.Start();
        }
    }
}