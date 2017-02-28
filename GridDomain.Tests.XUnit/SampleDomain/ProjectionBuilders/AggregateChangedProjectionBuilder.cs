using System.Diagnostics;
using System.Threading.Tasks;
using GridDomain.CQRS;
using GridDomain.Tests.XUnit.SampleDomain.Events;

namespace GridDomain.Tests.XUnit.SampleDomain.ProjectionBuilders
{
    public class AggregateChangedProjectionBuilder : IHandler<SampleAggregateChangedEvent>
    {
        private static readonly Stopwatch Watch = new Stopwatch();

        static AggregateChangedProjectionBuilder()
        {
            Watch.Start();
        }

        public virtual Task Handle(SampleAggregateChangedEvent msg)
        {
            msg.History.SequenceNumber = int.Parse(msg.Value);
            msg.History.ElapsedTicksFromAppStart = Watch.ElapsedTicks;
            return Task.CompletedTask;
        }
    }
}