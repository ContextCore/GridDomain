using System.Diagnostics;
using GridDomain.CQRS;
using GridDomain.Tests.Acceptance.EventsUpgrade.SampleDomain.Events;

namespace GridDomain.Tests.Acceptance.EventsUpgrade.SampleDomain.ProjectionBuilders
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