using System.Diagnostics;
using System.Threading.Tasks;
using GridDomain.CQRS;
using GridDomain.Tests.XUnit.SampleDomain.Events;

namespace GridDomain.Tests.XUnit.SampleDomain.ProjectionBuilders
{
    public class AggregateChangedProjectionBuilder : IHandler<SampleAggregateChangedEvent>
    {
        public static int ProjectionGroupHashCode;
        private static readonly Stopwatch Watch = new Stopwatch();
        static AggregateChangedProjectionBuilder()
        {
            Watch.Start();
        }
        private int _number;
        public virtual Task Handle(SampleAggregateChangedEvent msg)
        {
            msg.History.ProjectionGroupHashCode = ProjectionGroupHashCode;
            msg.History.SequenceNumber = ++_number;
            msg.History.ElapsedTicksFromAppStart = Watch.ElapsedTicks;
            return Task.CompletedTask;
        }
    }
}