using System.Diagnostics;
using GridDomain.CQRS;
using GridDomain.Tests.Acceptance.EventsUpgrade.SampleDomain.Events;

namespace GridDomain.Tests.Acceptance.EventsUpgrade.SampleDomain.ProjectionBuilders
{

    public class AggregateChangedProjectionBuilder 
    {
        public static int ProjectionGroupHashCode;
        private static Stopwatch watch = new Stopwatch();
        static AggregateChangedProjectionBuilder()
        {
            watch.Start();
        }
        private int number = 0;
        
    }
}