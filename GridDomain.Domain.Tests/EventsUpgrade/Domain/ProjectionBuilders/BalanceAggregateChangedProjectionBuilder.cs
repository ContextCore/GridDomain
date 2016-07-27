using System.Diagnostics;

namespace GridDomain.Tests.EventsUpgrade.Domain.ProjectionBuilders
{

    public class BalanceAggregateChangedProjectionBuilder 
    {
        public static int ProjectionGroupHashCode;
        private static Stopwatch watch = new Stopwatch();
        static BalanceAggregateChangedProjectionBuilder()
        {
            watch.Start();
        }
        private int number = 0;
        
    }
}